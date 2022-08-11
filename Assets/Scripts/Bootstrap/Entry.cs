using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace RainingKeys.Bootstrap
{
    internal static class Entry
    {
        internal static MethodBase OnStartInputs;
        internal static MethodBase OnEndInputs;
        internal static MethodBase OnKeyPress;
        internal static MethodBase OnKeyRelease;

        private static void Load(UnityModManager.ModEntry entry)
        {
            LoadAssembly(Path.Combine(entry.Path, "NewtonSoft.Json.dll"));
            var asm = LoadAssembly(Path.Combine(entry.Path, "RainingKeys.dll"));

            var impl = asm.GetType("RainingKeys.ReplayInputImpl");

            OnStartInputs = AccessTools.Method(impl, "OnStartInputs");
            OnEndInputs = AccessTools.Method(impl, "OnEndInputs");
            OnKeyPress = AccessTools.Method(impl, "OnKeyPressed");
            OnKeyRelease = AccessTools.Method(impl, "OnKeyReleased");
            
            asm.GetType("RainingKeys.Startup")?.GetMethod("Load", AccessTools.all)
                ?.Invoke(null, new object[] { entry });
        }

        private static Assembly LoadAssembly(string path)
        {
            return AppDomain.CurrentDomain.Load(File.ReadAllBytes(path));
        }
    }

    public static class ReplayInput
    {
        public static void OnStartInputs()
        {
            Entry.OnStartInputs.Invoke(null, new object[] { });
        }

        public static void OnEndInputs()
        {
            Entry.OnEndInputs.Invoke(null, new object[] { });
        }

        public static void OnKeyPressed(KeyCode keyCode)
        {
            Entry.OnKeyPress.Invoke(null, new object[] { keyCode });
        }

        public static void OnKeyReleased(KeyCode keyCode)
        {
            Entry.OnKeyRelease.Invoke(null, new object[] { keyCode });
        }
    }
}