# ZNT-EVOLUTION

## 导入流程

- [x] FMODAsset
- [x] tk2dSpriteCollectionData 
- [x] ~~VisualEffect~~
- [x] tk2dSpriteAnimation 
- [x] ExplosionAsset 
- [x] PhysicObjectAsset 
- [x] CharacterAnimationAsset 
- [x] CharacterAsset

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