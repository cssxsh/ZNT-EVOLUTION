﻿# ZNT-EVOLUTION

## 插件文件结构

```text
Zombie Night Terror
├───znt.exe
├───BepInEx
│   ├───plugins
│   │   ├───ZNT-Evolution-Core
|   |   |   └───ZNT-Evolution-Core.dll
│   │   └───<...>
│   └───<...>
├───znt_Data
│   ├───Brush
│   │   ├───arknights_311
│   │   └───<...>
│   ├───Decor
│   │   └───<...>
│   └───<...>
└───<...>
```

## 导入记录

### 角色导入流程

- [x] FMODAsset 声音资源
- [x] tk2dSpriteCollectionData 精灵图资源
- [ ] ~~VisualEffect~~
- [x] tk2dSpriteAnimation 精灵图动画
- [ ] ExplosionAsset 技能效果
- [x] PhysicObjectAsset 物理效果
- [x] CharacterAnimationAsset 动作-动画
- [x] CharacterAsset 角色资源

### 关卡单位导入流程

- [ ] Rotorz.Tile.Brush 笔刷
- [x] UnityEngine.Sprite 预览
- [x] LevelElement 关卡单位

### 导入方法

- `AssetBundle.LoadFromFile`

`AssetBundle` 的 assets 文件需要其中有一个 AssetBundle 类型的对象定义索引

## 开发记录

### 游戏实体位置

- `GameScene:/Elements/TileSystems/Gameplay/`
- `DontDestroyOnLoad:/`

### 创建实体

[CharacterAsset](docs/CharacterAsset.md)

- `CustomAssetObject.CreateGameObject`

### 创建精灵图集

[tk2dSpriteCollectionData](docs/tk2dSpriteCollectionData.md)

- `tk2dSpriteCollectionData.CreateFromTexture`
- `new GameObject("SpriteCollection").AddComponent<tk2dSpriteCollectionData>()`

### 创建声音

[FMODAsset](docs/FMODAsset.md)

- `FMODUnity.RuntimeManager.LoadBank`
- `Bank.getEventList`

支持从 `znt_Data/StreamingAssets/xxx.bank` (File) 和 `AssetBundle` (TextAsset) 导入 `Bank`  
`FMODAsset` 创建后还需要加入 `FmodAssetIndex`

### 创建动画

[tk2dSpriteAnimation](docs/tk2dSpriteAnimation.md)

- `new GameObject("Animation").AddComponent<tk2dSpriteAnimation>()`

### 创建关卡单位

[LevelElement](docs/LevelElement.md)

`LevelElement` 创建后需要加入 `LevelElementIndex`