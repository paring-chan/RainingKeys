using System;
using System.Collections.Generic;
using UnityEngine;

namespace RainingKeys.Components {
    public class KeyContainer : MonoBehaviour {
        public Key template;
        
        public readonly Dictionary<KeyCode, Key> Keys = new();
        
        public Color inactiveTextColor;
        public Color activeTextColor;
        public Color inactiveLineColor;
        public Color activeLineColor;
        public Color rainColor;
        public Color inactiveCountTextColor;
        public Color activeCountTextColor;

        public Vector2 position;

        public float size;
        
        public ViewerPosition viewerPosition;

        private void Start()
        {
            var rt = GetComponent<RectTransform>();
            
            rt.anchorMin = position;
            rt.anchorMax = position;
            rt.pivot = position;
            rt.anchoredPosition = Vector2.zero;

            rt.localScale = new Vector3(1, 1, 1) * (size / 100f);
        }

        public void Down(KeyCode key)
        {
            if (Keys.TryGetValue(key, out var el))
            {
                el.Down();
            }
        }
        
        public void Up(KeyCode key)
        {
            if (Keys.TryGetValue(key, out var el))
            {
                el.Up();
            }
        }

        public void Clear()
        {
            foreach (var (_, value) in Keys)
            {
                Destroy(value);
            }

            Keys.Clear();
        }

        public Key AddKey(KeyElement key)
        {
            var c = Instantiate(template, transform);
            
            c.inactiveTextColor = inactiveTextColor;
            c.activeTextColor = activeTextColor;
            c.inactiveLineColor = inactiveLineColor;
            c.activeLineColor = activeLineColor;
            c.inactiveCountTextColor = inactiveCountTextColor;
            c.activeCountTextColor = activeCountTextColor;
            c.rainColor = rainColor;
            c.elem = key;
            c.position = viewerPosition;
            c.gameObject.SetActive(true);
            Keys.Add(key.key, c);
            c.position = viewerPosition;

            return c;
        }
    }
}