using RainingKeys.Utils;
using UnityEngine;

namespace RainingKeys {
    public static class Util {
        public static Color ColorRgbaSliders(Color color)
        {
            float oldR = Mathf.Round(color.r * 255);
            float oldG = Mathf.Round(color.g * 255);
            float oldB = Mathf.Round(color.b * 255);
            float oldA = Mathf.Round(color.a * 255);
            float newR = MoreGUILayout.NamedSlider("R:", oldR, 0, 255, 300f, 1, 40f);
            float newG = MoreGUILayout.NamedSlider("G:", oldG, 0, 255, 300f, 1, 40f);
            float newB = MoreGUILayout.NamedSlider("B:", oldB, 0, 255, 300f, 1, 40f);
            float newA = MoreGUILayout.NamedSlider("A:", oldA, 0, 255, 300f, 1, 40f);
            if (oldR != newR || oldG != newG || oldB != newB || oldA != newA)
            {
                return new Color(newR / 255, newG / 255, newB / 255, newA / 255);
            }

            return color;
        }
    }
}