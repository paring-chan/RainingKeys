using System.Collections.Generic;
using UnityEngine;

namespace RainingKeys
{
    public static class ReplayInput
    {
        public static void OnStartInputs()
        {
            var container = Startup.Container;

            if (!container) return;
            
            container.Clear();

            foreach (var key in Startup.Config.keys)
            {
                container.AddKey(new KeyElement
                {
                    count = 0,
                    key = key.key
                });
            } 
        }

        public static void OnEndInputs()
        {
            var container = Startup.Container;

            if (!container) return;
            
            container.Clear();
            
            foreach (var key in Startup.Config.keys)
            {
                container.AddKey(key);
            } 
        }

        public static void OnKeyPressed(KeyCode keyCode)
        {
            var c = Startup.Container;
            if (c) c.Down(keyCode);
        }

        public static void OnKeyReleased(KeyCode keyCode)
        {
            var c = Startup.Container;
            if (c) c.Up(keyCode);
        }
    }
}