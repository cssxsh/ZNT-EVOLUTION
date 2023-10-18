﻿using BepInEx;
using HarmonyLib;

namespace ZNT.Evolution.Core
{
    [BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.core", Name: "Evolution Core", Version: "0.2.4")]
    public class EvolutionCorePlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(DebugPatch));
            Harmony.CreateAndPatchAll(typeof(StartManagerPatch));
        }
    }
}