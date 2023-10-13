﻿using System.Collections.Generic;
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
        /// <param name="path"> Bank 的 path, 例如 <c>bank:/Arknights 311 Mudrok</c> </param>
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

                FmodAssetIndex.Index.AddAssetElement(asset);

                dictionary.TryAdd(asset.path, asset);
            }

            return dictionary;
        }

        public static string PushToIndex(AssetElement asset)
        {
            Traverse.Create(asset).Field("assetId").SetValue(asset.name);
            switch (asset)
            {
                case LevelElement element:
                    // TODO Traverse.Create(element).Field("tags").SetValue(...);
                    LevelElementIndex.Index.AddAssetElement(element);
                    break;
            }

            return asset.AssetId;
        }
    }
}