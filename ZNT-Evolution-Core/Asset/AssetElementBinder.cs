using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset;

public static class AssetElementBinder
{
    /// <summary>
    /// 同步 Bank 中的 Event 到 FmodAssetIndex 中
    /// </summary>
    /// <param name="path"> Bank 的 path, 例如 <c>bank:/Gunner</c> </param>
    /// <returns> 同步的内容 </returns>
    public static Dictionary<string, FMODAsset> FetchFMODAsset(string path)
    {
        var result = RuntimeManager.StudioSystem.getBank(path, out var bank);
        if (!bank.isValid()) throw new BankLoadException(path, result);
        result = bank.getEventList(out var events);
        if (events == null) throw new BankLoadException(path, result);
        var dictionary = new Dictionary<string, FMODAsset>(events.Length);
        foreach (var description in events)
        {
            var asset = ScriptableObject.CreateInstance<FMODAsset>();

            description.getID(out var guid);
            asset.id = $"{{{guid}}}";
            description.getPath(out asset.path);
            asset.name = asset.path.Split('/').Last();
            Traverse.Create(asset).Field<string>("assetId").Value = $"{path} - {asset.path}";
            asset.Bind();
            dictionary.TryAdd(asset.path, asset);
        }

        return dictionary;
    }

    public static string Bind(this AssetElement asset)
    {
        var assetId = Traverse.Create(asset).Field<string>("assetId");
        if (string.IsNullOrEmpty(assetId.Value)) assetId.Value = asset.name;
        switch (asset)
        {
            case LevelElement element:
                lock (LevelElementIndex.IndexName) LevelElementIndex.Index.AddAssetElement(element);
                break;
            case FMODAsset fmod:
                lock (FmodAssetIndex.IndexName) FmodAssetIndex.Index.AddAssetElement(fmod);
                FmodAssetIndex.PathIndex[fmod.path] = fmod;
                break;
            case VisualEffect effect:
                lock (VisualEffectIndex.IndexName) VisualEffectIndex.Index.AddAssetElement(effect);
                break;
            case ShaderAnimator animator:
                lock (ShaderAnimatorIndex.IndexName) ShaderAnimatorIndex.Index.AddAssetElement(animator);
                break;
            default:
                throw new NotSupportedException($"Bind: {asset}");
        }

        return asset.AssetId;
    }

    public static void Unbind(this AssetElement asset)
    {
        switch (asset)
        {
            case LevelElement element:
                lock (LevelElementIndex.IndexName) LevelElementIndex.Index.RemoveAssetElement(element);
                break;
            case FMODAsset fmod:
                lock (FmodAssetIndex.IndexName) FmodAssetIndex.Index.RemoveAssetElement(fmod);
                FmodAssetIndex.PathIndex.Remove(fmod.path);
                break;
            case VisualEffect effect:
                lock (VisualEffectIndex.IndexName) VisualEffectIndex.Index.RemoveAssetElement(effect);
                break;
            case ShaderAnimator animator:
                lock (ShaderAnimatorIndex.IndexName) ShaderAnimatorIndex.Index.RemoveAssetElement(animator);
                break;
            default:
                throw new NotSupportedException($"Unbind: {asset}");
        }
    }

    public static IEnumerable<LevelElement> BoundLevelElements()
    {
        return LevelElementIndex.Index.Values
            .Cast<LevelElement>()
            .Where(element => string.IsNullOrEmpty(element.AssetId) || element.AssetId.Length != 0x20);
    }
}