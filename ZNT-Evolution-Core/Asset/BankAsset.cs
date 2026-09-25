using System;
using BepInEx.Logging;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Asset;

public class BankAsset : TextAsset
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(BankAsset));

    public byte[] data;

    public string path => $"bank:/{name}";

    public void Load()
    {
        if (data is null) throw new NullReferenceException();
        FMODUnity.RuntimeManager.LoadBank(this);
        FMODUnity.RuntimeManager.WaitForAllLoads();
        foreach (var (_, asset) in FMODAsset.FetchFMODAsset(path))
        {
            Logger.LogDebug($"Bind FMODAsset {asset.path} from {path}");
        }
    }

    public void UnLoad()
    {
        FMODAsset.ClearFMODAsset(path);
        FMODUnity.RuntimeManager.UnloadBank(name);
    }
}