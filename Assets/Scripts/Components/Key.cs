using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace RainingKeys.Components
{
    public class Key : MonoBehaviour
    {
        public Color inactiveTextColor;
        public Color activeTextColor;
        public Color inactiveLineColor;
        public Color activeLineColor;
        public Color rainColor;

        [NonSerialized] public readonly Queue<KeyHighlight> HighlightPool = new();

        public KeyElement elem;

        public KeyHighlight highlightTemplate;

        public Image highlightImage;

        public Text labelText;

        public Color inactiveCountTextColor;

        public Color activeCountTextColor;

        public Text countText;

        public ViewerPosition position;

        private KeyCode Code => elem.key;

        public bool controlled;

        private static readonly Dictionary<KeyCode, string> KeyToString =
            new()
            {
                { KeyCode.Alpha0, "0" },
                { KeyCode.Alpha1, "1" },
                { KeyCode.Alpha2, "2" },
                { KeyCode.Alpha3, "3" },
                { KeyCode.Alpha4, "4" },
                { KeyCode.Alpha5, "5" },
                { KeyCode.Alpha6, "6" },
                { KeyCode.Alpha7, "7" },
                { KeyCode.Alpha8, "8" },
                { KeyCode.Alpha9, "9" },
                { KeyCode.Keypad0, "0" },
                { KeyCode.Keypad1, "1" },
                { KeyCode.Keypad2, "2" },
                { KeyCode.Keypad3, "3" },
                { KeyCode.Keypad4, "4" },
                { KeyCode.Keypad5, "5" },
                { KeyCode.Keypad6, "6" },
                { KeyCode.Keypad7, "7" },
                { KeyCode.Keypad8, "8" },
                { KeyCode.Keypad9, "9" },
                { KeyCode.KeypadPlus, "+" },
                { KeyCode.KeypadMinus, "-" },
                { KeyCode.KeypadMultiply, "*" },
                { KeyCode.KeypadDivide, "/" },
                { KeyCode.KeypadEnter, "↵" },
                { KeyCode.KeypadEquals, "=" },
                { KeyCode.KeypadPeriod, "." },
                { KeyCode.Return, "↵" },
                { KeyCode.None, " " },
                { KeyCode.Tab, "⇥" },
                { KeyCode.Backslash, "\\" },
                { KeyCode.Slash, "/" },
                { KeyCode.Minus, "-" },
                { KeyCode.Equals, "=" },
                { KeyCode.LeftBracket, "[" },
                { KeyCode.RightBracket, "]" },
                { KeyCode.Semicolon, ";" },
                { KeyCode.Comma, "," },
                { KeyCode.Period, "." },
                { KeyCode.Quote, "'" },
                { KeyCode.UpArrow, "↑" },
                { KeyCode.DownArrow, "↓" },
                { KeyCode.LeftArrow, "←" },
                { KeyCode.RightArrow, "→" },
                { KeyCode.Space, "␣" },
                { KeyCode.BackQuote, "`" },
                { KeyCode.LeftShift, "L⇧" },
                { KeyCode.RightShift, "R⇧" },
                { KeyCode.LeftControl, "LCtrl" },
                { KeyCode.RightControl, "RCtrl" },
                { KeyCode.LeftAlt, "LAlt" },
                { KeyCode.RightAlt, "AAlt" },
                { KeyCode.Delete, "Del" },
                { KeyCode.PageDown, "Pg↓" },
                { KeyCode.PageUp, "Pg↑" },
                { KeyCode.Insert, "Ins" },
            };

        private void Awake()
        {
#if UNITY_EDITOR
            countText.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/NanumSquareRoundB.ttf");
            labelText.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/NanumSquareRoundB.ttf");
#endif

            for (int i = 0; i < 30; i++)
            {
                var o = Instantiate(highlightTemplate, highlightTemplate.transform.parent);
                o.gameObject.SetActive(false);
                HighlightPool.Enqueue(o);
            }
            
            Up();
        }

        private void Start()
        {
            if (!KeyToString.TryGetValue(Code, out string code))
            {
                code = Code.ToString();
            }

            labelText.text = code;

            countText.text = $"{elem.count}";

            var rt = (RectTransform)highlightTemplate.transform.parent;

            var size = rt.sizeDelta;

            switch (position)
            {
                case ViewerPosition.Left:
                case ViewerPosition.Right:
                    size.x = Values.RainTrackSize;
                    break;
                case ViewerPosition.Top:
                case ViewerPosition.Bottom:
                    size.y = Values.RainTrackSize;
                    break;
            }

            rt.sizeDelta = size;
        }

        private KeyHighlight _highlight;

        public void Down()
        {
            highlightImage.color = activeLineColor;
            labelText.color = activeTextColor;
            countText.color = activeCountTextColor;
            
            if (_highlight)
            {
                return;
            }

            if (!HighlightPool.TryDequeue(out var k))
            {
                k = Instantiate(highlightTemplate, highlightTemplate.transform.parent);
            }

            _highlight = k;

            k.direction = position;
            k.color = rainColor;
            k.gameObject.SetActive(true);
        }

        public void Up()
        {
            highlightImage.color = inactiveLineColor;
            labelText.color = inactiveTextColor;
            countText.color = inactiveCountTextColor;

            if (_highlight) _highlight.ended = true;

            _highlight = null;
        }

        private void Update()
        {
            if (controlled) return;

            if (Input.GetKeyDown(Code))
            {
                countText.text = $"{++elem.count}";
                Down();
            }
            else if (Input.GetKeyUp(Code))
            {
                Up();
            }
        }
    }
}