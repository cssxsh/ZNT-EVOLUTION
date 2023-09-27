using System;
using BepInEx;
using HarmonyLib;

namespace Arknights_ZNT
{
    [BepInPlugin("xyz.cssxsh.znt.arknights-plugin", "Arknights Plugin", "0.1.0")]
    public class ArknightsPlugin : BaseUnityPlugin
    {
        public void Start()
        {
            Logger.LogInfo("Hello World!");
            new Harmony(Info.Metadata.GUID).PatchAll();
        }
    }
}