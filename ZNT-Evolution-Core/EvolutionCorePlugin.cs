using System;
using System.IO;
using System.Threading;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core
{
    [BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.core", Name: "Evolution Core", Version: "0.2.4")]
    public class EvolutionCorePlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(DebugPatch));
            Harmony.CreateAndPatchAll(typeof(StartManagerPatch));

            StartManagerPatch.Loading["fmod"] = new Thread(() =>
            {
                try
                {
                    LevelElementLoader.LoadBanks(folder: Application.streamingAssetsPath);
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e);
                }
            });

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
                                Logger.LogInfo($"LevelElement {element.name} - {element.Title} Loaded");
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

        public void Start()
        {
            var timeout = Config.Bind(
                section: "config",
                key: "join_timeout",
                defaultValue: -1,
                description: "外部资源加载等待时间, 单位毫秒"
            );
            StartManagerPatch.JoinTimeout = timeout.Value;
        }
    }
}