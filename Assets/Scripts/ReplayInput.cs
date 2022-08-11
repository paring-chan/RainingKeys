using HarmonyLib;
using RainingKeys.Bootstrap;
using UnityEngine;

namespace RainingKeys
{
    public static class ReplayInputImpl
    {
        public static void OnStartInputs()
        {
            var container = Startup.Container;

            if (!container) return;
            
            container.Clear();

            foreach (var key in Startup.Config.keys)
            {
                var k = container.AddKey(new KeyElement
                {
                    count = 0,
                    key = key.key
                });
                k.controlled = true;
            } 
        }

        public static bool OnEndInputs()
        {
            var container = Startup.Container;

            if (!container) return false;
            
            container.Clear();
            
            foreach (var key in Startup.Config.keys)
            {
                container.AddKey(key);
            }

            return false;
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