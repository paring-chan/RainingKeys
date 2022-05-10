using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;

namespace RainingKeys.Bootstrap {
    internal static class Entry {
        private static void Load(UnityModManager.ModEntry entry)
        {
            LoadAssembly(Path.Combine(entry.Path, "NewtonSoft.Json.dll"));
            var asm = LoadAssembly(Path.Combine(entry.Path, "RainingKeys.dll"));
            asm.GetType("RainingKeys.Startup")?.GetMethod("Load", AccessTools.all)?.Invoke(null, new object[] {entry});
        }

        private static Assembly LoadAssembly(string path)
        {
            using FileStream stream = new FileStream(path, FileMode.Open);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return AppDomain.CurrentDomain.Load(data);
        }
    }
}