using System.Collections.Generic;
using System.Threading;
using HarmonyLib;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    public static class StartManagerPatch
    {
        internal static readonly Dictionary<string, Thread> Loading = new Dictionary<string, Thread>();

        internal static int JoinTimeout = -1;

        [HarmonyPatch(typeof(StartManager), "Start"), HarmonyPostfix]
        public static void Start(StartManager __instance)
        {
            foreach (var (_, thread) in Loading) thread.Start();
        }
        
        [HarmonyPatch(typeof(StartManager), "LoadNextScene"), HarmonyPrefix]
        public static void LoadNextScene(StartManager __instance)
        {
            foreach (var (_, thread) in Loading) thread.Join(JoinTimeout);
        }
    }
}