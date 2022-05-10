using UnityEngine;
using UnityEngine.UI;

namespace RainingKeys.Components {
    public class KeyHighlight : MonoBehaviour {
        public KeyCode key;
        public Color color;

        public Image image;

        public RectTransform rt;

        public ViewerPosition direction;

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

        private void Update()
        {
            var toMove = Time.unscaledDeltaTime * 400f;
            var sizeDelta = rt.sizeDelta;
            var delta = GetSizeDelta(toMove);
            if (Input.GetKey(key) && !_ended)
            {
                sizeDelta += delta;
                rt.sizeDelta = sizeDelta;
            }
            else
            {
                _ended = true;
            }

            rt.anchoredPosition += delta;

            if (direction == ViewerPosition.Top || direction == ViewerPosition.Bottom)
            {
                if (rt.anchoredPosition.y - sizeDelta.y > transform.parent.GetComponent<RectTransform>().rect.height)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if (rt.anchoredPosition.x - sizeDelta.x > transform.parent.GetComponent<RectTransform>().rect.width)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}