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

        public KeyElement elem;

        public KeyHighlight highlightTemplate;

        public Image highlightImage;

        public Text labelText;

        public Color inactiveCountTextColor;

        public Color activeCountTextColor;

        public Text countText;

        public ViewerPosition position;

        private KeyCode Code => elem.key;

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
#else
            countText.font = RDString.GetFontDataForLanguage(RDString.language).font;
            labelText.font = RDString.GetFontDataForLanguage(RDString.language).font;
#endif
        }

        private void Start()
        {
            if (!KeyToString.TryGetValue(Code, out string code))
            {
                code = Code.ToString();
            }

            labelText.text = code;

            countText.text = $"{elem.count}";
        }

        private void Update()
        {
            var pressed = Input.GetKey(Code);
            highlightImage.color = pressed ? activeLineColor : inactiveLineColor;
            labelText.color = pressed ? activeTextColor : inactiveTextColor;
            countText.color = pressed ? activeCountTextColor : inactiveCountTextColor;

            if (Input.GetKeyDown(Code))
            {
                countText.text = $"{++elem.count}";
                var k = Instantiate(highlightTemplate, highlightTemplate.transform.parent);
                k.direction = position;
                k.key = Code;
                k.color = rainColor;
                k.gameObject.SetActive(true);
            }
        }
    }
}