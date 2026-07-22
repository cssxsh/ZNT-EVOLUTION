# tk2dSpriteCollectionData

> 精灵图集合类，将原图分割为一个一个小块，以便之后作为动画帧使用

| Assets             | PathID  | Name                              | Shader                                          |
|:-------------------|:--------|:----------------------------------|:------------------------------------------------|
| `resources.assets` | `13753` | `sprites_assault_rifle`           | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `13761` | `sprites_background_buildings`    | `ZNT/Common/Flat Textured AlphaTest`            |
| `resources.assets` | `13762` | `sprites_front_background`        | `ZNT/Common/Flat Textured AlphaTest`            |
| `resources.assets` | `13773` | `sprites_traps`                   | `ZNT/Environments/Environments Base`            |
| `resources.assets` | `13792` | `sprites_ui`                      | `ZNT/Common/Flat Textured AlphaTest`            |
| `resources.assets` | `13814` | `sprites_props`                   | `ZNT/Environments/Environments Base`            |
| `resources.assets` | `13816` | `sprites_gameplay_exterior`       | `ZNT/Environments/Environments Base`            |
| `resources.assets` | `13830` | `sprites_melee`                   | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `13836` | `sprites_editor`                  | `ZNT/Common/Flat Textured AlphaTest`            |
| `resources.assets` | `13839` | `sprites_empty`                   | `tk2d/BlendVertexColor`                         |
| `resources.assets` | `13850` | `sprites_gunner`                  | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `13852` | `sprites_decors_exterior`         | `ZNT/Common/Flat Textured AlphaTest`            |
| `resources.assets` | `13864` | `sprites_civils`                  | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `13881` | `sprites_flat`                    | `ZNT/Common/Flat Textured AlphaTest`            |
| `resources.assets` | `13897` | `sprites_flames`                  | `ZNT/Effects/Flame Sliced Pixel`                |
| `resources.assets` | `13899` | `sprites_shotgun`                 | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `13901` | `sprites_buildings_front_details` | `ZNT/Environments/Environments Base`            |
| `resources.assets` | `13904` | `sprites_gameplay_interior`       | `ZNT/Environments/Environments Base`            |
| `resources.assets` | `13925` | `sprites_blood`                   | `ZNT/Common/Flat Textured AlphaTest`            |
| `resources.assets` | `15314` | `sprites_monolithEffects`         | `ZNT/Effects/MonolythCracks`                    |
| `resources.assets` | `15408` | `sprites_drones_sentry`           | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `15536` | `sprites_haze`                    | `ZNT/Effects/Haze`                              |
| `resources.assets` | `15842` | `sprites_middleground`            | `ZNT/Common/Flat Textured AlphaTest`            |
| `resources.assets` | `15972` | `sprites_bosschemistfinal`        | `ZNT/Environments/Environments Base`            |
| `resources.assets` | `16125` | `sprites_scientists`              | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `16203` | `sprites_steam`                   | `ZNT/Effects/Steam`                             |
| `resources.assets` | `16296` | `sprites_zombie`                  | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `16346` | `sprites_boss`                    | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `16493` | `sprites_mountains`               | `ZNT/Common/Animated Flat Textured Transparent` |
| `resources.assets` | `16631` | `Sprites_background_campaign`     | `ZNT/Common/Animated Flat Textured Cutout`      |
| `resources.assets` | `16646` | `sprites_stereogram`              | `ZNT/Environments/Environments Base`            |
| `resources.assets` | `16746` | `sprites_rails`                   | `ZNT/Common/Animated Flat Textured Cutout`      |
| `resources.assets` | `16747` | `sprites_animated_backgrounds`    | `ZNT/Common/Animated Flat Textured Cutout`      |
| `resources.assets` | `16748` | `sprites_front_rail`              | `ZNT/Common/Animated Flat Textured Cutout`      |
| `resources.assets` | `17042` | `sprites_monolith`                | `ZNT/Effects/MonolythParts`                     |
| `resources.assets` | `17161` | `sprite_rippedZombie`             | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `17163` | `sprite_wind`                     | `ZNT/Effects/Wind`                              |
| `resources.assets` | `17400` | `sprites_sky`                     | `ZNT/Misc/GenericBlend`                         |
| `resources.assets` | `17431` | `sprites_bossbrain`               | `ZNT/Environments/Environments Base`            |
| `resources.assets` | `17642` | `sprites_background_campaign_far` | `ZNT/Common/Animated Flat Textured Cutout`      |
| `resources.assets` | `17690` | `sprites_lasershot`               | `ZNT/Misc/GenericBlend`                         |
| `resources.assets` | `18069` | `sprites_brainlaser`              | `ZNT/Common/Tex_Col_VCol_Blend`                 |
| `resources.assets` | `18131` | `sprites_radioactivity`           | `ZNT/Effects/Radioactivity`                     |
| `resources.assets` | `18243` | `sprites_civils_2`                | `ZNT/Characters/Characters Base`                |
| `resources.assets` | `18299` | `sprites_water`                   | `ZNT/Effects/Water Lit`                         |

## 结构

- tk2dSpriteCollectionData
  * name
  * spriteDefinitions `tk2dSpriteDefinition[]`
  * ...

- tk2dSpriteDefinition
  * name
  * <u>ref material</u> `UnityEngine.Material`
  * attachPoints `tk2dSpriteDefinition.AttachPoint[]` 附着点，同步组件坐标
  * ...

- tk2dSpriteDefinition.AttachPoint
  * name 组件名称
  * position `UnityEngine.Vector3`
  * angle