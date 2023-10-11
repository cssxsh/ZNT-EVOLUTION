using System;
using System.IO;
using System.Threading;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core
{
    [BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.core", Name: "Evolution Core", Version: "0.1.0")]
    public class EvolutionCorePlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(DebugPatch));
            Harmony.CreateAndPatchAll(typeof(StartManagerPatch));

            var path = Path.Combine(Application.dataPath, ".", "Elements");

            if (!Directory.Exists(path)) Directory.Exists(path);

            foreach (var directory in Directory.EnumerateDirectories(path))
            {
                var target = Path.GetFullPath(directory);
                var thread = new Thread(() => {
                    try
                    {
                        var element = LevelElementLoader.LoadFormFolder(path: target);
                        Logger.LogInfo($"LevelElement {element.name} Loaded");
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e);
                    }
                });
                StartManagerPatch.Loading[target] = thread;
            }
        }
    }
}