using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
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
                asset.id = "{" + guid + "}";
                description.getPath(out asset.path);
                Traverse.Create(asset).Field("assetId").SetValue(guid.ToString());

                lock (typeof(FmodAssetIndex))
                {
                    FmodAssetIndex.Index.AddAssetElement(asset);
                    Traverse.Create(typeof(FmodAssetIndex)).Field("pathIndex").SetValue(null);
                }

                dictionary.TryAdd(asset.path, asset);
            }

            return dictionary;
        }

        public static string Bind(this AssetElement asset)
        {
            Traverse.Create(asset).Field("assetId").SetValue(asset.name);
            switch (asset)
            {
                case LevelElement element:
                    lock (typeof(LevelElementIndex))
                    {
                        LevelElementIndex.Index.AddAssetElement(element);
                    }

                    break;
                default:
                    throw new NotSupportedException($"Bind: {asset.GetType()}");
            }

            return asset.AssetId;
        }

        public static void Unbind(this AssetElement asset)
        {
            switch (asset)
            {
                case LevelElement element:
                    lock (typeof(LevelElementIndex))
                    {
                        LevelElementIndex.Index.RemoveAssetElement(element);
                    }

                    break;
                default:
                    throw new NotSupportedException($"Unbind: {asset.GetType()}");
            }
        }

        public static IEnumerable<LevelElement> LevelElements(bool isMod = true)
        {
            return LevelElementIndex.Index.Values
                .Select(element => (LevelElement)element)
                .Where(element => !isMod || element.name == element.AssetId);
        }
    }
}