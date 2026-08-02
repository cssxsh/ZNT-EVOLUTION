# ZNT-EVOLUTION

## 项目环境变量

* `ZNTGamePath` 游戏文件目录
* `ZNTModsPath` 游戏模组目录

## 游戏文件结构

```text
Zombie Night Terror
├───BepInEx
│   ├───plugins
│   │   └───ZNT-Evolution-Core
│   │       ├───ZipStorer.dll
|   |       └───ZNT-Evolution-Core.dll
│   └───LogOutput.log
└──znt_Data
    └───Mods
        ├───Assets Of Sound
        └───<...>
```

MOD 放入 `znt_Data/Mods` 目录下

### 存档位置

```
%LocalAppData%Low\NoClip\Zombie Night Terror
```

## 插件文件结构

### 元数据

> `metadata.json`

插件基础数据，必不可少

- `Id` - 插件唯一标识符，不可重复
- `Name` - 插件名称
- `Version` - 插件版本
- `Dependencies` - 插件依赖

```json
{
  "Id": "io.github.cssxsh.znt.mod.example",
  "Name": "Example Mod",
  "Version": "0.1.0.0",
  "Dependencies": {
    "xyz.cssxsh.znt.evolution.core": "0.0.0.0"
  }
}
```

### FMOD

> `*.strings.bank`, `*.bank`

导入新的音频  
参考 [FMOD.md - 制作 bank 文件](docs/FMOD.md#制作-bank-文件)

### Sprite

#### `UnityEngine.Texture2D`

> `*.tga`, `*.png`, `*.exr`

导入新的贴图，命名规范 `sprites_xxx_atlas`

#### `UnityEngine.Material`

> `*.material.merge.json`

导入新的纹理，命名规范 `sprites_xxx_mat`  
参考 [Sprite.md - Material Merge](docs/Sprite.md#material-merge)

#### `tk2dSpriteCollectionData`

> `*.sprite.info.json`, `*.sprite.merge.json`

导入新的精灵图集，命名规范 `sprites_xxx`  
参考 [Sprite.md - Sprite Create](docs/Sprite.md#sprite-create)

#### `tk2dSpriteAnimation`

> `*.animation.json`

导入新的精灵动画，命名规范 `anim_xxx`  
参考 [Sprite.md - Animation Create](docs/Sprite.md#animation-create)

#### `ZNT.Evolution.Core.Asset.CustomVisualEffect`

> `*.visual.json`

导入新的特效  
参考 [Sprite.md - Visual Create](docs/Sprite.md#visual-create)

### Asset

#### `ExplosionAsset`

> `*.explosion.json`

导入新的爆炸，命名规范 `xxxExplosion` / `xxxRepulse`  
参考 [Asset.md - Explosion Create](docs/Asset.md#explosion-create)

#### `DecorAsset`

> `*.decor.json`

导入新的物件  
参考 [Asset.md - Decor Create](docs/Asset.md#decor-create)

#### `BreakablePropAsset`

> `*.breakable.json`

导入新的可破坏道具  
参考 [Asset.md - Breakable Create](docs/Asset.md#breakable-create)

#### `TriggerAsset`

> `*.trigger.json`

导入新的开关  
参考 [Asset.md - Trigger Create](docs/Asset.md#trigger-create)

#### `MovingObjectAsset`

> `*.moving.json`

导入新的动体  
参考 [Asset.md - Moving Create](docs/Asset.md#moving-create)

#### `PhysicObjectAsset`

> `*.physic.json`

导入新的物体  
参考 [Asset.md - Physic Create](docs/Asset.md#physic-create)

#### `SentryGunAsset`

> `*.sentry.json`

导入新的炮台  
参考 [Asset.md - Sentry Create](docs/Asset.md#sentry-create)

#### `HumanAsset`

> `*.human.json`

导入新的人类  
参考 [Asset.md - Human Create](docs/Asset.md#human-create)

#### `ZNT.Evolution.Core.Asset.SpawnPointAsset`

> `*.spawn.json`

导入新的生成点  
参考 [Asset.md - Spawn Create](docs/Asset.md#spawn-create)

### Element

#### `Rotorz.Tile.OrientedBrush`

> `*.brush.info.json`, `*.brush.merge.json`

#### `UnityEngine.Sprite`

> `*.preview.png`, `*.preview.info.json`

#### `LevelElement`

> `*.element.json`