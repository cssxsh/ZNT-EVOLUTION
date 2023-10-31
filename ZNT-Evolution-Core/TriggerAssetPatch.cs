using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Editor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core
{
    public static class TriggerAssetPatch
    {
        [HarmonyPatch(typeof(TriggerAsset), methodName: "LoadFromAsset"), HarmonyPostfix]
        public static void LoadFromAsset(TriggerAsset __instance, GameObject gameObject)
        {
            if (gameObject.GetComponents<MineBehaviour>().Length != 0)
            {
                gameObject.AddComponent<MineTrapEditor>();
            }
        }
    }
}