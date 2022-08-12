using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core;
using HarmonyLib;
using Newtonsoft.Json;
using RainingKeys.Bootstrap;
using RainingKeys.Components;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using Object = UnityEngine.Object;

namespace RainingKeys
{
    internal static class Startup
    {
        private const string SettingPath = "Options/RainingKeys.json";

        public static ModConfig Config;

        private static bool _recording;

        private static KeyContainer ContainerTemplate => Positions[Config.position];

        internal static KeyContainer Container;

        private static Canvas _canvas;

        private static GameObject _obj;

        private static readonly Dictionary<ViewerPosition, KeyContainer> Positions = new();

        private static Font defaultFont;

        private static void Repaint()
        {
            if (!ContainerTemplate)
            {
                return;
            }
            if (_obj != null && Container != null)
            {
                Object.Destroy(_obj);
                _obj = null;
                Container = null;
            }

            Values.RainSpeed = Config.rainSpeed;
            Values.RainTrackSize = Config.trackLength;

            _obj = new GameObject("KeyViewer Canvas");

            Object.DontDestroyOnLoad(_obj);

            var canvas = _obj.AddComponent<Canvas>();

            _canvas = canvas;

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = _obj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvas.sortingOrder = 1000;

            Container = Object.Instantiate(ContainerTemplate, _obj.transform);

            Container.Spacing = Config.spacing;

            Container.inactiveTextColor = Config.inactiveTextColor.Color;
            Container.activeTextColor = Config.activeTextColor.Color;
            Container.inactiveLineColor = Config.inactiveLineColor.Color;
            Container.activeLineColor = Config.activeLineColor.Color;
            Container.inactiveCountTextColor = Config.inactiveCountTextColor.Color;
            Container.activeCountTextColor = Config.activeCountTextColor.Color;
            Container.rainColor = Config.rainColor.Color;

            Container.size = Config.size;

            Container.position = new Vector2(Config.x, Config.y);

            Font font = null;

            if (!string.IsNullOrEmpty(Config.font))
            {
                _fonts.TryGetValue(Config.font, out font);
            }

            font ??= defaultFont;

            Container.font = font;

            foreach (var key in Config.keys)
            {
                Container.AddKey(key);
            }
        }

        internal static void SaveConfig()
        {
            File.WriteAllText(SettingPath, JsonConvert.SerializeObject(Config));
        }

        private static Dictionary<string, Font> _fonts;

        private static void Load(UnityModManager.ModEntry entry)
        {
            Config = File.Exists(SettingPath)
                ? JsonConvert.DeserializeObject<ModConfig>(File.ReadAllText(SettingPath), new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })
                : new();

            defaultFont = RDString.GetFontDataForLanguage(RDString.language).font;

            _fonts = Config!.enableCustomFonts
                ? Font.GetOSInstalledFontNames().ToList()
                    .ToDictionary(x => x, x => Font.CreateDynamicFontFromOSFont(x, 12))
                : new();

            bool showFonts = false;

            entry.OnUpdate = (_, _) =>
            {
                bool showViewer = true;
                if (scrController.instance != null
                    && scrConductor.instance != null)
                {
                    bool playing = !scrController.instance.paused && scrConductor.instance.isGameWorld;
                    showViewer &= playing;
                }
                else
                {
                    showViewer = false;
                }

                if (!showViewer && !Config.showOnlyPlaying)
                {
                    showViewer = true;
                }

                if (showViewer != Container.gameObject.activeSelf)
                {
                    Container.gameObject.SetActive(showViewer);
                }
            };

            var bundle = AssetBundle.LoadFromFile(Path.Combine(entry.Path, "assets.bundle"));

            Positions[ViewerPosition.Top] = bundle
                .LoadAsset<GameObject>("Assets/Prefab/Container_Top.prefab").GetComponent<KeyContainer>();
            Positions[ViewerPosition.Bottom] = bundle
                .LoadAsset<GameObject>("Assets/Prefab/Container_Bottom.prefab").GetComponent<KeyContainer>();
            Positions[ViewerPosition.Left] = bundle
                .LoadAsset<GameObject>("Assets/Prefab/Container_Left.prefab").GetComponent<KeyContainer>();
            Positions[ViewerPosition.Right] = bundle
                .LoadAsset<GameObject>("Assets/Prefab/Container_Right.prefab").GetComponent<KeyContainer>();

            if (!Directory.Exists("Options"))
            {
                Directory.CreateDirectory("Options");
            }

            entry.OnGUI = _ =>
            {
                GUILayout.Label("Keys");

                GUILayout.Label(String.Join(", ", Config.keys.Select(i => i.key.ToString())));

                GUILayout.BeginHorizontal();

                if (_recording)
                {
                    if (GUILayout.Button("Stop"))
                    {
                        _recording = false;
                    }

                    GUILayout.Label("Press keys to register or unregister");

                    Enum.TryParse<KeyCode>($"Mouse{Event.current.button}", out var k);

                    if ((Event.current.isKey && Event.current.type == EventType.KeyDown &&
                         Event.current.keyCode != KeyCode.None) ||
                        (Event.current.isMouse && Event.current.type == EventType.MouseDown))
                    {
                        if (!Event.current.isMouse)
                        {
                            k = Event.current.keyCode;
                        }

                        var deleted = false;

                        for (var i = 0; i < Config.keys.Count; i++)
                        {
                            var key = Config.keys[i];

                            if (key.key == k)
                            {
                                Config.keys.RemoveAt(i);
                                deleted = true;
                                break;
                            }
                        }

                        if (!deleted)
                        {
                            Config.keys.Add(new KeyElement
                            {
                                key = k,
                                count = 0
                            });
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("Select keys"))
                    {
                        _recording = true;
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                GUILayout.Label("Rain Speed");
                var newRainSpeed = GUILayout.TextField(Config.rainSpeed.ToString(CultureInfo.InvariantCulture));
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.Label("Track Length");
                var newTrackSize = GUILayout.TextField(Config.trackLength.ToString(CultureInfo.InvariantCulture));
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                
                GUILayout.Label("Spacing");
                var newSpacing = GUILayout.TextField(Config.spacing.ToString(CultureInfo.InvariantCulture));

                if (float.TryParse(newSpacing, out var spacing))
                {
                    Config.spacing = spacing;
                }
                
                if (float.TryParse(newRainSpeed, out var speed))
                {
                    Config.rainSpeed = speed;
                }

                if (float.TryParse(newTrackSize, out var len))
                {
                    Config.trackLength = len;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(
                    "Inactive Text Color",
                    GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(8f);
                GUILayout.Label(
                    "Active Text Color",
                    GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(20f);
                GUILayout.EndHorizontal();
                MoreGUILayout.BeginIndent();

                (Config.inactiveTextColor.Color, Config.activeTextColor.Color) = MoreGUILayout.ColorRgbaSlidersPair(
                    Config.inactiveTextColor.Color, Config.activeTextColor.Color);

                MoreGUILayout.EndIndent();

                GUILayout.BeginHorizontal();
                GUILayout.Label(
                    "Inactive Count Text Color",
                    GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(8f);
                GUILayout.Label(
                    "Active Count Text Color",
                    GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(20f);
                GUILayout.EndHorizontal();
                MoreGUILayout.BeginIndent();

                (Config.inactiveCountTextColor.Color, Config.activeCountTextColor.Color) =
                    MoreGUILayout.ColorRgbaSlidersPair(
                        Config.inactiveCountTextColor.Color, Config.activeCountTextColor.Color);

                MoreGUILayout.EndIndent();

                GUILayout.BeginHorizontal();
                GUILayout.Label(
                    "Inactive Background Color",
                    GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(8f);
                GUILayout.Label(
                    "Active Background Color",
                    GUILayout.Width(200f));
                GUILayout.FlexibleSpace();
                GUILayout.Space(20f);
                GUILayout.EndHorizontal();
                MoreGUILayout.BeginIndent();

                (Config.inactiveLineColor.Color, Config.activeLineColor.Color) = MoreGUILayout.ColorRgbaSlidersPair(
                    Config.inactiveLineColor.Color, Config.activeLineColor.Color);

                MoreGUILayout.EndIndent();

                GUILayout.Label("Rain Color");

                MoreGUILayout.BeginIndent();

                Config.rainColor.Color = Util.ColorRgbaSliders(Config.rainColor.Color);

                MoreGUILayout.EndIndent();

                GUILayout.BeginHorizontal();

                Config.x =
                    MoreGUILayout.NamedSlider(
                        "X",
                        Config.x,
                        0f,
                        1f,
                        300f,
                        0.01f,
                        valueFormat: "{0:0.##}");

                Config.y =
                    MoreGUILayout.NamedSlider(
                        "Y",
                        Config.y,
                        0f,
                        1f,
                        300f,
                        0.01f,
                        valueFormat: "{0:0.##}");

                GUILayout.EndHorizontal();

                Config.size = MoreGUILayout.NamedSlider(
                    "Size",
                    Config.size,
                    10f,
                    200f,
                    300f,
                    1f);

                GUILayout.BeginHorizontal();

                foreach (var name in Enum.GetNames(typeof(ViewerPosition)))
                {
                    if (GUILayout.Button(name))
                    {
                        Config.position = Enum.Parse<ViewerPosition>(name);
                    }
                }

                GUILayout.EndHorizontal();

                if (Config.enableCustomFonts)
                {
                    GUILayout.Label("Font");

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Show installed font list"))
                    {
                        showFonts = !showFonts;
                    }

                    if (GUILayout.Button("Reset Font"))
                    {
                        Config.font = null;
                    }

                    GUILayout.EndHorizontal();

                    if (showFonts)
                    {
                        MoreGUILayout.BeginIndent();

                        foreach (var font in _fonts)
                        {
                            GUILayout.BeginHorizontal();

                            GUILayout.Label(font.Key, new GUIStyle(GUI.skin.label)
                            {
                                font = font.Value
                            });

                            if (Config.font != font.Key)
                            {
                                if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                                {
                                    Config.font = font.Key;
                                }
                            }

                            GUILayout.EndHorizontal();
                        }

                        MoreGUILayout.EndIndent();
                    }
                }

                Config.showOnlyPlaying =
                    GUILayout.Toggle(Config.showOnlyPlaying, "Show key viewer only in play mode");

                if (GUILayout.Button("Reset Counts"))
                {
                    foreach (var key in Config.keys)
                    {
                        key.count = 0;
                    }

                    foreach (var (_, key) in Container.Keys)
                    {
                        key.countText.text = "0";
                    }
                }
            };
            
            entry.OnSaveGUI = _ =>
            {
                SaveConfig();
                Repaint();
            };

            entry.OnToggle = (_, b) =>
            {
                if (b)
                {
                    Repaint();
                }
                else
                {
                    if (_canvas != null)
                    {
                        Object.Destroy(_canvas);
                        _canvas = null;
                        Container = null;
                    }
                }

                return true;
            };
        }
    }
}