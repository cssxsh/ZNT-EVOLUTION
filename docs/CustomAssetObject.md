# CustomAssetObject

> 主要有 HumanAsset, ZombieAsset, DecorAsset

## 结构

- HumanAsset
  * name
  * SpriteCollection `tk2dSpriteCollectionData`
  * AnimationLibrary `tk2dSpriteAnimation` 定义了角色的各种动作
  * RiseAsset `CustomAssetObject` 转变类型
  * MagazineSize 弹匣容量
  * DamageMultipliers `UnityDictionary<DamageType, float>` 伤害倍率
  * Attachments `UnityDictionary<string, GameObject>` 
  * ...
  * Voice `Voice` 语音类型

```csharp
public enum DamageType
{
  None,
  Fall,
  Bite,
  Gun,
  Rifle,
  Shotgun,
  Melee,
  Sword,
  Canon,
  Explosion,
  Spikes,
  Fire,
  Electricity,
  Laser,
  Sentry,
  Contamination,
  Sacrifice,
  Acid,
  Radioactivity,
  Ripped,
  Plasma,
  Squashed,
  TankDash,
  MachineGun,
  Crawler,
  Tank,
  Boomer,
  Spit,
  HolyFire,
}
```

```text
"Attachments": {
    "moving_attack": "Gertrude Moving Attack : UnityEngine.GameObject",
    "moving_attack": "DrugLord Moving Attack : UnityEngine.GameObject",
    "moving_attack": "Priest Moving Attack : UnityEngine.GameObject",
    "moving_attack": "Lumberjack Moving Attack : UnityEngine.GameObject",
    "shield_attack": "VirginShield : UnityEngine.GameObject",
    "shield_effect": "RepelWave : UnityEngine.GameObject",
    "attach_laser": "LaserAttachment : UnityEngine.GameObject"
}
```

```csharp
public enum Voice
{
  None,
  Woman1,
  Woman2,
  Man1,
  Man2,
  Survivor1,
  Survivor2,
  Bouncer,
  Cop,
  Hazmat,
  Doctor,
  Computer,
  Dj,
  Clown,
  Terminator,
  DrugLord,
  Gertrude,
  Chemist,
  ChemistMutated,
  ChemistFinal,
  Michelle,
  MachineGunner,
  Director,
  Preacher,
  Homeless,
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