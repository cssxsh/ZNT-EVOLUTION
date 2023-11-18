# UnityEngine.AssetBundle

> UnityEngine 序列化打包和依赖管理

## Assets

> 游戏对象，数据，图像

`Name` 唯一, 不能冲突  
在 Asset 中可以通过 `File ID` 和 `Path ID` 引用另一个 Asset  
当前 `Assets` 的 `File ID` 为 `0`

### MonoBehaviour

> 脚本对象

需要加载前置 Bundle (`znt_Data/data.unity3d`, `znt_Data/Resources/unity default resources`)  
然后使用 `Tools -> Get script information` 读取 `znt_Data/Managed` 下的 DLL

## Dependencies

> Asset 依赖

定义当前 Bundle 语境下 `File ID` 指向的 External Assets (通过 `Name`)