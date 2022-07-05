using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdofaiTweaks.Core;
using Newtonsoft.Json;
using RainingKeys.Components;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using Object = UnityEngine.Object;

namespace RainingKeys {
    internal static class Startup {
        private const string SettingPath = "Options/RainingKeys.json";

        private static ModConfig _config;

        private static bool _recording;

        private static KeyContainer ContainerTemplate => Positions[_config.position];

        private static KeyContainer _container;

        private static Canvas _canvas;

        private static GameObject _obj;

        private static readonly Dictionary<ViewerPosition, KeyContainer> Positions = new();

        private static void Repaint()
        {
            if (_obj != null && _container != null)
            {
                Object.Destroy(_obj);
                _obj = null;
                _container = null;
            }

            _obj = new GameObject("KeyViewer Canvas");

            Object.DontDestroyOnLoad(_obj);

            var canvas = _obj.AddComponent<Canvas>();

            _canvas = canvas;

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = _obj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            _container = Object.Instantiate(ContainerTemplate, _obj.transform);

            _container.inactiveTextColor = _config.inactiveTextColor.Color;
            _container.activeTextColor = _config.activeTextColor.Color;
            _container.inactiveLineColor = _config.inactiveLineColor.Color;
            _container.activeLineColor = _config.activeLineColor.Color;
            _container.inactiveCountTextColor = _config.inactiveCountTextColor.Color;
            _container.activeCountTextColor = _config.activeCountTextColor.Color;
            _container.rainColor = _config.rainColor.Color;

            _container.size = _config.size;

            _container.position = new Vector2(_config.x, _config.y);

            foreach (var key in _config.keys)
            {
                _container.AddKey(key);
            }
        }

        internal static void SaveConfig()
        {
            File.WriteAllText(SettingPath, JsonConvert.SerializeObject(_config));
        }

        private static void Load(UnityModManager.ModEntry entry)
        {
            entry.OnUpdate = (modEntry, f) =>
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

                if (!showViewer && _config.showOnlyPlaying)
                {
                    showViewer = true;
                }

                if (showViewer != _container.gameObject.activeSelf)
                {
                    _container.gameObject.SetActive(showViewer);
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


            _config = File.Exists(SettingPath)
                ? JsonConvert.DeserializeObject<ModConfig>(File.ReadAllText(SettingPath))
                : new();

            entry.OnGUI = _ =>
            {
                GUILayout.Label("Keys");

                GUILayout.Label(String.Join(", ", _config.keys.Select(i => i.key.ToString())));

                GUILayout.BeginHorizontal();

                if (_recording)
                {
                    if (GUILayout.Button("Stop"))
                    {
                        _recording = false;
                    }

                    GUILayout.Label("Press keys to register or unregister");
                    if (Event.current.isKey && Event.current.type == EventType.KeyDown &&
                        Event.current.keyCode != KeyCode.None)
                    {
                        var deleted = false;

                        for (var i = 0; i < _config.keys.Count; i++)
                        {
                            var key = _config.keys[i];

                            if (key.key == Event.current.keyCode)
                            {
                                _config.keys.RemoveAt(i);
                                deleted = true;
                                break;
                            }
                        }

                        if (!deleted)
                        {
                            _config.keys.Add(new KeyElement
                            {
                                key = Event.current.keyCode,
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

                (_config.inactiveTextColor.Color, _config.activeTextColor.Color) = MoreGUILayout.ColorRgbaSlidersPair(
                    _config.inactiveTextColor.Color, _config.activeTextColor.Color);

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

                (_config.inactiveCountTextColor.Color, _config.activeCountTextColor.Color) =
                    MoreGUILayout.ColorRgbaSlidersPair(
                        _config.inactiveCountTextColor.Color, _config.activeCountTextColor.Color);

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

                (_config.inactiveLineColor.Color, _config.activeLineColor.Color) = MoreGUILayout.ColorRgbaSlidersPair(
                    _config.inactiveLineColor.Color, _config.activeLineColor.Color);

                MoreGUILayout.EndIndent();

                GUILayout.Label("Rain Color");

                MoreGUILayout.BeginIndent();

                _config.rainColor.Color = Util.ColorRgbaSliders(_config.rainColor.Color);

                MoreGUILayout.EndIndent();

                GUILayout.BeginHorizontal();

                _config.x =
                    MoreGUILayout.NamedSlider(
                        "X",
                        _config.x,
                        0f,
                        1f,
                        300f,
                        0.01f,
                        valueFormat: "{0:0.##}");

                _config.y =
                    MoreGUILayout.NamedSlider(
                        "Y",
                        _config.y,
                        0f,
                        1f,
                        300f,
                        0.01f,
                        valueFormat: "{0:0.##}");

                GUILayout.EndHorizontal();

                _config.size = MoreGUILayout.NamedSlider(
                    "Size",
                    _config.size,
                    10f,
                    200f,
                    300f,
                    1f);
                
                GUILayout.BeginHorizontal();
                
                foreach (var name in Enum.GetNames(typeof(ViewerPosition)))
                {
                    if (GUILayout.Button(name))
                    {
                        _config.position = Enum.Parse<ViewerPosition>(name);
                    }
                }
                
                GUILayout.EndHorizontal();

                _config.showOnlyPlaying = GUILayout.Toggle(_config.showOnlyPlaying , "Show key viewer only in play mode");

                if (GUILayout.Button("Reset Counts"))
                {
                    foreach (var key in _config.keys)
                    {
                        key.count = 0;
                    }

                    foreach (var key in _container.Keys)
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
                        _container = null;
                    }
                }

                return true;
            };
        }
    }
}