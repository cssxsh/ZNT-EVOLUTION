# Rotorz.Tile.Brush

> 主要为 Rotorz.Tile.OrientedBrush

## 结构

- `Rotorz.Tile.OrientedBrush`
    * name
    * Orientations `IList<BrushOrientation>`
    * DefaultOrientation `Rotorz.Tile.BrushOrientation`
    * ...

- `Rotorz.Tile.BrushOrientation`
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

### Vision

> RayConeDetection

| name         | Frequency | Layers                                                           | WithTags                             | WithoutTags    | ObstaclesLayers                                    | ObstaclesTag                   |
|:-------------|:----------|:-----------------------------------------------------------------|:-------------------------------------|:---------------|:---------------------------------------------------|:-------------------------------|
| `Human`      | `10`      | `Zombie, Crate, Gameplay, Stairs Top, Block Humans, World Enemy` | `Zombie, WorldEnemy`                 | `IgnoreAttack` |                                                    |                                |
| `WorldEnemy` | `10`      | `Zombie, Human, Gameplay, Stairs Top, Block Humans`              | `Human, Zombie`                      |                |                                                    |                                |
| `Zombie`     | `20`      | `Human, Prop, World Enemy`                                       | `Door, Human, Breakable, WorldEnemy` | `CannotAttack` | `Crate, Gameplay, Stairs Top, Spit, Block Zombies` | `Indestructible, Destructible` |
| `SentryGun`  | `60`      | `3, 6, 7, Zombie, Block Explosion`                               | `Zombie`                             |                | `Gameplay, Stairs Top`                             | `Indestructible, Destructible` |

### Attacker

> LineDetection

| name         | Layers                     | WithTags                       | WithoutTags    | BlockingView                                |
|:-------------|:---------------------------|:-------------------------------|:---------------|:--------------------------------------------|
| `Human`      | `Zombie, World Enemy`      | `Zombie, WorldEnemy`           |                | `Crate, Gameplay, Stairs Top, Block Humans` |
| `WorldEnemy` | `Zombie, Human`            | `Human, Zombie`                |                | `Gameplay, Stairs Top, Block Humans`        |
| `Zombie`     | `Human, Prop, World Enemy` | `Human, Breakable, WorldEnemy` | `CannotAttack` | `0`                                         |

### AlertZone

> SphereDetection

| name         | Frequency | Layers                | WithTags             | WithoutTags    | ObstaclesLayers                            | ObstaclesTag                         |
|:-------------|:----------|:----------------------|:---------------------|:---------------|:-------------------------------------------|:-------------------------------------|
| `Human`      | `10`      | `Zombie, World Enemy` | `Zombie, WorldEnemy` | `IgnoreAttack` | `Gameplay, Stairs Top, Spit, Block Humans` | `Indestructible, Destructible`       |
| `WorldEnemy` | `10`      | `Zombie, Human`       | `Human, Zombie`      |                | `Gameplay, Prop, Stairs Top, Block Humans` | `Door, Indestructible, Destructible` |

### DangerZone

> SphereDetection

| name         | Frequency | Layers                | WithTags             | WithoutTags    | ObstaclesLayers                            | ObstaclesTag                         |
|:-------------|:----------|:----------------------|:---------------------|:---------------|:-------------------------------------------|:-------------------------------------|
| `Human`      | `10`      | `Zombie, World Enemy` | `Zombie, WorldEnemy` | `IgnoreAttack` | `Gameplay, Stairs Top, Spit, Block Humans` | `Indestructible, Destructible`       |
| `WorldEnemy` | `10`      | `Zombie, Human`       | `Human, Zombie`      |                | `Gameplay, Prop, Stairs Top, Block Humans` | `Door, Indestructible, Destructible` |