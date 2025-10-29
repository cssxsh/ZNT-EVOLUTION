# Rotorz.Tile.Brush

> 主要为 Rotorz.Tile.OrientedBrush

## 结构

- Rotorz.Tile.OrientedBrush
    * name
    * Orientations `IList<BrushOrientation>`
    * DefaultOrientation `Rotorz.Tile.BrushOrientation`
    * ...

- Rotorz.Tile.BrushOrientation
    * name
    * _variations `UnityEngine.Object[]`
    * ...

## 原型

`_variations[0]` -> `znt_Data/data.unity3d` - `resources.assets`

- `Human`: `2533`
- `CircularExplosion`: `2609`
- `CircularEnemyExplosion`: `2627`
- `ActivableDecor`: `2664`
- `Zombie`: `2672`
- `MineTrap`: `2676`
- `PhysicObject`: `4088`
- `MovingObject`: `4106`
- `SentryGun`: `4359`

## Vision

> RayConeDetection

| name         | Layers                                                           | WithTags                             | WithoutTags    |
|:-------------|:-----------------------------------------------------------------|:-------------------------------------|:---------------|
| `Human`      | `Zombie, Crate, Gameplay, Stairs Top, Block Humans, World Enemy` | `Zombie, WorldEnemy`                 | `IgnoreAttack` |
| `WorldEnemy` | `Zombie, Human, Gameplay, Stairs Top, Block Humans`              | `Human, Zombie`                      |                |
| `Zombie`     | `Human, Prop, World Enemy`                                       | `Door, Human, Breakable, WorldEnemy` | `CannotAttack` |

## AlertZone

> SphereDetection

| name         | Layers                | WithTags             | WithoutTags    |
|:-------------|:----------------------|:---------------------|:---------------|
| `Human`      | `Zombie, World Enemy` | `Zombie, WorldEnemy` | `IgnoreAttack` |
| `WorldEnemy` | `Zombie, Human`       | `Human, Zombie`      |                |

## DangerZone

> SphereDetection

| name         | Layers                     | WithTags                             | WithoutTags    | 
|:-------------|:---------------------------|:-------------------------------------|:---------------|
| `Human`      | `Zombie, World Enemy`      | `Zombie, WorldEnemy`                 | `IgnoreAttack` |
| `WorldEnemy` | `Zombie, Human`            | `Human, Zombie`                      |                |
| `Zombie`     | `Human, Prop, World Enemy` | `Door, Human, Breakable, WorldEnemy` | `CannotAttack` |