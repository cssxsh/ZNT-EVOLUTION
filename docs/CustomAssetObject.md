# CustomAssetObject

> 主要有 HumanAsset, ZombieAsset, DecorAsset

## 结构

- HumanAsset
  * name
  * SpriteCollection `tk2dSpriteCollectionData`
  * AnimationLibrary `tk2dSpriteAnimation` 定义了角色的各种动作
  * MagazineSize 弹匣容量
  * Attachments `UnityDictionary<string, GameObject>` 
  * ...

```log
"Attachments": {
    "moving_attack": "Gertrude Moving Attack : UnityEngine.GameObject",
    "moving_attack": "DrugLord Moving Attack : UnityEngine.GameObject",
    "moving_attack": "Priest Moving Attack : UnityEngine.GameObject",,
    "moving_attack": "Lumberjack Moving Attack : UnityEngine.GameObject"
    "shield_attack": "VirginShield : UnityEngine.GameObject",
    "shield_effect": "RepelWave : UnityEngine.GameObject",
    "attach_laser": "LaserAttachment : UnityEngine.GameObject"
}
```

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