using System;
using System.Collections.Generic;
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
        var result = FMODUnity.RuntimeManager.StudioSystem.getBank(path, out var bank);
        if (result != FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
        result = bank.getEventList(out var events);
        if (result != FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
        var dictionary = new Dictionary<string, FMODAsset>(events.Length);
        foreach (var description in events)
        {
            var asset = ScriptableObject.CreateInstance<FMODAsset>();
            UnityEngine.Object.DontDestroyOnLoad(asset);
            result = description.getID(out var guid);
            if (result != FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
            asset.id = $"{{{guid}}}";
            result = description.getPath(out asset.path);
            if (result != FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
            asset.name = asset.path.Substring(asset.path.LastIndexOf('/') + 1);
            Traverse.Create(asset).Field<string>("assetId").Value = $"{path} - {asset.path}";
            _ = asset.Bind();
            dictionary.Add(asset.path, asset);
        }

        return dictionary;
    }

    /// <summary>
    /// 清理 Bank 中的 Event 从 FmodAssetIndex 中
    /// </summary>
    /// <param name="path"> Bank 的 path, 例如 <c>bank:/Gunner</c> </param>
    public static void ClearFMODAsset(string path)
    {
        var result = FMODUnity.RuntimeManager.StudioSystem.getBank(path, out var bank);
        if (result != FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
        result = bank.getEventList(out var events);
        if (result != FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
        foreach (var description in events)
        {
            result = description.getPath(out var key);
            if (result != FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
            if (FmodAssetIndex.PathIndex.TryGetValue(key, out var asset)) asset.Unbind();
        }
    }

    public static string Bind(this AssetElement asset)
    {
        if (asset.AssetId is null or "") Traverse.Create(asset).Field<string>("assetId").Value = asset.name;
        lock (AssetElementIndex.IndexPath)
        {
            switch (asset)
            {
                case LevelElement element:
                    LevelElementIndex.Index.AddAssetElement(element);
                    break;
                case FMODAsset fmod:
                    FmodAssetIndex.Index.AddAssetElement(fmod);
                    FmodAssetIndex.PathIndex.TryAdd(fmod.path, fmod);
                    break;
                case VisualEffect effect:
                    VisualEffectIndex.Index.AddAssetElement(effect);
                    break;
                case ShaderAnimator animator:
                    ShaderAnimatorIndex.Index.AddAssetElement(animator);
                    break;
                default:
                    throw new NotSupportedException($"Bind: {asset}");
            }
        }

        return asset.AssetId;
    }

    public static int Bind(this TMPro.TMP_Asset asset)
    {
        if (asset.hashCode == 0) asset.hashCode = TMPro.TMP_TextUtilities.GetSimpleHashCode(asset.name);
        lock (TMPro.MaterialReferenceManager.instance)
        {
            switch (asset)
            {
                case TMPro.TMP_FontAsset font:
                    TMPro.MaterialReferenceManager.AddFontAsset(font);
                    TMPro.TMP_Settings.fallbackFontAssets.RemoveAll(f => f is null);
                    TMPro.TMP_Settings.fallbackFontAssets.Add(font);
                    break;
                case TMPro.TMP_SpriteAsset emoji:
                    TMPro.MaterialReferenceManager.AddSpriteAsset(emoji);
                    if (emoji.hashCode is not 160120832) break;
                    Traverse.Create(TMPro.TMP_Settings.instance)
                        .Field<TMPro.TMP_SpriteAsset>("m_defaultSpriteAsset").Value = emoji;
                    break;
            }
        }

        return asset.hashCode;
    }

    public static void Unbind(this AssetElement asset)
    {
        lock (AssetElementIndex.IndexPath)
        {
            switch (asset)
            {
                case LevelElement element:
                    LevelElementIndex.Index.RemoveAssetElement(element);
                    break;
                case FMODAsset fmod:
                    FmodAssetIndex.Index.RemoveAssetElement(fmod);
                    FmodAssetIndex.PathIndex.Remove(fmod.path);
                    break;
                case VisualEffect effect:
                    VisualEffectIndex.Index.RemoveAssetElement(effect);
                    break;
                case ShaderAnimator animator:
                    ShaderAnimatorIndex.Index.RemoveAssetElement(animator);
                    break;
                default:
                    throw new NotSupportedException($"Unbind: {asset}");
            }
        }
    }

    public static void Unbind(this TMPro.TMP_Asset asset)
    {
        lock (TMPro.MaterialReferenceManager.instance)
        {
            switch (asset)
            {
                case TMPro.TMP_FontAsset font:
                    Traverse.Create(TMPro.MaterialReferenceManager.instance)
                        .Field<Dictionary<int, TMPro.TMP_FontAsset>>("m_FontAssetReferenceLookup")
                        .Value.Remove(font.hashCode);
                    TMPro.TMP_Settings.fallbackFontAssets.Remove(font);
                    break;
                case TMPro.TMP_SpriteAsset emoji:
                    Traverse.Create(TMPro.MaterialReferenceManager.instance)
                        .Field<Dictionary<int, TMPro.TMP_SpriteAsset>>("m_SpriteAssetReferenceLookup")
                        .Value.Remove(emoji.hashCode);
                    if (TMPro.TMP_Settings.defaultSpriteAsset != emoji) break;
                    Traverse.Create(TMPro.TMP_Settings.instance)
                        .Field<TMPro.TMP_SpriteAsset>("m_defaultSpriteAsset").Value = null;
                    break;
            }
        }
    }
}