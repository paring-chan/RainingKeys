using Newtonsoft.Json;
using UnityEngine;

namespace RainingKeys {
    [System.Serializable]public class SerializableColor
    {
        public float r;
        public float g;
        public float b;
        public float a;
 
        [JsonIgnore]
        public Color Color
        {
            get => new(r, g, b, a);
            set
            {
                r = value.r;
                g = value.g;
                b = value.b;
                a = value.a;
            }
        }
 
        public SerializableColor()
        {
            r = 1f;
            g = 1f;
            b = 1f;
            a = 1f;
        }
 
        public SerializableColor(float r, float g, float b, float a = 255f)
        {
            this.r = r / 255f;
            this.g = g / 255f;
            this.b = b / 255f;
            this.a = a / 255f;
        }
 
        public SerializableColor(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }
    }

}