# ZNT-EVOLUTION

## 导入记录

### 导入流程

- [x] FMODAsset 声音资源
- [ ] tk2dSpriteCollectionData 精灵图资源
- [ ] ~~VisualEffect~~
- [x] tk2dSpriteAnimation 精灵图动画
- [ ] ExplosionAsset 技能效果
- [x] PhysicObjectAsset 物理效果
- [x] CharacterAnimationAsset 动作-动画
- [x] CharacterAsset 角色资源

### 导入方法

- `AssetBundle.LoadFromFile`

`AssetBundle` 的 assets 文件需要在 其中有一个 AssetBundle 类型的对象定义索引

## 开发记录

### 游戏实体位置

- `GameScene:/Elements/TileSystems/Gameplay/`
- `DontDestroyOnLoad:/`

### 创建实体

- `CustomAssetObject.CreateGameObject`

### 创建精灵图集

- `UnityWebRequestTexture.GetTexture`
- `tk2dSpriteCollectionData.CreateFromTexture`

### 创建声音

- `FMODUnity.RuntimeManager.LoadBank`

支持从 `znt_Data/StreamingAssets` (File) 和 `AssetBundle` (TextAsset) 导入
导入后需要还需要为 `FMODAsset`

### 创建动画

- tk2dSpriteAnimation
  * clips

- tk2dSpriteAnimationClip
  * name
  * frames
  * fps
  * loopStart
  * useableInLevelEditor
  * staticAnimation
  * wrapMode
  
- tk2dSpriteAnimationFrame
  * ~~ref spriteCollection~~
  * spriteId
  * triggerEvent
  * eventInfo
  * eventInt
  * eventFloat
  * useAttachedEffects
  * ~~ref attachedEffects~~
  * ~~ref shaderAnimator~~
  * playSound
  * ~~ref soundAsset~~
  * soundPlayMode
  * stopEventOnAnimChange
  * preventSoundRestart
  * setSoundParam
  * soundParamName
  * soundParamValue