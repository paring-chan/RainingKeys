using System;
using UnityEngine;
using UnityEngine.UI;

namespace RainingKeys.Components {
    public class KeyHighlight : MonoBehaviour {
        public KeyCode key;
        public Color color;

        public Image image;

        public RectTransform rt;

        public ViewerPosition direction;

        public BoxCollider2D collider2d;

        private bool _ended;

        private void Start()
        {
            image.color = color;
        }

        private Vector2 GetSizeDelta(float toMove)
        {
            switch (direction)
            {
                case ViewerPosition.Bottom:
                    return new Vector2(0, toMove);
                case ViewerPosition.Left:
                    return new Vector2(toMove, 0);
                case ViewerPosition.Right:
                    return new Vector2(-toMove, 0);
                case ViewerPosition.Top:
                    return new Vector2(0, -toMove);
            }
            return Vector2.zero;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Destroy(gameObject);
        }

        private void Update()
        {
            var toMove = Time.unscaledDeltaTime * 400f;
            var sizeDelta = rt.sizeDelta;
            var delta = GetSizeDelta(toMove);
            if (Input.GetKey(key) && !_ended)
            {
                sizeDelta += new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
                collider2d.size = sizeDelta;
                rt.sizeDelta = sizeDelta;
                collider2d.offset -= delta / 2;
            }
            else
            {
                _ended = true;
            }

            rt.anchoredPosition += delta;
        }
    }
}