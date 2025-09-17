# 如何在 BepInEx 插件中向使用 TextMeshPro 的 Unity 游戏中导入新的字体

> 目标游戏 https://store.steampowered.com/app/416680/Zombie_Night_Terror/

## 确定 TextMeshPro 版本

* 1.4 `TMPro.TMP_FontAsset.version : string`
* 1.2.3 `TMPro.TMP_FontAsset.creationSettings : FontAssetCreationSettings`
* 1.2.2 `TMPro.TMP_FontAsset.fontCreationSettings : FontCreationSetting`

## 制作 TextMeshPro 字体文件

`Menu -> Window -> TextMeshPro -> Font Asset Creator`

## 使用 AssetBundle Browser 打包

1. 将 `TMP_SDF.shader` 和 `TMP_SDF-Mobile.shader` 分到不同的包变成依赖项
2. 通过 `AssetBundleExtractor` 将 `Dependencies` 的 `File Path` 修改为 `sharedassets0.assets`
3. 通过 `AssetBundleExtractor` 将 `TMP_FontAsset.m_AssemblyName` 修改为 `Assembly-CSharp.dll`
4. 通过 `AssetBundleExtractor` 将 `Material.m_Shader` 修正到 `sharedassets0.assets`
5. 通过 `AssetBundleExtractor` 将 `AssetBundle.m_PreloadTable` 修正到 `sharedassets0.assets`

## 从 AssetBundle 加载新的字体

```csharp
public static void FetchFMODAsset(AssetBundle assetBundle)
{
    var fonts = assetBundle.LoadAllAssets<TMPro.TMP_FontAsset>();
    TMPro.TMP_Settings.fallbackFontAssets.AddRange(fonts);
}
```