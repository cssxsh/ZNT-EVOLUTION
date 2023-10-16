using System;
using System.IO;
using System.Linq;
using System.Threading;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;

namespace ZNT.Evolution.Core
{
    [BepInPlugin(GUID: "xyz.cssxsh.znt.evolution.core", Name: "Evolution Core", Version: "0.1.1")]
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
                    var loaded = new []
                    {
                        "Master Bank.strings",
                        "Master Bank",
                        "AmbBank",
                        "DialogBank",
                        "IntroBank",
                        "Musicbank"
                    };
                    foreach (var file in Directory.EnumerateFiles(Application.streamingAssetsPath, "*.bank"))
                    {
                        var bank = Path.GetFileNameWithoutExtension(file);
                        if (loaded.Contains(bank)) continue;
                        if (!bank.EndsWith(".strings")) continue;
                        try
                        {
                            FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: true);
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning(e);
                        }
                    }
                    foreach (var file in Directory.EnumerateFiles(Application.streamingAssetsPath, "*.bank"))
                    {
                        var bank = Path.GetFileNameWithoutExtension(file);
                        if (loaded.Contains(bank)) continue;
                        if (bank.EndsWith(".strings")) continue;
                        try
                        {
                            FMODUnity.RuntimeManager.LoadBank(bankName: bank, loadSamples: true);
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning(e);
                            continue;
                        }
                        var path = $"bank:/{bank}";
                        Logger.LogDebug($"load: {path}");
                        foreach (var (_, asset) in AssetElementBinder.FetchFMODAsset(path: path))
                        {
                            Logger.LogDebug($"load: {asset.path}");
                        }
                    }
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