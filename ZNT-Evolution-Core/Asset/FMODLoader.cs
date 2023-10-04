using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public static class FMODLoader
    {
        /// <summary>
        /// 同步 Bank 中的 Event 到 FmodAssetIndex 中
        /// </summary>
        /// <param name="path"> Bank 的 path, 例如 <c>bank:/Arknights 311 Mudrok</c> </param>
        /// <returns> 同步的内容 </returns>
        public static Dictionary<string, FMODAsset> FetchFMODAsset(string path)
        {
            FMODUnity.RuntimeManager.StudioSystem.getBank(path, out var bank);
            bank.getEventList(out var events);
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
    }
}