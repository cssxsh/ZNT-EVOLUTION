# Enum

> 常用枚举

## DamageType

> 攻击类型

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

## Voice

> 声音类型

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

## CharacterType

> 角色类型

```csharp
public enum CharacterType
{
  Zombie,
  Civilian,
  ArmedCivilian,
  Survivor,
  Cop,
  MenInBlack,
  Inhuman,
  Boss,
  Astronaut,
  Cultist,
}
```

## Tag

> 标签

| Index  | Mask         |        Name        |
|:-------|:-------------|:------------------:|
| `0x00` | `0x00000001` |       `Door`       |
| `0x01` | `0x00000002` |   `IgnoreAttack`   |
| `0x02` | `0x00000004` |    `Character`     |
| `0x03` | `0x00000008` |      `Human`       |
| `0x04` | `0x00000010` |      `Zombie`      |
| `0x05` | `0x00000020` |   `Interactable`   |
| `0x06` | `0x00000040` |    `Breakable`     |
| `0x07` | `0x00000080` |       `Tile`       |
| `0x08` | `0x00000100` |      `Decor`       |
| `0x09` | `0x00000200` |      `Slope`       |
| `0x0A` | `0x00000400` |   `GameplayTile`   |
| `0x0B` | `0x00000800` |     `NoForce`      |
| `0x0C` | `0x00001000` |  `Indestructible`  |
| `0x0D` | `0x00002000` |   `Destructible`   |
| `0x0E` | `0x00004000` |   `CannotAttack`   |
| `0x0F` | `0x00008000` |      `Sneaky`      |
| `0x10` | `0x00010000` | `StaticCharacter`  |
| `0x11` | `0x00020000` |    `WorldEnemy`    |
| `0x12` | `0x00040000` | `IgnoreProjectile` |
| `0x13` | `0x00080000` |      `Corpse`      |
| `0x14` | `0x00100000` |      `Vomit`       |
| `0x15` | `0x00200000` |       `Vip`        |
| `0x16` | `0x00400000` |    `Projectile`    |
| `0x17` | `0x00800000` |     `NoDanger`     |
| `0x18` | `0x01000000` | `IgnoreUiRaycast`  |

# Layer

> 图层

| Index  | Mask         |        Name         |
|:-------|:-------------|:-------------------:|
| `0x00` | `0x00000001` |      `Default`      |
| `0x01` | `0x00000002` |   `TransparentFX`   |
| `0x02` | `0x00000004` |  `Ignore Raycast`   |
| `0x03` | `0x00000008` |                     |
| `0x04` | `0x00000010` |       `Water`       |
| `0x05` | `0x00000020` |        `UI`         |
| `0x06` | `0x00000040` |                     |
| `0x07` | `0x00000080` |                     |
| `0x08` | `0x00000100` |      `Zombie`       |
| `0x09` | `0x00000200` |       `Human`       |
| `0x0A` | `0x00000400` | `Ignore Collisions` |
| `0x0B` | `0x00000800` | `Ignore Characters` |
| `0x0C` | `0x00001000` |  `Zombie Stopper`   |
| `0x0D` | `0x00002000` |     `Renderer`      |
| `0x0E` | `0x00004000` |       `Crate`       |
| `0x0F` | `0x00008000` |     `Gameplay`      |
| `0x10` | `0x00010000` |       `Prop`        |
| `0x11` | `0x00020000` |    `Foreground`     |
| `0x12` | `0x00040000` |      `One Way`      |
| `0x13` | `0x00080000` |     `Exterior`      |
| `0x14` | `0x00100000` |      `Stairs`       |
| `0x15` | `0x00200000` |    `Stairs Top`     |
| `0x16` | `0x00400000` |   `Middleground`    |
| `0x17` | `0x00800000` |    `Background`     |
| `0x18` | `0x01000000` |        `Sky`        |
| `0x19` | `0x02000000` |    `Projectile`     |
| `0x1A` | `0x04000000` |       `Spit`        |
| `0x1B` | `0x08000000` |   `Block Humans`    |
| `0x1C` | `0x10000000` |   `Block Zombies`   |
| `0x1D` | `0x20000000` |    `World Enemy`    |
| `0x1E` | `0x40000000` |    `Moving Trap`    |
| `0x1F` | `0x80000000` |  `Block Explosion`  |

## Layer Collision

> 碰撞

| Collision | 0x00 | 0x01 | 0x02 | 0x03 | 0x04 | 0x05 | 0x06 | 0x07 | 0x08 | 0x09 | 0x0A | 0x0B | 0x0C | 0x0D | 0x0E | 0x0F | 0x10 | 0x11 | 0x12 | 0x13 | 0x14 | 0x15 | 0x16 | 0x17 | 0x18 | 0x19 | 0x1A | 0x1B | 0x1C | 0x1D | 0x1E | 0x1F |
|:---------:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|
| **0x00**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |      |  Y   |  Y   |  Y   |      |      |      |  Y   |  Y   |      |      |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x01**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x02**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x03**  |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |
| **0x04**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |      |      |  Y   |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |  Y   |      |      |  Y   |      |      |
| **0x05**  |      |      |      |  Y   |      |  Y   |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x06**  |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |
| **0x07**  |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |
| **0x08**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |      |      |      |      |  Y   |      |  Y   |  Y   |  Y   |      |      |      |  Y   |  Y   |      |      |      |  Y   |      |      |  Y   |      |  Y   |      |
| **0x09**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |      |      |      |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |  Y   |  Y   |  Y   |      |      |      |      |
| **0x0A**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x0B**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |
| **0x0C**  |      |      |      |  Y   |      |      |  Y   |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |  Y   |      |      |
| **0x0D**  |      |      |      |  Y   |  Y   |      |  Y   |  Y   |      |      |      |      |      |  Y   |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |  Y   |      |
| **0x0E**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |  Y   |  Y   |  Y   |  Y   |      |      |      |      |  Y   |      |  Y   |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x0F**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |  Y   |  Y   |      |  Y   |      |      |  Y   |      |  Y   |      |      |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x10**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |  Y   |      |      |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |  Y   |      |  Y   |      |      |      |      |      |      |
| **0x11**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x12**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x13**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x14**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |  Y   |      |      |  Y   |      |      |      |      |      |      |      |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x15**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |  Y   |  Y   |  Y   |  Y   |      |      |      |      |      |      |  Y   |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x16**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x17**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |  Y   |      |  Y   |      |      |      |      |  Y   |      |      |      |      |      |      |      |      |      |      |
| **0x18**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x19**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |      |      |      |  Y   |  Y   |  Y   |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |      |      |  Y   |      |      |
| **0x1A**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |      |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |  Y   |      |      |      |  Y   |      |  Y   |
| **0x1B**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x1C**  |      |      |      |  Y   |      |      |  Y   |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |  Y   |      |      |
| **0x1D**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |      |      |      |      |  Y   |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |  Y   |  Y   |      |  Y   |      |      |      |
| **0x1E**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |      |      |      |      |  Y   |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |
| **0x1F**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |  Y   |      |      |      |      |      |

## Sorting Layer

| ID           | Name                  | Value |
|:-------------|:----------------------|:-----:|
| `0x08D7B4B9` | `SkyDecor`            | `-12` |
| `0xC22B02A3` | `BackgroundDecor`     | `-11` |
| `0x88F263AD` | `MiddlegroundDecor`   | `-10` |
| `0x220BEC37` | `ExteriorDecor`       | `-9`  |
| `0xA3EA8441` | `ExteriorTile`        | `-8`  |
| `0x09EEEB53` | `ExteriorDetailsTile` | `-7`  |
| `0x03B836D7` | `WallBackDecor`       | `-6`  |
| `0xFDB611FD` | `Rain`                | `-5`  |
| `0x387DDD9F` | `WallTile`            | `-4`  |
| `0xC81B7ABF` | `WallFrontDecor`      | `-3`  |
| `0xE5855F21` | `WallFrontTile`       | `-2`  |
| `0x9B92996F` | `GameplayDecor`       | `-1`  |
| `0x00000000` | `Default`             |  `0`  |
| `0x0F73C973` | `Elevator`            |  `1`  |
| `0xC82E234F` | `Door`                |  `2`  |
| `0x5374895F` | `Prop`                |  `3`  |
| `0x270E1B19` | `Blood`               |  `4`  |
| `0x4F949767` | `Corpse`              |  `5`  |
| `0x2322C27B` | `Static Zombie`       |  `6`  |
| `0x6813A8E1` | `Human`               |  `7`  |
| `0x45A8730B` | `Bonus`               |  `8`  |
| `0x11A0103F` | `Zombie`              |  `9`  |
| `0xEE2FF0B5` | `Zombie Overlord`     | `10`  |
| `0xFCC6DE09` | `Trap`                | `11`  |
| `0x160BBE75` | `StairsTile`          | `12`  |
| `0xBE07F37F` | `Water`               | `13`  |
| `0x311FDB2F` | `GameplayTile`        | `14`  |
| `0x2095F59D` | `BloodUpTile`         | `15`  |
| `0x9D109DA9` | `BloodDownTile`       | `16`  |
| `0x8EA23237` | `BloodLeftLeftTile`   | `17`  |
| `0x8297AB09` | `BloodDownRightTile`  | `18`  |
| `0x3604A1DD` | `ForegroundDecor`     | `19`  |
| `0x624661BB` | `GameplayFrontTile`   | `20`  |
| `0x94841565` | `Foreground Parallax` | `21`  |
| `0xBFD74D83` | `FrontEffect`         | `22`  |
| `0xF16F8A89` | `Mask`                | `23`  |
| `0x94DD6639` | `LevelEditor`         | `24`  |
| `0x4629EB59` | `UI`                  | `25`  |

# UnityEngine.AnimationCurve

> 动画曲线

ZNT 将其化用为衰减函数，x 轴是标准化的距离，y 轴是倍率  
根据实际使用情况，插件提供两种简写：  
- `(x1, y1) - (x2, y2)`
- `(x1, y1, k1) ~ (x2, y2, k2)`

# ExplosionAsset

> 爆炸资源

## Explosion Create

> `*.explosion.json`

- `Type` : `ExplosionType` 起爆类型
    * `Normal`
    * `Fragmentation`
    * `Continuous`
- `DamageType` : [`DamageType`](#damagetype) 伤害类型
- `DetectedLayers` : [`UnityEngine.LayerMask`](#layer) 作用图层
- `OriginOffset` : `UnityEngine.Vector3`
- `autoExplode` : `bool` 自动起爆
- `Delay` : `float` 起爆延迟
- `DamageRadius` : `float` 伤害半径
- `TileRadius` : `float`
- `ApplyDamageOn` : [`Tag`](#tag) 伤害作用于
- `Damage` : `float` 伤害值
- `DamageDistanceFallof` : `UnityEngine.AnimationCurve` 伤害衰变
- `ApplyForceOn` : [`Tag`](#tag) 推力作用于
- `ForceMode` : `UnityEngine.ForceMode2D`
    * `Force`
    * `Impulse`
- `Force` : `float` 推力大小
- `ForceMultipliers` : `UnityDictionary<Layer, float>` 推力倍率
- `ForceDistanceFallof` : `UnityEngine.AnimationCurve` 推力衰变
- `CustomForceDirection` : `bool` 锁定推力方向，用于反击
- `ForceDirection` : `UnityEngine.Vector2` 推力方向
- `TransformForceDirection` : `bool` 根据 `Transform` 锁定推力方向
- `Duration` : `float` 循环起爆间隔
- `FragSubExplosion` : [`ExplosionAsset`](#explosionasset)
- `FragSettings` : `FragSettings[]`
    * `Offset` : `UnityEngine.Vector2`
    * `Direction` : `UnityEngine.Vector2`
    * `Force` : `float`
    * `Delay` : `float`
    * `Throw` : `float`
    * `ColliderRadius` : `float`
- `CheckObstacles` : `bool`
- `CheckObstaclesOriginOffset` : `UnityEngine.Vector3`
- `ShakeCamera` : `bool` 抖动摄像头
- `ShakeParams` : `ShakeParams` 抖动参数
    * `Duration` : `float`
    * `TranslationStrength` : `float`
    * `RotationStrength` : `float`
    * `Vibrato` : `int`
    * `Randomness` : `float`
- `AlertHumans` : `bool` 提醒人类
- `AlertRadiusAddition` : `float`
- `Sound` : [`FMODAsset`](FMODAsset.md)
- `SpawnEffectOnExplode` : `bool` 起爆时生成特效
- `EffectToSpawn` : [`VisualEffect`](Sprite.md#visualeffect) 起爆时生成特效
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
    * `CircularExplosion`
    * `CircularEnemyExplosion`
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`UnityEngine.Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 爆炸名称

```json
{
  "Type": "Normal",
  "DamageType": "Boomer",
  "DetectedLayers": "Zombie, Human, Ignore Characters, Prop, Projectile, World Enemy",
  "OriginOffset": {
    "$type": "UnityEngine.Vector3, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 1.0,
    "z": 0.0
  },
  "autoExplode": true,
  "Delay": 0.9,
  "DamageRadius": 4.0,
  "TileRadius": 3.0,
  "ApplyDamageOn": "Human, Breakable, WorldEnemy, Projectile",
  "Damage": 300.0,
  "DamageDistanceFallof": "(0.3, 1, -0.004121497) ~ (1, 0.25, -3.207787)",
  "ApplyForceOn": "Human, Zombie, WorldEnemy",
  "ForceMode": "Impulse",
  "Force": 9.0,
  "ForceMultipliers": {},
  "ForceDistanceFallof": "(0.5, 1, -0.004121497) ~ (1, 0.5, -2.414424)",
  "CustomForceDirection": false,
  "ForceDirection": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 0.0
  },
  "TransformForceDirection": false,
  "Duration": 0.0,
  "FragSubExplosion": null,
  "FragSettings": [],
  "CheckObstacles": true,
  "CheckObstaclesOriginOffset": {
    "$type": "UnityEngine.Vector3, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 0.0,
    "z": 0.0
  },
  "ShakeCamera": true,
  "ShakeParams": {
    "Duration": 1.25,
    "TranslationStrength": 3.0,
    "RotationStrength": 8.0,
    "Vibrato": 40,
    "Randomness": 90.0
  },
  "AlertHumans": true,
  "AlertRadiusAddition": 3.0,
  "Sound": null,
  "SpawnEffectOnExplode": false,
  "EffectToSpawn": null,
  "HierarchyName": "",
  "Prefab": "CircularExplosion : UnityEngine.Transform",
  "Tag": 0,
  "Layer": "Default",
  "assetId": "",
  "name": "BasicExplosion",
  "hideFlags": "None"
}
```

## BuildIn

| Assets                 | PathID | Name                        | Prefab                   | Type            |
|:-----------------------|:-------|:----------------------------|:-------------------------|:----------------|
| `sharedassets3.assets` | `113`  | `BasicExplosion`            | `CircularExplosion`      | `Normal`        |
| `sharedassets3.assets` | `114`  | `BlockerExplosion`          | `CircularExplosion`      | `Fragmentation` |
| `sharedassets3.assets` | `115`  | `BasicSpitExplosion`        | `CircularExplosion`      | `Normal`        |
| `sharedassets3.assets` | `116`  | `CrawlerSpitExplosion`      | `CircularExplosion`      | `Normal`        |
| `sharedassets3.assets` | `117`  | `OverlordSpitExplosion`     | `CircularExplosion`      | `Normal`        |
| `sharedassets3.assets` | `118`  | `TankSpitExplosion`         | `TankSpitExplosion`      | `Continuous`    |
| `resources.assets`     | `9659` | `AstrogoliathEscape`        | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9660` | `AstrogoliathLandingAttack` | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9661` | `ChemistRepulse`            | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9662` | `DrugLordRepulse`           | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9663` | `GertrudeAttack`            | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9664` | `GertrudeRepulse`           | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9665` | `KamikazeExplosion`         | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9666` | `RickRepulse`               | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9667` | `TerminatorRepulse`         | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9668` | `ClimberExplosion`          | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9669` | `SubExplosion`              | `FragExplosion`          | `Normal`        |
| `resources.assets`     | `9670` | `TankExplosion`             | `CircularExplosion`      | `Fragmentation` |
| `resources.assets`     | `9671` | `TankJumpExplosion`         | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9672` | `TankScream`                | `TankScream`             | `Continuous`    |
| `resources.assets`     | `9673` | `BarrelExplosion`           | `CircularEnemyExplosion` | `Normal`        |
| `resources.assets`     | `9674` | `CannonballExplosion`       | `CircularEnemyExplosion` | `Normal`        |
| `resources.assets`     | `9675` | `GazolineTankExplosion`     | `CircularEnemyExplosion` | `Normal`        |
| `resources.assets`     | `9676` | `GrenadeExplosion`          | `CircularEnemyExplosion` | `Normal`        |
| `resources.assets`     | `9677` | `LaserShotExplosion`        | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9678` | `MachineGunnerRepulse`      | `CircularExplosion`      | `Normal`        |
| `resources.assets`     | `9679` | `MineExplosion`             | `CircularEnemyExplosion` | `Normal`        |
| `resources.assets`     | `9680` | `MolotovExplosion`          | `CircularEnemyExplosion` | `Normal`        |
| `resources.assets`     | `9681` | `SentryCannonExplosion`     | `CircularEnemyExplosion` | `Normal`        |
| `resources.assets`     | `9682` | `SentryPlasmaExplosion`     | `CircularExplosion`      | `Normal`        |

# DecorAsset

> 物件资源

## Decor Create

> `*.decor.json`

- `Animation` : [`tk2dSpriteAnimation`](Sprite.md#tk2dspriteanimation)
- `ActivateAnimation` : `string`
- `DeactivateAnimation` : `string`
- `UseStaticAnimation` : `bool`
- `ActiveAnimation` : `string`
- `InactiveAnimation` : `string`
- `playSounds` : `bool` 是否播放声音
- `sound` : [`FMODAsset`](FMODAsset.md)
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
    * `IndicatorDoor`
    * `ActivableDecor`
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`UnityEngine.Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 物件名称

```json
{
  "Animation": "anim_traps : tk2dSpriteAnimation",
  "ActivateAnimation": "alarm_on",
  "DeactivateAnimation": "alarm_off",
  "UseStaticAnimation": false,
  "ActiveAnimation": "alarm_on",
  "InactiveAnimation": "alarm_off",
  "playSounds": false,
  "sound": null,
  "HierarchyName": "Alarm",
  "Prefab": "Alarm : UnityEngine.Transform",
  "Tag": "Decor",
  "Layer": "Prop",
  "assetId": "",
  "name": "Alarm",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID | Name                 | Prefab            | Animation               |
|:-------------------|:-------|:---------------------|:------------------|:------------------------|
| `resources.assets` | `9633` | `Alarm`              | `Alarm`           | `anim_traps`            |
| `resources.assets` | `9634` | `Aliens1`            | `Aliens1`         | `anim_aliens_cinematic` |
| `resources.assets` | `9635` | `Aliens2`            | `Aliens2`         | `anim_aliens_cinematic` |
| `resources.assets` | `9636` | `Aliens3`            | `Aliens3`         | `anim_aliens_cinematic` |
| `resources.assets` | `9637` | `Aliens5`            | `Aliens5`         | `anim_aliens_cinematic` |
| `resources.assets` | `9638` | `Aliens6`            | `Aliens6`         | `anim_aliens_cinematic` |
| `resources.assets` | `9639` | `Aliens7`            | `Aliens7`         | `anim_aliens_cinematic` |
| `resources.assets` | `9640` | `Aliens8`            | `Aliens8`         | `anim_aliens_cinematic` |
| `resources.assets` | `9641` | `Aliens9`            | `Aliens9`         | `anim_aliens_cinematic` |
| `resources.assets` | `9642` | `ElevatorDoor`       | `ElevatorDoor`    | `anim_props`            |
| `resources.assets` | `9643` | `FanHospital`        | `FanHospital`     | `anim_props`            |
| `resources.assets` | `9644` | `GarbageGirl`        | `GarbageGirl`     | `anim_girl_3_garbage`   |
| `resources.assets` | `9645` | `IndicatorDoor`      | `IndicatorDoor`   | `anim_props`            |
| `resources.assets` | `9646` | `MotherBrain`        | `MotherbrainRoom` | `anim_props`            |
| `resources.assets` | `9647` | `Plier`              | `Plier`           | `anim_chemist_pliers`   |
| `resources.assets` | `9648` | `Radio`              | `ActivableDecor`  | `anim_props`            |
| `resources.assets` | `9649` | `RomeroBlinking`     | `RomeroBlinking`  | `anim_props`            |
| `resources.assets` | `9650` | `Troll`              | `ActivableDecor`  | `anim_traps`            |
| `resources.assets` | `9651` | `TutorialClick`      | `ActivableDecor`  | `anim_props`            |
| `resources.assets` | `9652` | `TutorialExplode`    | `ActivableDecor`  | `anim_props`            |
| `resources.assets` | `9653` | `TutorialOverlord`   | `ActivableDecor`  | `anim_props`            |
| `resources.assets` | `9654` | `TutorialScream`     | `ActivableDecor`  | `anim_props`            |
| `resources.assets` | `9655` | `TutorialScreamWall` | `ActivableDecor`  | `anim_props`            |

# BreakablePropAsset

> 可破坏道具资源

## Breakable Create

> `*.breakable.json`

- `Animation` : [`tk2dSpriteAnimation`](Sprite.md#tk2dspriteanimation)
- `ActiveAnimation` : `string`
- `InactiveAnimation` : `string`
- `HitAnimation` : `string`
- `BrokenAnimation` : `string`
- `ColliderSize` : `UnityEngine.Vector2`
- `ColliderOffset` : `UnityEngine.Vector2`
- `MaxHp` : `int`
- `UserInteractable` : `bool`
- `MenuOffset` : `UnityEngine.Vector2`
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
    * `BreakablePropinteractable`
    * `BreakableProp`
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`UnityEngine.Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 可破坏道具名称

```json
{
  "Animation": "anim_traps : tk2dSpriteAnimation",
  "ActiveAnimation": "monolith_on",
  "InactiveAnimation": "monolith_off",
  "HitAnimation": "monolith_hit",
  "BrokenAnimation": "monolith_destroyed",
  "ColliderSize": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 2.0,
    "y": 3.0
  },
  "ColliderOffset": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 0.5,
    "y": 0.5
  },
  "MaxHp": 250,
  "UserInteractable": true,
  "MenuOffset": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 0.0,
    "y": -0.5
  },
  "HierarchyName": "Monolith",
  "Prefab": "BreakablePropinteractable : UnityEngine.Transform",
  "Tag": "Interactable, Breakable",
  "Layer": "Prop",
  "assetId": "",
  "name": "Monolith",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID | Name                    | Prefab                      | Animation    |
|:-------------------|:-------|:------------------------|:----------------------------|:-------------|
| `resources.assets` | `9686` | `ElectricGenerator 1`   | `BreakablePropinteractable` | `anim_traps` |
| `resources.assets` | `9687` | `ElectricGenerator 2`   | `BreakablePropinteractable` | `anim_traps` |
| `resources.assets` | `9688` | `ElectricGenerator Big` | `BreakableProp`             | `anim_traps` |
| `resources.assets` | `9689` | `GazolineTank`          | `Gazoline Tank`             | `anim_traps` |
| `resources.assets` | `9690` | `Monolith`              | `BreakablePropinteractable` | `anim_traps` |

# TriggerAsset

> 开关资源

## Trigger Create

> `*.trigger.json`

- `type` : `TriggerAsset.TrapType` 触发类型
    * `Receiver`
    * `Trap`
    * `Trigger`
    * `Both`
- `OverrideAnimations` : `bool`
- `PlaySounds` : `bool`
- `DetectedLayers` : [`UnityEngine.LayerMask`](#layer) 作用图层
- `CheckTags` : `bool`
- `DetectedTags` : [`Tag`](#tag) 作用标签
- `resizeMode` : `TriggerAsset.ResizeAxis`
    * `None`
    * `Horizontal`
    * `Vertical`
    * `Both`
- `clampMethod` : `ResizeHandles.ClampType`
    * `None`
    * `Relative`
    * `Absolute`
- `minSize` : `UnityEngine.Vector2`
- `roundToNeareset` : `bool`
- `RendererColor` : `UnityEngine.Color`
- `RenderMode` : `TrapRenderMode`
    * `Tiled`
    * `SlicedAndTiled`
    * `Sliced`
    * `None`
- `PixelsPerMeter` : `int`
- `EffectPrefab` : `ResizableParticleSystem`
    * `ElectricArc`
    * `HeatWaves`
- `Animation` : [`tk2dSpriteAnimation`](Sprite.md#tk2dspriteanimation)
- `ActiveAnimation` : `string`
- `InactiveAnimation` : `string`
- `ActivateAnimation` : `string`
- `DeactivateAnimation` : `string`
- `ActivateEvent` : [`FMODAsset`](FMODAsset.md)
- `EventFollowObject` : `bool`
- `Type` : [`DamageType`](#damagetype) 伤害类型
- `Mode` : `TrapMode`
    * `Damage`
    * `Kill`
- `KillDelay` : `float`
- `Damage` : `float`
- `DamageRate` : `float`
- `Name` : `string` 菜单栏中的名字
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
    * `BaseTrap`
    * `InvisibleTrap`
    * `InvisibleTrigger`
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`UnityEngine.Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 开关名称

```csharp
{
  "type": "Both",
  "OverrideAnimations": false,
  "PlaySounds": false,
  "DetectedLayers": "Zombie, Human, World Enemy",
  "CheckTags": true,
  "DetectedTags": "Human, Zombie, WorldEnemy",
  "resizeMode": "Both",
  "clampMethod": "Absolute",
  "minSize": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 0.25,
    "y": 0.25
  },
  "roundToNeareset": 0.25,
  "RendererColor": {
    "$type": "UnityEngine.Color, UnityEngine.CoreModule",
    "r": 1.0,
    "g": 1.0,
    "b": 1.0,
    "a": 1.0
  },
  "RenderMode": "None",
  "PixelsPerMeter": 12,
  "EffectPrefab": null,
  "Animation": "anim_props : tk2dSpriteAnimation",
  "ActiveAnimation": "",
  "InactiveAnimation": "",
  "ActivateAnimation": "",
  "DeactivateAnimation": "",
  "ActivateEvent": null,
  "EventFollowObject": false,
  "Type": "Bite",
  "Mode": "Kill",
  "KillDelay": 0.1,
  "Damage": 1000.0,
  "DamageRate": 0.0,
  "Name": "InvisibleTrap",
  "HierarchyName": "InvisibleTrap",
  "Prefab": "InvisibleTrap : UnityEngine.Transform",
  "Tag": 0,
  "Layer": "Default",
  "assetId": "",
  "name": "InvisibleTrap",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID | Name                    | Prefab                    |
|:-------------------|:-------|:------------------------|:--------------------------|
| `resources.assets` | `9691` | `ActivationFilter`      | `ActivationFilter`        |
| `resources.assets` | `9692` | `BonusReward`           | `BonusReward`             |
| `resources.assets` | `9693` | `CameraControl`         | `CameraControl`           |
| `resources.assets` | `9694` | `CameraMover`           | `CameraMover`             |
| `resources.assets` | `9695` | `CameraShake`           | `CameraShake`             |
| `resources.assets` | `9696` | `CameraZoomer`          | `CameraZoomer`            |
| `resources.assets` | `9697` | `ChallengeTrigger`      | `ChallengeTrigger`        |
| `resources.assets` | `9698` | `CharacterCondition`    | `ActiveCharactersControl` |
| `resources.assets` | `9699` | `CountdownTimer`        | `CountdownTimer`          |
| `resources.assets` | `9700` | `Counter`               | `Counter`                 |
| `resources.assets` | `9701` | `EndDemoLoader`         | `EndDemoLoader`           |
| `resources.assets` | `9702` | `EndGameControl`        | `EndGameControl`          |
| `resources.assets` | `9703` | `ExecutionTimer`        | `ExecutionTimer`          |
| `resources.assets` | `9704` | `GamePause`             | `GamePause`               |
| `resources.assets` | `9705` | `HudControl`            | `HudControl`              |
| `resources.assets` | `9706` | `LevelLoader`           | `LevelLoader`             |
| `resources.assets` | `9707` | `LoopTimer`             | `LoopTimer`               |
| `resources.assets` | `9708` | `MutationLocker`        | `MutationLocker`          |
| `resources.assets` | `9709` | `ProgressionUnlock`     | `ProgressionUnlock`       |
| `resources.assets` | `9710` | `SlowMotion`            | `SlowMotion`              |
| `resources.assets` | `9711` | `AstronautDeath`        | `AstronautDeath`          |
| `resources.assets` | `9712` | `BloodSpurt`            | `BloodSpurt`              |
| `resources.assets` | `9713` | `CaveDebris`            | `CaveDebris`              |
| `resources.assets` | `9714` | `ChemistBossFinalIntro` | `BossChemistFinal_intro`  |
| `resources.assets` | `9715` | `FloatingZombie1`       | `FloatingZombie1`         |
| `resources.assets` | `9716` | `FloatingZombie2`       | `FloatingZombie2`         |
| `resources.assets` | `9717` | `FloatingZombie3`       | `FloatingZombie3`         |
| `resources.assets` | `9718` | `FlyingStars`           | `FlyingStars`             |
| `resources.assets` | `9719` | `GreenFog`              | `GreenFog`                |
| `resources.assets` | `9720` | `GreenHaze`             | `GreenHaze`               |
| `resources.assets` | `9721` | `GreenSteam`            | `GreenSteam`              |
| `resources.assets` | `9722` | `MeteoriteCrash`        | `MeteoriteCrash`          |
| `resources.assets` | `9723` | `MonolithBreak`         | `MonolithBreak`           |
| `resources.assets` | `9724` | `RocketCrash`           | `RocketCrash`             |
| `resources.assets` | `9725` | `RocketTakeOff`         | `RocketTakeOff`           |
| `resources.assets` | `9726` | `RoofDebris`            | `RoofDebris`              |
| `resources.assets` | `9727` | `SpaceTimeTeleport`     | `SpaceTimeTeleport`       |
| `resources.assets` | `9728` | `Sprinkler`             | `Sprinkler`               |
| `resources.assets` | `9729` | `Tunnel`                | `TunnelCamera`            |
| `resources.assets` | `9730` | `Vortex`                | `Vortex`                  |
| `resources.assets` | `9731` | `VortexWind`            | `VortexWind`              |
| `resources.assets` | `9732` | `WhiteFog`              | `WhiteFog`                |
| `resources.assets` | `9775` | `Acid`                  | `AcidTrap`                |
| `resources.assets` | `9776` | `BreakableFloor`        | `BreakableFloor`          |
| `resources.assets` | `9777` | `Electricity`           | `BaseTrap`                |
| `resources.assets` | `9778` | `Fire`                  | `BaseTrap`                |
| `resources.assets` | `9779` | `InvisibleTrap`         | `InvisibleTrap`           |
| `resources.assets` | `9780` | `Laser`                 | `LaserTrap`               |
| `resources.assets` | `9781` | `Mine`                  | `MineTrap`                |
| `resources.assets` | `9782` | `MovingSurface`         | `MovingSurface`           |
| `resources.assets` | `9783` | `Radioactivity`         | `RadioactivityTrap`       |
| `resources.assets` | `9788` | `Spikes`                | `BaseTrap`                |
| `resources.assets` | `9789` | `SteamDown`             | `SteamDownTrap`           |
| `resources.assets` | `9790` | `SteamUp`               | `SteamUpTrap`             |
| `resources.assets` | `9791` | `Water`                 | `WaterTrap`               |
| `resources.assets` | `9792` | `Wind`                  | `WindArea`                |
| `resources.assets` | `9793` | `CharacterTrigger`      | `InvisibleTrigger`        |
| `resources.assets` | `9794` | `ClickableObject`       | `ClickableObject`         |
| `resources.assets` | `9795` | `DespawnTrigger`        | `DespawnTrigger`          |
| `resources.assets` | `9796` | `HumanTrigger`          | `InvisibleTrigger`        |
| `resources.assets` | `9797` | `JumpTrigger`           | `JumpTrigger`             |
| `resources.assets` | `9798` | `PreventClick`          | `PreventClick`            |
| `resources.assets` | `9799` | `SpitTrigger`           | `InvisibleTrigger`        |
| `resources.assets` | `9800` | `StartTrigger`          | `StartTrigger`            |
| `resources.assets` | `9801` | `VipTrigger`            | `InvisibleTrigger`        |
| `resources.assets` | `9802` | `ZombieEscape`          | `EscapeTrigger`           |
| `resources.assets` | `9803` | `ZombieTrigger`         | `InvisibleTrigger`        |

# MovingObjectAsset

> 动体资源

## Moving Create

> `*.moving.json`

- `library` : [`tk2dSpriteAnimation`](Sprite.md#tk2dspriteanimation)
- `MoveAnimation` : `string`
- `StandAnimation` : `string`
- `StopAnimation` : `string`
- `DisableAnimation` : `string`
- `HitAnimation` : `string`
- `DestroyAnimation` : `string`
- `StandSound` : [`FMODAsset`](FMODAsset.md)
- `DisableSound` : [`FMODAsset`](FMODAsset.md)
- `MoveSound` : [`FMODAsset`](FMODAsset.md)
- `StopSound` : [`FMODAsset`](FMODAsset.md)
- `HitSound` : [`FMODAsset`](FMODAsset.md)
- `HideOnDisable` : `bool`
- `AttachOnCollide` : `bool`
- `ColliderType` : `MovingObjectAsset.Collider`
    * `None`
    * `Box`
    * `Circle`
- `ColliderIsTrigger` : `bool`
- `ColliderMaterial` : `UnityEngine.PhysicsMaterial2D`
    * `BossBrainPhysic`
    * `BonusPhysic`
    * `CratePhysic`
    * `BarrelPhysic`
    * `RippedPhysic`
    * `SpitPhysic`
    * `CorpsePhysic`
- `ColliderOffset` : `UnityEngine.Vector2`
- `BoxSize` : `UnityEngine.Vector2`
- `CircleRadius` : `float`
- `ApplyDamage` : `bool`
- `DamageShape` : `MovingObjectAsset.Shape`
    * `Box`
    * `Circle`
- `DamageOffset` : `UnityEngine.Vector2`
- `DamageSize` : `UnityEngine.Vector2`
- `DamageRadius` : `float`
- `DamageType` : [`DamageType`](#damagetype) 伤害类型
- `DamageAppliedTo` : [`UnityEngine.LayerMask`](#layer) 伤害作用图层
- `Speed` : `float`
- `MoveOnStart` : `bool`
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
    * `MovingObject`
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`UnityEngine.Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 动体名称

```json
{
  "library": "anim_traps : tk2dSpriteAnimation",
  "MoveAnimation": "car",
  "StandAnimation": "car",
  "StopAnimation": "car",
  "DisableAnimation": "",
  "HitAnimation": "",
  "DestroyAnimation": "",
  "StandSound": null,
  "DisableSound": null,
  "MoveSound": "event:/Environment/Car_startLoop",
  "StopSound": "event:/Environment/Car_stop",
  "HitSound": null,
  "HideOnDisable": false,
  "AttachOnCollide": false,
  "ColliderType": "Box",
  "ColliderIsTrigger": true,
  "ColliderMaterial": null,
  "ColliderOffset": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": -1.0,
    "y": -0.4
  },
  "BoxSize": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 3.25,
    "y": 0.8
  },
  "CircleRadius": 0.0,
  "ApplyDamage": true,
  "DamageShape": "Box",
  "DamageOffset": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": -1.0,
    "y": -0.4
  },
  "DamageSize": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 3.5,
    "y": 1.0
  },
  "DamageRadius": 0.0,
  "DamageType": "Ripped",
  "DamageAppliedTo": "Zombie, Human, World Enemy",
  "Speed": 15.0,
  "MoveOnStart": true,
  "HierarchyName": "Car",
  "Prefab": "MovingObject : UnityEngine.Transform",
  "Tag": 0,
  "Layer": "Moving Trap",
  "assetId": "",
  "name": "Car",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID | Name                    | Prefab                        | Animation    |
|:-------------------|:-------|:------------------------|:------------------------------|:-------------|
| `resources.assets` | `9757` | `Car`                   | `MovingObject`                | `anim_traps` |
| `resources.assets` | `9758` | `CarBoss`               | `MovingCarBoss`               | `anim_traps` |
| `resources.assets` | `9759` | `CircularSaw`           | `CircularSaw`                 | `anim_traps` |
| `resources.assets` | `9760` | `Elevator 1`            | `MovingElevator 1`            | `anim_traps` |
| `resources.assets` | `9761` | `Elevator 2`            | `MovingElevator 2`            | `anim_traps` |
| `resources.assets` | `9762` | `Elevator 3`            | `MovingElevator 3`            | `anim_traps` |
| `resources.assets` | `9763` | `ElevatorExterior`      | `MovingElevatorExterior`      | `anim_traps` |
| `resources.assets` | `9764` | `ElevatorPlatform`      | `MovingElevatorPlatform`      | `anim_traps` |
| `resources.assets` | `9765` | `ElevatorPlatformMoon`  | `MovingMoonPlatform`          | `anim_traps` |
| `resources.assets` | `9766` | `ElevatorPlatformSmall` | `MovingElevatorPlatformSmall` | `anim_traps` |
| `resources.assets` | `9767` | `Harvester`             | `Harvester`                   | `anim_traps` |
| `resources.assets` | `9768` | `InvisibleMovingTrap`   | `InvisibleMovingTrap`         | `anim_traps` |
| `resources.assets` | `9769` | `LawnMower`             | `MovingObject`                | `anim_traps` |
| `resources.assets` | `9770` | `MovingContainer`       | `MovingObject`                | `anim_traps` |
| `resources.assets` | `9771` | `PressWall`             | `MovingPressWall`             | `anim_traps` |
| `resources.assets` | `9772` | `Subway`                | `MovingSubway`                | `anim_traps` |
| `resources.assets` | `9773` | `SubwayTrap`            | `MovingSubway`                | `anim_traps` |
| `resources.assets` | `9774` | `TankCage`              | `TankCage`                    | `anim_traps` |

# PhysicObjectAsset

> 物体资源

## Physic Create

> `*.physic.json`

- `carryParent` : `bool`
- `ignoreCollisionDuration` : `float` 碰撞忽略时间
- `startDirection` : `UnityEngine.Vector2` 初始方向
- `startForce` : `float` 初始力，可视为初始速度
- `allowRotation` : `bool` 允许滚动
- `StartAngularVelocity` : `float`
- `Friction` : `float` 摩擦系数
- `Bounciness` : `float` 弹力系数
- `GravityScale` : `float` 重力系数
- `ColliderRadius` : `float` 碰撞半径
- `ColliderOffset` : `UnityEngine.Vector2`
- `ExplodeOn` : `PhysicObjectBehaviour.ExplodeSurface` 爆炸作用类型
    * `Wall`
    * `Ground`
    * `Ceiling`
    * `Target` 碰到角色就会爆炸
    * `Zombie`
    * `Climber`
    * `Blocker`
    * `Tank`
    * `WorldEnemy`
- `DamageRadius` : `float` 穿透伤害半径
- `DamageOffset` : `UnityEngine.Vector2`
- `DamageCharacterOnTrigger` : `bool` 开启伤害在穿透时
- `DamageCharacterOnCollide` : `bool` 开启伤害在碰撞时
- `AttachToParent` : `bool`
- `DamageAmount` : `float` 伤害值
- `DamageType` : [`DamageType`](#damagetype) 穿透伤害类型
- `TargetLayers` : [`UnityEngine.LayerMask`](#layer) 伤害触发图层
- `Explosion` : [`ExplosionAsset`](#explosionasset) 爆炸
- `playAnimation` : `bool`
- `library` : [`tk2dSpriteAnimation`](Sprite.md#tk2dspriteanimation)
- `MoveAnimation` : `string`
- `StopAnimation` : `string`
- `HitAnimation` : `string`
- `ExplodeEffect` : [`VisualEffect`](Sprite.md#visualeffect)
- `SnapEffectPosition` : `bool`
- `RollingSound` : [`FMODAsset`](FMODAsset.md)
- `HitSound` : [`FMODAsset`](FMODAsset.md)
- `HitSoundParam` : `string`
- `RolDetectionRadius` : `float`
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
    * `PhysicObject`
    * `Grenade`
    * `Molotov`
    * `MoonCanonProjectile`
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`UnityEngine.Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 物体名称

当 `ignoreCollisionDuration` 开启会检查是否卡住（速度为零时爆炸）  
当 `ExplodeOn` 包含 `Target` 会对角色图层产生碰撞（使用默认图层，反之使用忽略角色的图层）  
`ExplodeOn` 的 `Zombie`/`Climber`/`Blocker`/`Tank`/`WorldEnemy` 为代码扩展，不要和 `Target` 一起使用

```json
{
  "carryParent": false,
  "ignoreCollisionDuration": 0.0,
  "startDirection": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 1.0,
    "y": 0.8
  },
  "startForce": 10.0,
  "allowRotation": true,
  "StartAngularVelocity": 1000.0,
  "Friction": 100.0,
  "Bounciness": 0.25,
  "GravityScale": 1.0,
  "ColliderRadius": 0.1,
  "ColliderOffset": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 0.0
  },
  "ExplodeOn": "Ground",
  "DamageRadius": 0.0,
  "DamageOffset": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 0.0
  },
  "DamageCharacterOnTrigger": false,
  "DamageCharacterOnCollide": false,
  "AttachToParent": false,
  "DamageAmount": 0.0,
  "DamageType": "None",
  "TargetLayers": 0,
  "Explosion": "MolotovExplosion : ExplosionAsset",
  "playAnimation": true,
  "library": "anim_traps : tk2dSpriteAnimation",
  "MoveAnimation": "molotov",
  "StopAnimation": "",
  "HitAnimation": "",
  "ExplodeEffect": "MolotovExplosion : VisualEffect",
  "SnapEffectPosition": true,
  "RollingSound": null,
  "HitSound": null,
  "HitSoundParam": "",
  "RolDetectionRadius": 0.0,
  "HierarchyName": "Molotov",
  "Prefab": "Molotov : UnityEngine.Transform",
  "Tag": 0,
  "Layer": "Projectile",
  "assetId": "",
  "name": "Molotov",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID | Name                        | Prefab                | Animation           |
|:-------------------|:-------|:----------------------------|:----------------------|:--------------------|
| `resources.assets` | `9733` | `AstrogoliathLandingAttack` | `PhysicObject`        | `anim_traps`        |
| `resources.assets` | `9734` | `Cannonball`                | `PhysicObject`        | `anim_traps`        |
| `resources.assets` | `9735` | `ChemistAttack`             | `RollingBoss`         | `anim_boss_chemist` |
| `resources.assets` | `9736` | `GertrudeAttack`            | `PhysicObject`        | `anim_traps`        |
| `resources.assets` | `9737` | `Grenade 1`                 | `Grenade`             | `anim_traps`        |
| `resources.assets` | `9738` | `Grenade 2`                 | `Grenade`             | `anim_traps`        |
| `resources.assets` | `9739` | `Grenade 3`                 | `Grenade`             | `anim_traps`        |
| `resources.assets` | `9740` | `Grenade 4`                 | `Grenade`             | `anim_traps`        |
| `resources.assets` | `9741` | `Grenade Cutscene`          | `Grenade Cutscene`    | `anim_traps`        |
| `resources.assets` | `9742` | `LaserShot`                 | `PhysicObject`        | `anim_traps`        |
| `resources.assets` | `9743` | `Molotov 2_short`           | `Molotov`             | `anim_traps`        |
| `resources.assets` | `9744` | `Molotov 3_long`            | `Molotov`             | `anim_traps`        |
| `resources.assets` | `9745` | `Molotov 4_high`            | `Molotov`             | `anim_traps`        |
| `resources.assets` | `9746` | `Molotov 5_grange`          | `Molotov`             | `anim_traps`        |
| `resources.assets` | `9747` | `Molotov 6_homerun`         | `Molotov`             | `anim_traps`        |
| `resources.assets` | `9748` | `Molotov`                   | `Molotov`             | `anim_traps`        |
| `resources.assets` | `9749` | `PhysicBarrel 1`            | `PhysicObject`        | `anim_traps`        |
| `resources.assets` | `9750` | `PhysicBarrel 2`            | `PhysicObject`        | `anim_traps`        |
| `resources.assets` | `9751` | `SentryPlasma1`             | `MoonCanonProjectile` | `anim_traps`        |
| `resources.assets` | `9752` | `SentryPlasma2`             | `MoonCanonProjectile` | `anim_traps`        |
| `resources.assets` | `9753` | `SentryPlasma3`             | `MoonCanonProjectile` | `anim_traps`        |
| `resources.assets` | `9754` | `SentryPlasma4`             | `MoonCanonProjectile` | `anim_traps`        |
| `resources.assets` | `9755` | `SentryProjectile`          | `PhysicObject`        | `anim_traps`        |
| `resources.assets` | `9756` | `SentryProjectileSlow`      | `PhysicObject`        | `anim_traps`        |

# SentryGunAsset

> 炮台资源

## Sentry Create

> `*.sentry.json`

- `Hp` : `float`
- `Invincible` : `bool`
- `DamageMultipliers` : `UnityDictionary<DamageType, float>` 伤害倍率
- `AllDetection` : `DetectionAsset` 恒为 `SentryAllDetection`
- `HumanDetection` : `DetectionAsset` 恒为 `SentryHumanDetection`
- `ZombieDetection` : `DetectionAsset` 恒为 `SentryZombieDetection`
- `GeneralDirection` : `UnityEngine.Vector3`
- `DeviationAngle` : `float`
- `FieldOfView` : `float`
- `Distance` : `float`
- `RayCount` : `int`
- `DamageType` : [`DamageType`](#damagetype)
- `HitMultipleTargets` : `bool`
- `NextTargetsDamageMultiplier` : `float`
- `MaxTargets` : `int`
- `Damage` : `float`
- `CriticalMutliplier` : `float`
- `AimRange` : `float`
- `AttackRange` : `float`
- `DamageRange` : `float`
- `AttackFrequency` : `float`
- `DefaultDamageChance` : `float`
- `CriticalDamageChance` : `float`
- `MissChance` : `float`
- `ThrowableObjects` : [`PhysicObjectAsset[]`](#physicobjectasset)
- `SpriteCollection` : [`tk2dSpriteCollectionData`](Sprite.md#tk2dspritecollectiondata)
- `SpriteIndex` : `int`
- `Animation` : [`tk2dSpriteAnimation`](Sprite.md#tk2dspriteanimation)
- `ActiveAnimation` : `string`
- `InactiveAnimation` : `string`
- `ActivateAnimation` : `string`
- `DeactivateAnimation` : `string`
- `FireAnimation` : `string`
- `Turn` : `string`
- `HitAnimation` : `string`
- `BreakAnimation` : `string`
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
    * `SentryGun`
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`UnityEngine.Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 炮台名称

```json
{
  "Hp": 500.0,
  "Invincible": false,
  "DamageMultipliers": {},
  "AllDetection": "SentryAllDetection : DetectionAsset",
  "HumanDetection": "SentryHumanDetection : DetectionAsset",
  "ZombieDetection": "SentryZombieDetection : DetectionAsset",
  "GeneralDirection": {
    "$type": "UnityEngine.Vector3, UnityEngine.CoreModule",
    "x": 1.0,
    "y": 0.0,
    "z": 0.0
  },
  "DeviationAngle": 0.0,
  "FieldOfView": 15.0,
  "Distance": 15.0,
  "RayCount": 5,
  "DamageType": "Shotgun",
  "HitMultipleTargets": false,
  "NextTargetsDamageMultiplier": 1.0,
  "MaxTargets": 2,
  "Damage": 50.0,
  "CriticalMutliplier": 2.0,
  "AimRange": 15.0,
  "AttackRange": 15.0,
  "DamageRange": 16.0,
  "AttackFrequency": 3.74999952,
  "DefaultDamageChance": 87.5,
  "CriticalDamageChance": 10.0,
  "MissChance": 2.5,
  "ThrowableObjects": [],
  "SpriteCollection": "sprites_drones_sentry : tk2dSpriteCollectionData",
  "SpriteIndex": 320,
  "Animation": "anim_traps : tk2dSpriteAnimation",
  "ActiveAnimation": "sentry_gun_active",
  "InactiveAnimation": "sentry_gun_inactive",
  "ActivateAnimation": "sentry_gun_activate",
  "DeactivateAnimation": "sentry_gun_deactivate",
  "FireAnimation": "sentry_gun_shoot",
  "Turn": "sentry_gun_turn",
  "HitAnimation": "sentry_gun_attacked",
  "BreakAnimation": "sentry_gun_break",
  "HierarchyName": "Sentry Gun",
  "Prefab": "SentryGun : UnityEngine.Transform",
  "Tag": "Breakable",
  "Layer": "Prop",
  "assetId": "",
  "name": "SentryGun",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID | Name               | Prefab      | Animation    |
|:-------------------|:-------|:-------------------|:------------|:-------------|
| `resources.assets` | `9784` | `SentryCannon`     | `SentryGun` | `anim_traps` |
| `resources.assets` | `9785` | `SentryCannonSlow` | `SentryGun` | `anim_traps` |
| `resources.assets` | `9786` | `SentryGun`        | `SentryGun` | `anim_traps` |
| `resources.assets` | `9787` | `SentryMoonCannon` | `SentryGun` | `anim_traps` |

# HumanAsset

> 人类资源

## Human Create

> `*.human.json`

- `Attitude` : `HumanAttitude`
    * `Combative`
    * `Neutral`
    * `Fearfull`
    * `Ignore`
- `DangerRadius` : `float`
- `AlertRadius` : `float`
- `AlertRelayRadius` : `float`
- `AlertRelayRatio` : `float`
- `RelayAlertOverTime` : `bool`
- `ParalyseDuration` : `float`
- `AlertDuration` : `float`
- `FleeDuration` : `float`
- `ScaredDuration` : `float`
- `Contaminable` : `bool`
- `AllowMultipleAttackers` : `bool`
- `GrabbedOnAttacked` : `bool`
- `IgnoreDamages` : `bool` 忽略攻击，取消硬直动画
- `InvincibleOnAttack` : `bool` 攻击时无敌
- `ResistScream` : `bool`
- `FleeBeforeZombieExplode` : `bool` 僵尸爆炸前逃离
- `ReportAlert` : `bool`
- `AllowRage` : `bool`
- `FreezeOnRage` : `bool`
- `RageDamageType` : [`DamageType`](#damagetype)
- `RageRefillTimer` : `float`
- `RageRefillRate` : `int`
- `RageHitCount` : `int`
- `RefillOnEnraged` : `bool`
- `RageRepulsion` : [`ExplosionAsset`](#explosionasset)
- `VisionFollowTarget` : `bool`
- `VisionAngleRange` : `Range`
- `OverrideOnAim` : `bool`
- `AimFieldOfView` : `float` 瞄准视野
- `AimRayCount` : `int`
- `AimDistance` : `float` 瞄准距离
- `MaxFallHeight` : `float` 最大下坠高度
- `RandomFlip` : `bool`
- `FlipTimeRange` : `Range`
- `AdnGain` : `int`
- `RiseOnDeath` : `bool` 死亡时感染
- `RiseAsset` : `CustomAssetObject` 感染后转变的单位
- `TimeToRise` : `Range`
- `RemoveCorpse` : `bool`
- `ReloadType` : `ReloadType` 换弹类型
    * `Automatic`
    * `ShellByShell`
    * `Stamina`
- `StaminaRefillTimer` : `float` 补弹时间
- `ReloadTime` : `float` 换弹时间
- `DirectAim` : `bool` 直接瞄准
- `AimStopTime` : `float`
- `MagazineSize` : `int` 弹匣容量
- `RandomSeed` : `int`
- `BlockOpponents` : `bool` 阻挡
- `MaxOpponentsBlock` : `int` 最大阻挡数量
- `MoveTowardStaticTargets` : `bool`
- `RemoveTargetOnKill` : `bool`
- `Attachments` : `UnityDictionary<string, UnityEngine.GameObject>` 附件
    * `moving_attack` 移动攻击
        + `Gertrude Moving Attack`
        + `DrugLord Moving Attack`
        + `Priest Moving Attack`
        + `Lumberjack Moving Attack`
    * `shield_attack` 护盾攻击
        + `VirginShield`
    * `shield_effect` 护盾效果
        + `RepelWave`
    * `attach_laser` 附加激光
        + `LaserAttachment`
- `ThrowableObjects` : [`PhysicObjectAsset[]`](#physicobjectasset) 投掷物
- `ExplosionAssets` : [`ExplosionAsset[]`](#explosionasset) 自爆
- `WaitAnimTime` : `Range`
- `AlertIconOffset` : `UnityEngine.Vector2`
- `Voice` : [`Voice`](#voice) 语音类型
- `CharacterType` : [`CharacterType`](#charactertype)
- `Height` : `float` 身高
- `Hp` : `float`
- `Invincible` : `bool` 无敌
- `DamageMultipliers` : `UnityDictionary<DamageType, float>` 伤害倍率
- `Orientation` : `UnityEngine.Vector3`
- `WalkSpeed` : `float` 散步速度
- `RunSpeed` : `float` 跑步速度(`SprintSpeed = RunSpeed + 1`)
- `MoveOnStart` : `bool`
- `CanClimb` : `bool` 可爬行
- `CanStepOver` : `bool` 可跨过
- `DeathVelocityThreshold` : `float` 死亡速率阈值
- `JumpDeathVelocityThreshold` : `float` 跳跃死亡速率阈值
- `ColliderSkinWidth` : `float`
- `SlopeDetectionOffset` : `float`
- `StairDetectionHeightRatio` : `float` 楼梯高度判定
- `GeneralDirection` : `UnityEngine.Vector3`
- `DeviationAngle` : `float`
- `FieldOfView` : `float` 视野
- `Distance` : `float` 视野距离
- `RayCount` : `int`
- `DamageType` : [`DamageType`](#damagetype)
- `EnemyTag` : [`Tag`](#tag) 敌对标签
- `HitMultipleTargets` : `bool` 同时攻击多个目标
- `NextTargetsDamageMultiplier` : `float`
- `MaxTargets` : `int`
- `Damage` : `float`
- `AimRange` : `float` 瞄准射程
- `AttackRange` : `float` 攻击射程
- `DamageRange` : `float` 破坏射程
- `MovingAttackRange` : `float` 移动攻击射程
- `DamageFalloff` : `UnityEngine.AnimationCurve`
- `AttackFrequency` : `float`
- `SpriteCollection` : [`tk2dSpriteCollectionData`](Sprite.md#tk2dspritecollectiondata)
- `SpriteIndex` : `int`
- `AnimationLibrary` : [`tk2dSpriteAnimation`](Sprite.md#tk2dspriteanimation)
- `Animations` : `CharacterAnimationAsset`
    * `MeleeAnimations`
    * `ArmedAnimations`
    * `UnarmedAnimations`
    * `BulkyMeleeAnimations`
    * `DroneAnimations`
    * `TerminatorAnimations`
    * `ZombieAnimations`
    * `CrawlerAnimations`
    * `OverlordAnimations`
    * `TankAnimations`
- `Sounds` : `CharacterSoundAsset`
    * `DroneSounds`
    * `HumanSounds`
    * `ZombieSounds`
- `HierarchyName` : `string` 选择列表中的名称
- `Prefab` : `UnityEngine.Transform` 预制体
    * `Human`
    * `WorldEnemy`
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`UnityEngine.Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 人类名称

```json
{
  "Attitude": "Combative",
  "DangerRadius": 2.0,
  "AlertRadius": 5.0,
  "AlertRelayRadius": 4.0,
  "AlertRelayRatio": 0.5,
  "RelayAlertOverTime": false,
  "ParalyseDuration": 2.0,
  "AlertDuration": 4.0,
  "FleeDuration": 1.0,
  "ScaredDuration": 0.0,
  "Contaminable": false,
  "AllowMultipleAttackers": true,
  "GrabbedOnAttacked": false,
  "IgnoreDamages": false,
  "InvincibleOnAttack": false,
  "ResistScream": false,
  "FleeBeforeZombieExplode": true,
  "ReportAlert": true,
  "AllowRage": true,
  "FreezeOnRage": false,
  "RageDamageType": "None",
  "RageRefillTimer": 2.0,
  "RageRefillRate": 1,
  "RageHitCount": 3,
  "RefillOnEnraged": true,
  "RageRepulsion": "AstrogoliathEscape : ExplosionAsset",
  "VisionFollowTarget": false,
  "VisionAngleRange": {
    "MinValue": 0.0,
    "MaxValue": 0.0
  },
  "OverrideOnAim": false,
  "AimFieldOfView": 70.0,
  "AimRayCount": 3,
  "AimDistance": 10.0,
  "MaxFallHeight": 20.0,
  "RandomFlip": true,
  "FlipTimeRange": {
    "MinValue": 4.0,
    "MaxValue": 8.0
  },
  "AdnGain": 100,
  "RiseOnDeath": false,
  "RiseAsset": "Zombie : ZombieAsset",
  "TimeToRise": {
    "MinValue": 1.5,
    "MaxValue": 2.0
  },
  "RemoveCorpse": true,
  "ReloadType": "Automatic",
  "StaminaRefillTimer": 0.0,
  "ReloadTime": 0.5,
  "DirectAim": false,
  "AimStopTime": 2.0,
  "MagazineSize": 2147483647,
  "RandomSeed": 0,
  "BlockOpponents": false,
  "MaxOpponentsBlock": 0,
  "MoveTowardStaticTargets": true,
  "RemoveTargetOnKill": false,
  "Attachments": {
    "moving_attack": "Gertrude Moving Attack : UnityEngine.GameObject"
  },
  "ThrowableObjects": [
    "LaserShot : PhysicObjectAsset",
    "AstrogoliathLandingAttack : PhysicObjectAsset"
  ],
  "ExplosionAssets": [],
  "WaitAnimTime": {
    "MinValue": 4.0,
    "MaxValue": 8.0
  },
  "AlertIconOffset": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 4.1
  },
  "Voice": "MachineGunner",
  "CharacterType": "Astronaut",
  "Height": 2.8,
  "Hp": 5000.0,
  "Invincible": false,
  "DamageMultipliers": {
    "Spit": 1.5,
    "Contamination": 1.5
  },
  "Orientation": {
    "$type": "UnityEngine.Vector3, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 0.0,
    "z": 1.0
  },
  "WalkSpeed": 1.8,
  "RunSpeed": 5.0,
  "MoveOnStart": false,
  "CanClimb": false,
  "CanStepOver": true,
  "DeathVelocityThreshold": 300.0,
  "JumpDeathVelocityThreshold": 8.0,
  "ColliderSkinWidth": 0.075,
  "SlopeDetectionOffset": 0.15,
  "StairDetectionHeightRatio": 0.1,
  "GeneralDirection": {
    "$type": "UnityEngine.Vector3, UnityEngine.CoreModule",
    "x": 1.0,
    "y": 0.0,
    "z": 0.0
  },
  "DeviationAngle": 0.0,
  "FieldOfView": 5.0,
  "Distance": 22.0,
  "RayCount": 10,
  "DamageType": "Laser",
  "EnemyTag": "Zombie, WorldEnemy",
  "HitMultipleTargets": false,
  "NextTargetsDamageMultiplier": 0.5,
  "MaxTargets": 5,
  "Damage": 100.0,
  "AimRange": 15.0,
  "AttackRange": 15.0,
  "DamageRange": 2.0,
  "MovingAttackRange": 13.0,
  "DamageFalloff": "(0, 1) - (1, 1)",
  "AttackFrequency": 4.0,
  "SpriteCollection": "sprites_boss : tk2dSpriteCollectionData",
  "SpriteIndex": 1091,
  "AnimationLibrary": "anim_astrogoliath : tk2dSpriteAnimation",
  "Animations": "ArmedAnimations : CharacterAnimationAsset",
  "Sounds": "HumanSounds : CharacterSoundAsset",
  "HierarchyName": "Astrogoliath",
  "Prefab": "Human : UnityEngine.Transform",
  "Tag": "Character, Human",
  "Layer": "Human",
  "assetId": "",
  "name": "Astrogoliath",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID | Name                           | Prefab       | Animation                |
|:-------------------|:-------|:-------------------------------|:-------------|:-------------------------|
| `resources.assets` | `9540` | `Bouncer`                      | `Human`      | `anim_bouncer`           |
| `resources.assets` | `9541` | `Cheerleader`                  | `Human`      | `anim_cheerleader`       |
| `resources.assets` | `9542` | `FootballPlayer`               | `Human`      | `anim_football`          |
| `resources.assets` | `9543` | `Gunner`                       | `Human`      | `anim_gunner`            |
| `resources.assets` | `9544` | `GunnerTutorial`               | `Human`      | `anim_gunner`            |
| `resources.assets` | `9545` | `Melee`                        | `Human`      | `anim_melee`             |
| `resources.assets` | `9546` | `Shotgunner`                   | `Human`      | `anim_shotgun`           |
| `resources.assets` | `9547` | `BossChemist`                  | `Human`      | `anim_boss_chemist`      |
| `resources.assets` | `9548` | `BossChemistInvincible`        | `Human`      | `anim_boss_chemist`      |
| `resources.assets` | `9549` | `BossDrugLord`                 | `Human`      | `anim_boss_1`            |
| `resources.assets` | `9550` | `BossGertrude`                 | `Human`      | `anim_boss_2`            |
| `resources.assets` | `9551` | `BossGertrudeCinematic`        | `Human`      | `anim_boss_cinematic`    |
| `resources.assets` | `9552` | `CivilianDirector`             | `Human`      | `anim_director`          |
| `resources.assets` | `9553` | `CivilianDsk`                  | `Human`      | `anim_dsk`               |
| `resources.assets` | `9554` | `CivilianFemale`               | `Human`      | `anim_girl`              |
| `resources.assets` | `9555` | `CivilianFemaleBlack`          | `Human`      | `anim_girl_2`            |
| `resources.assets` | `9556` | `CivilianFemaleBlonde`         | `Human`      | `anim_girl_3`            |
| `resources.assets` | `9557` | `CivilianFemaleBlondeGarbage`  | `Human`      | `anim_girl_3_garbage`    |
| `resources.assets` | `9558` | `CivilianFemaleHostage`        | `Human`      | `anim_girl_hostage`      |
| `resources.assets` | `9559` | `CivilianFemaleNude`           | `Human`      | `anim_nudegirl`          |
| `resources.assets` | `9560` | `CivilianGranny`               | `Human`      | `anim_granny`            |
| `resources.assets` | `9561` | `CivilianMale`                 | `Human`      | `anim_civil`             |
| `resources.assets` | `9562` | `CivilianMaleBlack`            | `Human`      | `anim_civil_3`           |
| `resources.assets` | `9563` | `CivilianMaleBlackExplosive`   | `Human`      | `anim_civil_3_explosive` |
| `resources.assets` | `9564` | `CivilianMaleHostage`          | `Human`      | `anim_civil_hostage`     |
| `resources.assets` | `9565` | `CivilianMaleNude`             | `Human`      | `anim_nudeguy`           |
| `resources.assets` | `9566` | `CivilianMaleYoung`            | `Human`      | `anim_civil_2`           |
| `resources.assets` | `9567` | `CivilianPerchman`             | `Human`      | `anim_soundman`          |
| `resources.assets` | `9568` | `CivilianToiletGuy`            | `Human`      | `anim_toiletguy`         |
| `resources.assets` | `9569` | `Driver`                       | `Human`      | `anim_driver`            |
| `resources.assets` | `9570` | `Homeless`                     | `Human`      | `anim_homeless`          |
| `resources.assets` | `9571` | `CopRilfeman`                  | `Human`      | `anim_assault_rifle_cop` |
| `resources.assets` | `9572` | `Crs`                          | `Human`      | `anim_crs`               |
| `resources.assets` | `9573` | `Kamikaze`                     | `Human`      | `anim_kamikaze`          |
| `resources.assets` | `9574` | `Ninja`                        | `Human`      | `anim_ninja`             |
| `resources.assets` | `9575` | `Priest`                       | `Human`      | `anim_priest`            |
| `resources.assets` | `9576` | `Virgin`                       | `Human`      | `anim_virgin`            |
| `resources.assets` | `9577` | `MenInBlackBrawler`            | `Human`      | `anim_men_in_black_2`    |
| `resources.assets` | `9578` | `MenInBlackGunner`             | `Human`      | `anim_men_in_black`      |
| `resources.assets` | `9579` | `MenInBlackRilfeman`           | `Human`      | `anim_men_in_black_3`    |
| `resources.assets` | `9580` | `MenInBlackShotgunner`         | `Human`      | `anim_men_in_black_4`    |
| `resources.assets` | `9581` | `Astrogoliath`                 | `Human`      | `anim_astrogoliath`      |
| `resources.assets` | `9582` | `Astronaut`                    | `Human`      | `anim_astronaut`         |
| `resources.assets` | `9583` | `Bishop`                       | `Human`      | `anim_bishop`            |
| `resources.assets` | `9584` | `Spacegirl_1`                  | `Human`      | `anim_spacegirl_1`       |
| `resources.assets` | `9585` | `Spacegirl_2`                  | `Human`      | `anim_spacegirl_1`       |
| `resources.assets` | `9586` | `Spaceman_1`                   | `Human`      | `anim_spaceman_1`        |
| `resources.assets` | `9587` | `Spaceman_2`                   | `Human`      | `anim_spaceman_2`        |
| `resources.assets` | `9588` | `DoctorFemale`                 | `Human`      | `anim_doctor_girl_1`     |
| `resources.assets` | `9589` | `ScientistFemale 1`            | `Human`      | `anim_doctor_girl_1`     |
| `resources.assets` | `9590` | `ScientistFemale 2`            | `Human`      | `anim_doctor_girl_1`     |
| `resources.assets` | `9591` | `ScientistHazmat`              | `Human`      | `anim_hazmat`            |
| `resources.assets` | `9592` | `ScientistMale 1`              | `Human`      | `anim_doctor`            |
| `resources.assets` | `9593` | `ScientistMale 2`              | `Human`      | `anim_scientist_2`       |
| `resources.assets` | `9594` | `ScientistMale 3`              | `Human`      | `anim_scientist_3`       |
| `resources.assets` | `9595` | `Chemist`                      | `Human`      | `anim_chemist`           |
| `resources.assets` | `9596` | `ChemistChair`                 | `Human`      | `anim_chemist-chair`     |
| `resources.assets` | `9597` | `ChemistPlier`                 | `Human`      | `anim_chemist_pliers`    |
| `resources.assets` | `9598` | `Clown`                        | `Human`      | `anim_clown`             |
| `resources.assets` | `9599` | `CopWeak`                      | `WorldEnemy` | `anim_assault_rifle_cop` |
| `resources.assets` | `9600` | `DaftPunk1`                    | `Human`      | `anim_daft_punk_1`       |
| `resources.assets` | `9601` | `DaftPunk2`                    | `Human`      | `anim_daft_punk_2`       |
| `resources.assets` | `9602` | `Drone`                        | `Human`      | `anim_drone`             |
| `resources.assets` | `9603` | `DroneExterminator`            | `WorldEnemy` | `anim_drone`             |
| `resources.assets` | `9604` | `DroneInvincible`              | `Human`      | `anim_drone`             |
| `resources.assets` | `9605` | `DroneInvisible`               | `Human`      | `anim_drone`             |
| `resources.assets` | `9606` | `Preacher`                     | `Human`      | `anim_preacher`          |
| `resources.assets` | `9607` | `Terminator`                   | `WorldEnemy` | `anim_terminator`        |
| `resources.assets` | `9608` | `CivilianFemaleBlondeSurvivor` | `Human`      | `anim_girl_survivor_2`   |
| `resources.assets` | `9609` | `CivilianFemaleSurvivor`       | `Human`      | `anim_girl_survivor_1`   |
| `resources.assets` | `9610` | `CivilianSurvivor`             | `Human`      | `anim_civil`             |
| `resources.assets` | `9611` | `CivilianSurvivorMolotov`      | `Human`      | `anim_civil_2`           |
| `resources.assets` | `9612` | `CivilianSurvivorTorch`        | `Human`      | `anim_civil_2_torch`     |
| `resources.assets` | `9613` | `Lumberjack`                   | `Human`      | `anim_lumberjack`        |
| `resources.assets` | `9614` | `MachineGunner`                | `Human`      | `anim_machine_gunner`    |
| `resources.assets` | `9615` | `Sniper`                       | `Human`      | `anim_sniper`            |
| `resources.assets` | `9616` | `Sniper2`                      | `Human`      | `anim_sniper_2`          |
| `resources.assets` | `9617` | `SurvivorGunner`               | `Human`      | `anim_rick`              |
| `resources.assets` | `9618` | `SurvivorRilfeman`             | `Human`      | `anim_assault_survivor`  |
| `resources.assets` | `9619` | `SurvivorShotgunner`           | `Human`      | `anim_shotgun_survivor`  |
| `resources.assets` | `9620` | `SwordWomen`                   | `Human`      | `anim_swordwomen`        |
| `resources.assets` | `9621` | `Worker 1`                     | `Human`      | `anim_worker_1`          |
| `resources.assets` | `9622` | `Worker 2`                     | `Human`      | `anim_worker_2`          |
| `resources.assets` | `9623` | `Worker 3`                     | `Human`      | `anim_worker_3`          |
| `resources.assets` | `9624` | `Worker 4`                     | `Human`      | `anim_worker_4`          |
| `resources.assets` | `9628` | `FakeCrawler`                  | `Human`      | `anim_zombie_climber`    |
| `resources.assets` | `9629` | `FakeOverlord`                 | `Human`      | `anim_zombie_blocker`    |
| `resources.assets` | `9630` | `FakeTank`                     | `Human`      | `anim_zombie_tank`       |
| `resources.assets` | `9631` | `FakeZombie`                   | `Human`      | `anim_zombie_base`       |

# ZNT.Evolution.Core.Asset.SpawnPointAsset

> 生成点资源

## Spawn Create

> `*.spawn.json`

- `spawnableObjects` : `CustomAssetObject[]`
- `interval` : `float`
- `startDelay` : `float`
- `count` : `int`
- `infinite` : `bool`
- `active` : `bool`
- `moveOnStart` : `bool`
- `orientation` : `UnityEngine.Vector3`
- `HierarchyName` : `string` 选择列表中的名称
- `Prefab` : `UnityEngine.Transform` 预制体
    * `HumanSpawn`
    * `HumanSpawnInvisible`
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`UnityEngine.Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 生成点名称

```json
{
  "spawnableObjects": [
    "HumanTerminator : HumanAsset"
  ],
  "interval": 5.0,
  "startDelay": 2.0,
  "count": 0,
  "infinite": true,
  "active": true,
  "moveOnStart": true,
  "orientation": {
    "$type": "UnityEngine.Vector3, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 0.0,
    "z": 1.0
  },
  "defaultSpeed": "Walk",
  "HierarchyName": "Test Spawn",
  "Prefab": "HumanSpawn : UnityEngine.Transform",
  "Tag": 0,
  "Layer": "Default",
  "name": "TestSpawn",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID  | Prefab                 | Type                  |
|:-------------------|:--------|:-----------------------|:----------------------|
| `resources.assets` | `13723` | `LawnMowerSpawn`       | `SpawnPoint`          |
| `resources.assets` | `13729` | `HumanSpawnPortal`     | `CharacterSpawnPoint` |
| `resources.assets` | `13778` | `HumanSpawnVan`        | `CharacterSpawnPoint` |
| `resources.assets` | `13788` | `MovingContainerSpawn` | `SpawnPoint`          |
| `resources.assets` | `13790` | `ZombieSpawnInvisible` | `CharacterSpawnPoint` |
| `resources.assets` | `13804` | `HumanSpawnInvisible`  | `CharacterSpawnPoint` |
| `resources.assets` | `13806` | `Elevator1Spawn`       | `SpawnPoint`          |
| `resources.assets` | `13811` | `ZombieSpawn`          | `CharacterSpawnPoint` |
| `resources.assets` | `13819` | `HumanSpawn`           | `CharacterSpawnPoint` |
| `resources.assets` | `13827` | `ZombieSpawnDoor`      | `CharacterSpawnPoint` |
| `resources.assets` | `13847` | `Elevator2Spawn`       | `SpawnPoint`          |
| `resources.assets` | `13860` | `Subway2Spawn`         | `SpawnPoint`          |
| `resources.assets` | `13869` | `ZombieSpawnReborn`    | `CharacterSpawnPoint` |
| `resources.assets` | `13919` | `SubwaySpawn`          | `SpawnPoint`          |
| `resources.assets` | `13928` | `ZombieSpawnCemetary`  | `CharacterSpawnPoint` |
