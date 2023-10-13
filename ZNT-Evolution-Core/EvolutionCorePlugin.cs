using System;
using System.IO;
using System.Threading;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core
{
    [BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.core", Name: "Evolution Core", Version: "0.1.1")]
    public class EvolutionCorePlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(DebugPatch));
            Harmony.CreateAndPatchAll(typeof(StartManagerPatch));

            foreach (var type in (LevelElement.Type[])Enum.GetValues(typeof(LevelElement.Type)))
            {
                var path = Path.Combine(Application.dataPath, type.ToString());

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                foreach (var directory in Directory.EnumerateDirectories(path))
                {
                    var target = Path.GetFullPath(directory);
                    var thread = new Thread(() =>
                    {
                        try
                        {
                            foreach (var (_, element) in LevelElementLoader.LoadFormFolder(path: target, type: type))
                            {
                                Logger.LogInfo($"LevelElement {element.name} Loaded");
                            }
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
}