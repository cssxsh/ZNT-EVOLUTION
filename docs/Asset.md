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
| `0x03` | `0x00000008` |         `3`         |
| `0x04` | `0x00000010` |       `Water`       |
| `0x05` | `0x00000020` |        `UI`         |
| `0x06` | `0x00000040` |         `6`         |
| `0x07` | `0x00000080` |         `7`         |
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

# ExplosionAsset

> 爆炸资源

## Explosion Create

> `*.explosion.json`

- `Type` : `Normal`/`Fragmentation`/`Continuous` 起爆类型
- `DamageType` : [`DamageType`](#damagetype) 伤害类型
- `DetectedLayers` : [`UnityEngine.LayerMask`](#layer) 作用图层
- `OriginOffset` : `UnityEngine.Vector3`
- `autoExplode` : `bool` 自动起爆
- `Delay` : `float` 起爆延迟
- `DamageRadius` : `float` 伤害半径
- `TileRadius` : `float`
- `ApplyDamageOn` : [`Tag`](#tag) 伤害作用于
- `Damage` : `float` 伤害值
- `DamageDistanceFallof` : `AnimationCurve` 伤害衰变
- `ApplyForceOn` : [`Tag`](#tag) 推力作用于
- `ForceMode` : `Force`/`Impulse`
- `Force` : `float` 推力大小
- `ForceMultipliers` : `UnityDictionary<Layer, float>` 推力倍率
- `ForceDistanceFallof` : `AnimationCurve` 推力衰变
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
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`Layer`](#layer) 图层
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
  "DamageDistanceFallof": {
    "keys": [
      {
        "time": 0.299999982,
        "value": 1.0,
        "inTangent": -0.00412149727,
        "outTangent": -0.00412149727,
        "inWeight": 0.333333343,
        "outWeight": 0.333333343,
        "weightedMode": "None",
        "tangentMode": 0
      },
      {
        "time": 1.0,
        "value": 0.25,
        "inTangent": -3.20778656,
        "outTangent": -3.20778656,
        "inWeight": 0.333333343,
        "outWeight": 0.333333343,
        "weightedMode": "None",
        "tangentMode": 0
      }
    ],
    "length": 2,
    "preWrapMode": "ClampForever",
    "postWrapMode": "ClampForever"
  },
  "ApplyForceOn": "Human, Zombie, WorldEnemy",
  "ForceMode": "Impulse",
  "Force": 9.0,
  "ForceMultipliers": {},
  "ForceDistanceFallof": {
    "keys": [
      {
        "time": 0.5,
        "value": 1.0,
        "inTangent": -0.00412149727,
        "outTangent": -0.00412149727,
        "inWeight": 0.333333343,
        "outWeight": 0.333333343,
        "weightedMode": "None",
        "tangentMode": 0
      },
      {
        "time": 1.0,
        "value": 0.5,
        "inTangent": -2.41442442,
        "outTangent": -2.41442442,
        "inWeight": 0.333333343,
        "outWeight": 0.333333343,
        "weightedMode": "None",
        "tangentMode": 0
      }
    ],
    "length": 2,
    "preWrapMode": "ClampForever",
    "postWrapMode": "ClampForever"
  },
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
- `playSounds` : `bool` 释放播放声音
- `sound` : [`FMODAsset`](FMODAsset.md)
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`Layer`](#layer) 图层
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
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`Layer`](#layer) 图层
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

# TriggerAsset

> 开关资源

## Trigger Create

> `*.trigger.json`

- `type` : `Receiver`/`Trap`/`Trigger`/`Both`
- `OverrideAnimations` : `bool`
- `PlaySounds` : `bool`
- `DetectedLayers` : [`UnityEngine.LayerMask`](#layer) 作用图层
- `CheckTags` : `bool`
- `DetectedTags` : [`Tag`](#tag) 作用标签
- `resizeMode` : `None`/`Horizontal`/`Vertical`/`Both`
- `clampMethod` : `None`/`Relative`/`Absolute`
- `minSize` : `UnityEngine.Vector2`
- `roundToNeareset` : `bool`
- `RendererColor` : `UnityEngine.Color`
- `RenderMode` : `Tiled`/`SlicedAndTiled`/`Sliced`/`None`
- `PixelsPerMeter` : `int`
- `EffectPrefab` : `ResizableParticleSystem`
- `Animation` : [`tk2dSpriteAnimation`](Sprite.md#tk2dspriteanimation)
- `ActiveAnimation` : `string`
- `InactiveAnimation` : `string`
- `ActivateAnimation` : `string`
- `DeactivateAnimation` : `string`
- `ActivateEvent` : [`FMODAsset`](FMODAsset.md)
- `EventFollowObject` : `bool`
- `Type` : [`DamageType`](#damagetype) 伤害类型
- `Mode` : `Damage`/`Kill`
- `KillDelay` : `float`
- `Damage` : `float`
- `DamageRate` : `float`
- `Name` : `string` 菜单栏中的名字
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`Layer`](#layer) 图层
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
- `ColliderType` : `None`/`Box`/`Circle`
- `ColliderIsTrigger` : `bool`
- `ColliderMaterial` : `UnityEngine.PhysicsMaterial2D`
- `ColliderOffset` : `UnityEngine.Vector2`
- `BoxSize` : `UnityEngine.Vector2`
- `CircleRadius` : `float`
- `ApplyDamage` : `bool`
- `DamageShape` : `Box`/`Circle`
- `DamageOffset` : `UnityEngine.Vector2`
- `DamageSize` : `UnityEngine.Vector2`
- `DamageRadius` : `float`
- `DamageType` : [`DamageType`](#damagetype) 伤害类型
- `DamageAppliedTo` : [`UnityEngine.LayerMask`](#layer) 伤害作用图层
- `Speed` : `float`
- `MoveOnStart` : `bool`
- `HierarchyName` : `string`
- `Prefab` : `UnityEngine.Transform` 预制体
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`Layer`](#layer) 图层
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
- `ExplodeOn` : `Wall`/`Ground`/`Ceiling`/`Target`/`Zombie`/`Climber`/`Blocker`/`Tank`/`IgnoreHuman` 爆炸作用类型
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
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`Layer`](#layer) 图层
- `assetId` : `string`
- `name` : `string` 物体名称

当 `ignoreCollisionDuration` 开启会检查是否卡住（速度为零时爆炸）  
当 `ExplodeOn` 包含 `Target` 会对角色图层产生碰撞（使用默认图层，反之使用忽略角色的图层）  
`ExplodeOn` 的 `Zombie`/`Climber`/`Blocker`/`Tank`/`IgnoreHuman` 为代码扩展

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

# SentryGunAsset

> 炮台资源

## Sentry Create

> `*.sentry.json`

- `Hp` : `float`
- `Invincible` : `bool`
- `DamageMultipliers` : `UnityDictionary<DamageType, float>` 伤害倍率
- `AllDetection` : `DetectionAsset` `SentryAllDetection`
- `HumanDetection` : `DetectionAsset` `SentryHumanDetection`
- `ZombieDetection` : `DetectionAsset` `SentryZombieDetection`
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
- `SpriteIndex`: `int`
- `Animation` - [`tk2dSpriteAnimation`](Sprite.md#tk2dspriteanimation)
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
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`Layer`](#layer) 图层
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

# HumanAsset

> 人类资源

## Human Create

> `*.human.json`

- `Attitude` : `Combative`/`Neutral`/`Fearfull`/`Ignore`
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
- `ReloadType` : `Automatic`/`ShellByShell`/`Stamina` 换弹类型
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
- `DamageFalloff` : `AnimationCurve`
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
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`Layer`](#layer) 图层
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
  "DamageFalloff": {
    "keys": [
      {
        "time": 0.0,
        "value": 1.0,
        "inTangent": 0.0,
        "outTangent": 0.0,
        "inWeight": 0.333333343,
        "outWeight": 0.333333343,
        "weightedMode": "None",
        "tangentMode": 0
      },
      {
        "time": 1.0,
        "value": 1.0,
        "inTangent": 0.0,
        "outTangent": 0.0,
        "inWeight": 0.333333343,
        "outWeight": 0.333333343,
        "weightedMode": "None",
        "tangentMode": 0
      }
    ],
    "length": 2,
    "preWrapMode": "ClampForever",
    "postWrapMode": "ClampForever"
  },
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

# ZNT.Evolution.Core.Asset.SpawnPointAsset

> 生成点资源

## Spawn Create

> `*.spawn.json`

- `spawnableObjects` - `CustomAssetObject[]`
- `interval` : `float`
- `startDelay` : `float`
- `count` : `int`
- `infinite` : `bool`
- `active` : `bool`
- `moveOnStart` : `bool`
- `orientation` : `UnityEngine.Vector3`
- `HierarchyName` : `string` 选择列表中的名称
- `Prefab` : `UnityEngine.Transform` 预制体
- `Tag` : [`Tag`](#tag) 标签
- `Layer` : [`Layer`](#layer) 图层
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