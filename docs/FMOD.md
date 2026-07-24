# 如何在 BepInEx 插件中向使用 FMOD 的 Unity 游戏中导入新的音频

> 目标游戏 https://store.steampowered.com/app/416680/Zombie_Night_Terror/

## 确定 FMOD 版本

在 `znt_Data\Plugins\` 下找到 `fmodstudio.dll`。  
右键属性详 -> 详细信息 -> 产品版本  
可知版本为 `1.10.13`  
到 <https://www.fmod.com/download> 下载对应版本 FMOD Studio

## 关于 Bank 文件

Bank 文件是 FMOD 的音频打包文件。  
目标游戏 MOD 是通过直接替换其内容来实现效果的，但这会丢失原版的音频。  
本文研究的是通过 FMOD 的 API 加载额外的 Bank 文件。  

首先，目标游戏将音频打包为

```text
Master Bank.bank
Master Bank.strings.bank
AmbBank.bank
DialogBank.bank
IntroBank.bank
Musicbank.bank
```

这样的形式，其中：  
* `Master Bank.bank` 是主包，会打包 `Event` `Bus` 等内容  
* `Master Bank.strings.bank` 是索引包，将 `guid` 和 `path` 关联起来  
* `AmbBank.bank`, `DialogBank.bank`, `IntroBank.bank`, `Musicbank.bank` 是从包，只会打包 `Event` 内容

对于一个游戏而言，FMOD 模块只能有一个主包，因为主包里必然有 `bus:/` 这个 `Root Bus` 。  
而所有 FMOD 资源对象的 `path` 和 `guid` 都是唯一的。  
再加载一个主包会因为 `bus:/` 冲突而报错，所以新的音频只能在新从包中加载。  
对于 `Root Bus` , 你可以通过以下代码获取 GUID，之后会用到。  
PS: 实际上主包 `Master Bank` 可以改成其他名字，但主包一定会有对应的 strings 索引

```csharp
_ = FMODUnity.RuntimeManager.StudioSystem.getBus("bus:/", out var root);
_ = root.getID(out var id);
```

得到的 GUID 为 `d09bf6f4-00a6-4bc5-95d3-9c5886ec31cd`

## 制作 Bank 文件

1. FMOD Studio 新建项目
2. 修改主包 `Master Bank` 的名字，要和游戏自带的包不一样
3. 新建从包，也要和游戏自带的包不一样
4. 将音频导入项目，制作 `Event`, 放入从包, 注意**只能放入从包**, 原因上文说过
5. 保存关闭项目
6. 打开项目下的 `Metadata/Master.xml` 文件，获取 `Master Bus` 的 GUID, `<object class="MixerMaster" id="{...}">`   
7. 用上文中提到的 `Root Bus` 的 GUID 将项目中所有 xml 文件中的 `Master Bus` 的 GUID 替换掉
8. F7 打包文件

PS: 第 6 步 和 第 7 步 一定要完成，不然项目打包出来的 `Bank` 文件 的 `Root Bus` 的 GUID 对不上，会导致加载后新音频没有输出

## 加载 Bank 文件

有两种方式：
* 从 `znt_Data/StreamingAssets` 下文件中加载 `FMODUnity.RuntimeManager.LoadBank(bankName: ..., loadSamples: true)`
* 从 `TextAsset` 中加载 `FMODUnity.RuntimeManager.LoadBank(asset: ..., loadSamples: true)`

将制作好的 索引包 和 从包 加载，注意不要加载主包。  
加载之后如果游戏有资源索引，还需要手动添加索引。  

以下是 `ZNT` 添加索引的代码示例，仅供参考，`path` 的内容是 `bank:/{bank_name}`
```csharp
public static Dictionary<string, FMODAsset> FetchFMODAsset(string path)
{
    var result = RuntimeManager.StudioSystem.getBank(path, out var bank);
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
        dictionary.Add(asset.path, asset);
        FmodAssetIndex.Index.AddAssetElement(asset);
        FmodAssetIndex.PathIndex.TryAdd(asset.path, asset);
    }

    return dictionary;
}
```

## 参考文档

- https://fmod.com/docs_cn/2.02/studio/supporting-downloadable-and-user-generated-content.html