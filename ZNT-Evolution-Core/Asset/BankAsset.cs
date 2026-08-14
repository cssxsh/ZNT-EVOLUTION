using System;
using BepInEx.Logging;
using UnityEngine;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Asset;

public class BankAsset : TextAsset
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(BankAsset));

    public byte[] data;

    public string Path => $"bank:/{name}";

    public void Load()
    {
        if (data is null) throw new NullReferenceException();
        FMODUnity.RuntimeManager.LoadBank(this);
        FMODUnity.RuntimeManager.WaitForAllLoads();
        foreach (var (_, asset) in AssetElementBinder.FetchFMODAsset(Path))
        {
            Logger.LogDebug($"Bind FMODAsset {asset.path} from {Path}");
        }
    }

    public void UnLoad()
    {
        AssetElementBinder.ClearFMODAsset(Path);
        FMODUnity.RuntimeManager.UnloadBank(name);
    }
}