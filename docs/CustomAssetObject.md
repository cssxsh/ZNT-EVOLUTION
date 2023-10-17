# CustomAssetObject

> 主要有 HumanAsset, ZombieAsset, DecorAsset

## 结构

- HumanAsset
  * name
  * SpriteCollection `tk2dSpriteCollectionData`
  * AnimationLibrary `tk2dSpriteAnimation` 定义了角色的各种动作
  * MagazineSize 弹匣容量
  * ...

- DecorAsset
  * name
  * Animation `tk2dSpriteAnimation`
  * ActivateAnimation
  * DeactivateAnimation
  * UseStaticAnimation `true`/`false`
  * ActiveAnimation
  * InactiveAnimation
  * playSounds `true`/`false`
  * sound `FMODAsset`