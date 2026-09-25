using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using ZNT.Evolution.Core.Asset;

namespace ZNT.Evolution.Core;

public static class CustomAssetExtensions
{
    extension(CustomAsset asset)
    {
        [UsedImplicitly]
        public void SetAssetId(string value) => Traverse.Create(asset).Field<string>("assetId").Value = value;
    }

    extension(AssetElement asset)
    {
        public bool Bind()
        {
            if (asset.AssetId is null or "") asset.SetAssetId(asset.name);
            lock (AssetElementIndex.IndexPath)
            {
                switch (asset)
                {
                    case LevelElement element:
                        if (LevelElementIndex.Index.Elements.ContainsKey(element.AssetId)) return false;
                        LevelElementIndex.Index.AddAssetElement(element);
                        return true;
                    case FMODAsset fmod:
                        if (FmodAssetIndex.Index.Elements.ContainsKey(fmod.AssetId)) return false;
                        FmodAssetIndex.Index.AddAssetElement(fmod);
                        FmodAssetIndex.PathIndex.TryAdd(fmod.path, fmod);
                        return true;
                    case VisualEffect effect:
                        if (VisualEffectIndex.Index.Elements.ContainsKey(effect.AssetId)) return false;
                        VisualEffectIndex.Index.AddAssetElement(effect);
                        return true;
                    case ShaderAnimator animator:
                        if (ShaderAnimatorIndex.Index.Elements.ContainsKey(animator.AssetId)) return false;
                        ShaderAnimatorIndex.Index.AddAssetElement(animator);
                        return true;
                    case VoiceAsset voice:
                        return voice.index is >= 0 and <= byte.MaxValue &&
                               VoiceAsset.Elements.TryAdd(voice.index, voice);
                    default:
                        throw new System.NotSupportedException($"Bind: {asset}");
                }
            }
        }

        public void Unbind()
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
                    case VoiceAsset voice:
                        VoiceAsset.Elements.Remove(voice.index);
                        break;
                    default:
                        throw new System.NotSupportedException($"Unbind: {asset}");
                }
            }
        }
    }

    extension(FMODAsset)
    {
        public static Dictionary<string, FMODAsset> FetchFMODAsset(string path)
        {
            var result = FMODUnity.RuntimeManager.StudioSystem.getBank(path, out var bank);
            if (result is not FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
            result = bank.getEventList(out var events);
            if (result is not FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
            var dictionary = new Dictionary<string, FMODAsset>(events.Length);
            foreach (var description in events)
            {
                result = description.getID(out var guid);
                if (result is not FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
                result = description.getPath(out var key);
                if (result is not FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
                if (FmodAssetIndex.PathIndex.ContainsKey(key)) continue;
                var asset = ScriptableObject.CreateInstance<FMODAsset>();
                Object.DontDestroyOnLoad(asset);
                asset.id = $"{{{guid}}}";
                asset.path = key;
                asset.name = key.Substring(key.LastIndexOf('/') + 1);
                asset.SetAssetId($"{path} - {key}");
                _ = asset.Bind();
                dictionary.Add(asset.path, asset);
            }

            return dictionary;
        }

        public static void ClearFMODAsset(string path)
        {
            var result = FMODUnity.RuntimeManager.StudioSystem.getBank(path, out var bank);
            if (result is not FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
            result = bank.getEventList(out var events);
            if (result is not FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
            foreach (var description in events)
            {
                result = description.getPath(out var key);
                if (result is not FMOD.RESULT.OK) throw new FMODUnity.BankLoadException(path, result);
                if (FmodAssetIndex.PathIndex.TryGetValue(key, out var asset)) asset.Unbind();
            }
        }
    }

    extension(ExplosionAsset explosion)
    {
        [UsedImplicitly]
        public bool AutoExplode
        {
            get => Traverse.Create(explosion).Field<bool>("autoExplode").Value;
            set => Traverse.Create(explosion).Field<bool>("autoExplode").Value = value;
        }
    }

    extension(MovingObjectAsset moving)
    {
        [UsedImplicitly]
        public tk2dSpriteAnimation Animation
        {
            get => Traverse.Create(moving).Field<tk2dSpriteAnimation>("library").Value;
            set => Traverse.Create(moving).Field<tk2dSpriteAnimation>("library").Value = value;
        }
    }

    extension(PhysicObjectAsset physic)
    {
        [UsedImplicitly]
        public tk2dSpriteAnimation Animation
        {
            get => Traverse.Create(physic).Field<tk2dSpriteAnimation>("library").Value;
            set => Traverse.Create(physic).Field<tk2dSpriteAnimation>("library").Value = value;
        }
    }

    extension(TMPro.TMP_Settings)
    {
        [UsedImplicitly]
        public static void SetDefaultSpriteAsset(TMPro.TMP_SpriteAsset value)
        {
            Traverse.Create(TMPro.TMP_Settings.instance)
                .Field<TMPro.TMP_SpriteAsset>("m_defaultSpriteAsset").Value = value;
        }
    }

    extension(TMPro.MaterialReferenceManager)
    {
        [UsedImplicitly]
        public static void RemoveFontAsset(TMPro.TMP_FontAsset fontAsset)
        {
            Traverse.Create(TMPro.MaterialReferenceManager.instance)
                .Field<Dictionary<int, TMPro.TMP_FontAsset>>("m_FontAssetReferenceLookup").Value
                .Remove(fontAsset.hashCode);
            Traverse.Create(TMPro.MaterialReferenceManager.instance)
                .Field<Dictionary<int, Material>>("m_FontMaterialReferenceLookup").Value
                .Remove(fontAsset.materialHashCode);
        }

        [UsedImplicitly]
        public static void RemoveSpriteAsset(TMPro.TMP_SpriteAsset spriteAsset)
        {
            Traverse.Create(TMPro.MaterialReferenceManager.instance)
                .Field<Dictionary<int, TMPro.TMP_SpriteAsset>>("m_SpriteAssetReferenceLookup").Value
                .Remove(spriteAsset.hashCode);
            Traverse.Create(TMPro.MaterialReferenceManager.instance)
                .Field<Dictionary<int, Material>>("m_FontMaterialReferenceLookup").Value
                .Remove(spriteAsset.hashCode);
        }
    }

    extension(TMPro.TMP_Asset asset)
    {
        public bool Bind()
        {
            if (asset.hashCode is 0) asset.hashCode = TMPro.TMP_TextUtilities.GetSimpleHashCode(asset.name);
            lock (TMPro.MaterialReferenceManager.instance)
            {
                switch (asset)
                {
                    case TMPro.TMP_FontAsset font:
                        if (TMPro.MaterialReferenceManager.instance.Contains(font)) return false;
                        TMPro.MaterialReferenceManager.AddFontAsset(font);
                        TMPro.TMP_Settings.fallbackFontAssets.RemoveAll(f => f is null);
                        TMPro.TMP_Settings.fallbackFontAssets.Add(font);
                        return true;
                    case TMPro.TMP_SpriteAsset emoji:
                        if (TMPro.MaterialReferenceManager.instance.Contains(emoji)) return false;
                        TMPro.MaterialReferenceManager.AddSpriteAsset(emoji);
                        if (emoji.hashCode is 160120832) TMPro.TMP_Settings.SetDefaultSpriteAsset(emoji);
                        return true;
                    default:
                        throw new System.NotSupportedException($"Bind: {asset}");
                }
            }
        }

        public void Unbind()
        {
            lock (TMPro.MaterialReferenceManager.instance)
            {
                switch (asset)
                {
                    case TMPro.TMP_FontAsset font:
                        TMPro.MaterialReferenceManager.RemoveFontAsset(font);
                        TMPro.TMP_Settings.fallbackFontAssets.Remove(font);
                        break;
                    case TMPro.TMP_SpriteAsset emoji:
                        TMPro.MaterialReferenceManager.RemoveSpriteAsset(emoji);
                        if (TMPro.TMP_Settings.defaultSpriteAsset != emoji) break;
                        TMPro.TMP_Settings.SetDefaultSpriteAsset(null);
                        break;
                }
            }
        }
    }
}