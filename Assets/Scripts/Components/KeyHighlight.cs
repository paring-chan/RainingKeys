using System;
using UnityEngine;
using UnityEngine.UI;

namespace RainingKeys.Components
{
    public class KeyHighlight : MonoBehaviour
    {
        public Color color;
        public Key keyComponent;

        public Image image;

        public RectTransform rt;

        public ViewerPosition direction;

        public bool ended;

        private Vector2 _initialPosition;
        private Vector2 _initialSize;

        private void Awake()
        {
            _initialPosition = rt.anchoredPosition;
            _initialSize = rt.sizeDelta;
        }

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

        private bool ShouldDestroy()
        {
            switch (direction)
            {
                case ViewerPosition.Top:
                    return -rt.anchoredPosition.y - rt.sizeDelta.y > Values.RainTrackSize;
                case ViewerPosition.Bottom:
                    return rt.anchoredPosition.y - rt.sizeDelta.y > Values.RainTrackSize;
                case ViewerPosition.Left:
                    return rt.anchoredPosition.x - rt.sizeDelta.x > Values.RainTrackSize;
                case ViewerPosition.Right:
                    return -rt.anchoredPosition.x - rt.sizeDelta.x > Values.RainTrackSize;
            }

            return false;
        }

        private void Update()
        {
            if (ShouldDestroy())
            {
                rt.sizeDelta = _initialSize;
                rt.anchoredPosition = _initialPosition;
                ended = false;
                gameObject.SetActive(false);

                keyComponent.HighlightPool.Enqueue(this);
                return;
            }

            var toMove = Time.unscaledDeltaTime * Values.RainSpeed;
            var sizeDelta = rt.sizeDelta;
            var delta = GetSizeDelta(toMove);
            if (!ended)
            {
                var vec = new Vector2(0, 0);

                if (delta.x != 0)
                {
                    vec.x += Mathf.Abs(delta.x);
                }
                else if (delta.y != 0)
                {
                    vec.y += Mathf.Abs(delta.y);
                }

                sizeDelta += vec;
                rt.sizeDelta = sizeDelta;
            }

            rt.anchoredPosition += delta;
        }
    }
}