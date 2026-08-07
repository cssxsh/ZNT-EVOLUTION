# UnityEngine.Texture2D

> 贴图

## Load Image

> `*.tga`, `*.png`, `*.exr`

图片尺寸应为 `2` 的幂，例如 `1024`

# UnityEngine.Material

> 纹理

## Material Merge

> `*.material.merge.json`

- `Source` : `UnityEngine.Material` 原型纹理，以其为蓝本，复制其参数
- `Name` : `string` 纹理名称
- `Shader` : `UnityEngine.Shader` 着色器
- `Textures` : `Dictionary<string, UnityEngine.Texture>` 纹理的贴图，需要和 `Shader` 配合使用
    * `_MainTex` 必选，纹理主贴图
    * `_RimTex` 可选，纹理描边贴图，没有制作时请设置为 `null`
- `Floats` : `Dictionary<string, float>` 纹理的参数，需要和 `Shader` 配合使用
- `Colors` : `Dictionary<string, UnityEngine.Color>` 纹理的颜色，需要和 `Shader` 配合使用
    * `_Color` 纹理的主颜色，复用 `Source` 效果时请不要设置

```json
{
  "Source": "sprites_assault_rifle_mat : UnityEngine.Material",
  "Name": "sprites_human_terminator_mat",
  "Shader": "ZNT/Characters/Characters Base",
  "Textures": {
    "_MainTex": "sprites_human_terminator_atlas : UnityEngine.Texture2D",
    "_RimTex": null
  },
  "Floats": {
    "_UseFlip": 0.0
  },
  "Colors": {
    "_Color": {
      "r": 1.0,
      "g": 1.0,
      "b": 1.0,
      "a": 1.0
    }
  }
}
```

## Shader

> 着色器

| Assets                              | PathID    | Name                                                 |
|:------------------------------------|:----------|:-----------------------------------------------------|
| `Resources/unity default resources` | `17`      | `Hidden/InternalErrorShader`                         |
| `Resources/unity default resources` | `68`      | `Hidden/InternalClear`                               |
| `Resources/unity default resources` | `69`      | `Hidden/Internal-Colored`                            |
| `Resources/unity default resources` | `10101`   | `GUI/Text Shader`                                    |
| `Resources/unity default resources` | `10755`   | `Hidden/FrameDebuggerRenderTargetDisplay`            |
| `Resources/unity_builtin_extra`     | `6`       | `Legacy Shaders/VertexLit`                           |
| `Resources/unity_builtin_extra`     | `7`       | `Legacy Shaders/Diffuse`                             |
| `Resources/unity_builtin_extra`     | `19`      | `Hidden/Internal-StencilWrite`                       |
| `Resources/unity_builtin_extra`     | `62`      | `Hidden/Internal-DepthNormalsTexture`                |
| `Resources/unity_builtin_extra`     | `65`      | `Hidden/Internal-CombineDepthNormals`                |
| `Resources/unity_builtin_extra`     | `66`      | `Hidden/BlitCopy`                                    |
| `Resources/unity_builtin_extra`     | `67`      | `Hidden/BlitCopyDepth`                               |
| `Resources/unity_builtin_extra`     | `68`      | `Hidden/ConvertTexture`                              |
| `Resources/unity_builtin_extra`     | `102`     | `Hidden/Internal-Flare`                              |
| `Resources/unity_builtin_extra`     | `105`     | `Hidden/Internal-Halo`                               |
| `Resources/unity_builtin_extra`     | `107`     | `Hidden/BlitCopyWithDepth`                           |
| `Resources/unity_builtin_extra`     | `109`     | `Hidden/BlitToDepth`                                 |
| `Resources/unity_builtin_extra`     | `110`     | `Hidden/BlitToDepth_MSAA`                            |
| `Resources/unity_builtin_extra`     | `9000`    | `Hidden/Internal-GUITextureClip`                     |
| `Resources/unity_builtin_extra`     | `9001`    | `Hidden/Internal-GUITextureClipText`                 |
| `Resources/unity_builtin_extra`     | `9002`    | `Hidden/Internal-GUITexture`                         |
| `Resources/unity_builtin_extra`     | `9003`    | `Hidden/Internal-GUITextureBlit`                     |
| `Resources/unity_builtin_extra`     | `9004`    | `Hidden/Internal-GUIRoundedRect`                     |
| `Resources/unity_builtin_extra`     | `9005`    | `Hidden/Internal-UIRDefault`                         |
| `Resources/unity_builtin_extra`     | `9006`    | `Hidden/Internal-UIRAtlasBlitCopy`                   |
| `Resources/unity_builtin_extra`     | `10757`   | `Sprites/Mask`                                       |
| `Resources/unity_builtin_extra`     | `10770`   | `UI/Default`                                         |
| `Resources/unity_builtin_extra`     | `10782`   | `UI/Default Font`                                    |
| `Resources/unity_builtin_extra`     | `15104`   | `Hidden/CubeBlur`                                    |
| `Resources/unity_builtin_extra`     | `15105`   | `Hidden/CubeCopy`                                    |
| `Resources/unity_builtin_extra`     | `15106`   | `Hidden/CubeBlend`                                   |
| `Resources/unity_builtin_extra`     | `15304`   | `Hidden/VR/BlitTexArraySlice`                        |
| `Resources/unity_builtin_extra`     | `15308`   | `Hidden/Internal-ODSWorldTexture`                    |
| `Resources/unity_builtin_extra`     | `15309`   | `Hidden/Internal-CubemapToEquirect`                  |
| `Resources/unity_builtin_extra`     | `15312`   | `Hidden/VR/BlitFromTex2DToTexArraySlice`             |
| `Resources/unity_builtin_extra`     | `16000`   | `Hidden/VideoComposite`                              |
| `Resources/unity_builtin_extra`     | `16001`   | `Hidden/VideoDecode`                                 |
| `Resources/unity_builtin_extra`     | `17000`   | `Hidden/Compositing`                                 |
| `Resources/unity_builtin_extra`     | `4800000` | `UI/Default`                                         |
| `globalgamemanagers.assets`         | `4`       | `Sprites/Default`                                    |
| `globalgamemanagers.assets`         | `5`       | `UI/Default`                                         |
| `sharedassets0.assets`              | `7`       | `TextMeshPro/Mobile/Distance Field`                  |
| `sharedassets0.assets`              | `8`       | `TextMeshPro/Distance Field`                         |
| `sharedassets1.assets`              | `2`       | `ZNT/PostEffects/ColorBlindRemapper`                 |
| `sharedassets2.assets`              | `14`      | `Hidden/ContrastComposite`                           |
| `sharedassets2.assets`              | `15`      | `ZNT/UI/UI Animated`                                 |
| `sharedassets2.assets`              | `16`      | `Hidden/SeparableBlur`                               |
| `sharedassets3.assets`              | `32`      | `Hidden/BrightPassFilter2`                           |
| `sharedassets3.assets`              | `33`      | `ZNT/PostEffects/GlobalRimLights`                    |
| `sharedassets3.assets`              | `34`      | `Hidden/Noise Shader YUV`                            |
| `sharedassets3.assets`              | `35`      | `ZNT/Misc/Line Color Blended`                        |
| `sharedassets3.assets`              | `36`      | `ZNT/Common/Tex_Blend`                               |
| `sharedassets3.assets`              | `37`      | `Hidden/LensFlareCreate`                             |
| `sharedassets3.assets`              | `38`      | `Hidden/BlurEffectConeTap`                           |
| `sharedassets3.assets`              | `39`      | `Hidden/Noise Shader RGB`                            |
| `sharedassets3.assets`              | `40`      | `Hidden/BlendForBloom`                               |
| `sharedassets3.assets`              | `41`      | `ZNT/PostEffects/VhsEffect`                          |
| `sharedassets3.assets`              | `42`      | `Hidden/BlurAndFlares`                               |
| `sharedassets13.assets`             | `8`       | `Legacy Shaders/Transparent/VertexLit`               |
| `sharedassets13.assets`             | `9`       | `Sprites/Diffuse`                                    |
| `sharedassets15.assets`             | `5`       | `TextMeshPro/Sprite`                                 |
| `sharedassets21.assets`             | `22`      | `ZNT/UI/Square Splat`                                |
| `sharedassets21.assets`             | `23`      | `ZNT/UI/Blood Splat`                                 |
| `resources.assets`                  | `436`     | `Legacy Shaders/Transparent/Cutout/VertexLit`        |
| `resources.assets`                  | `437`     | `Legacy Shaders/Particles/Additive`                  |
| `resources.assets`                  | `438`     | `Legacy Shaders/Particles/Alpha Blended`             |
| `resources.assets`                  | `439`     | `Legacy Shaders/Particles/Alpha Blended Premultiply` |
| `resources.assets`                  | `440`     | `Unlit/Color`                                        |
| `resources.assets`                  | `441`     | `ZNT/Effects/MonolythCracks`                         |
| `resources.assets`                  | `442`     | `ZNT/Effects/RepelWave`                              |
| `resources.assets`                  | `443`     | `ZNT/Effects/Steam`                                  |
| `resources.assets`                  | `444`     | `ZNT/Effects/MonolythCracksLit`                      |
| `resources.assets`                  | `445`     | `ZNT/Common/VCol_Geometry`                           |
| `resources.assets`                  | `446`     | `ZNT/Common/Tex_Col_VCol_Blend`                      |
| `resources.assets`                  | `447`     | `ZNT/Common/Flat Colored Geometry`                   |
| `resources.assets`                  | `448`     | `ZNT/Effects/Fire`                                   |
| `resources.assets`                  | `449`     | `ZNT/Environments/Environments Cables`               |
| `resources.assets`                  | `450`     | `ZNT/Effects/Camera FoV`                             |
| `resources.assets`                  | `451`     | `ZNT/Common/Animated Flat Textured Transparent`      |
| `resources.assets`                  | `452`     | `ZNT/Effects/Rain`                                   |
| `resources.assets`                  | `453`     | `ZNT/Effects/Water Lit`                              |
| `resources.assets`                  | `454`     | `ZNT/Effects/ForceField Impact`                      |
| `resources.assets`                  | `455`     | `ZNT/Effects/WormHole_fg`                            |
| `resources.assets`                  | `456`     | `ZNT/Common/Tex_Col_VCol_Blend_Snap`                 |
| `resources.assets`                  | `457`     | `ZNT/Effects/Laser`                                  |
| `resources.assets`                  | `458`     | `ZNT/Effects/Tunnel`                                 |
| `resources.assets`                  | `459`     | `ZNT/Misc/Resizable Ring Texture Space`              |
| `resources.assets`                  | `460`     | `ZNT/Effects/Water_Prepass`                          |
| `resources.assets`                  | `461`     | `ZNT/Effects/TemporalAnomalyDeform`                  |
| `resources.assets`                  | `462`     | `ZNT/Effects/TemporalAnomalyVortex`                  |
| `resources.assets`                  | `463`     | `ZNT/Effects/SpaceTimeTeleport`                      |
| `resources.assets`                  | `464`     | `ZNT/Effects/Radioactivity`                          |
| `resources.assets`                  | `465`     | `ZNT/Common/Animated Flat Textured Cutout`           |
| `resources.assets`                  | `466`     | `ZNT/Effects/Sound Cone`                             |
| `resources.assets`                  | `467`     | `ZNT/Common/Flat Textured AlphaTest`                 |
| `resources.assets`                  | `468`     | `ZNT/Common/VCol_Blend`                              |
| `resources.assets`                  | `469`     | `ZNT/Effects/Sound Waves`                            |
| `resources.assets`                  | `470`     | `tk2d/BlendVertexColor`                              |
| `resources.assets`                  | `471`     | `ZNT/Effects/MonolythParts`                          |
| `resources.assets`                  | `472`     | `ZNT/Characters/Characters Dissolve`                 |
| `resources.assets`                  | `473`     | `ZNT/Common/Flat Colored AlphaTest`                  |
| `resources.assets`                  | `474`     | `ZNT/Misc/Invisible Shadow Caster`                   |
| `resources.assets`                  | `475`     | `ZNT/Effects/ForceField`                             |
| `resources.assets`                  | `476`     | `ZNT/Effects/WormHole_bg`                            |
| `resources.assets`                  | `477`     | `ZNT/Misc/Generic Texture Lit`                       |
| `resources.assets`                  | `478`     | `ZNT/Effects/Fog`                                    |
| `resources.assets`                  | `479`     | `ZNT/Effects/Force Field - small`                    |
| `resources.assets`                  | `480`     | `ZNT/Effects/Doppler Waves`                          |
| `resources.assets`                  | `481`     | `ZNT/Effects/AoE Preview`                            |
| `resources.assets`                  | `482`     | `ZNT/Effects/SpitGaz`                                |
| `resources.assets`                  | `483`     | `ZNT/Effects/Flame Sliced Pixel`                     |
| `resources.assets`                  | `484`     | `ZNT/Characters/Characters Base`                     |
| `resources.assets`                  | `485`     | `ZNT/Misc/Generic Particle Lit`                      |
| `resources.assets`                  | `486`     | `ZNT/Common/Tex_VCol_Ambient_Blend`                  |
| `resources.assets`                  | `487`     | `ZNT/Environments/Environments Base`                 |
| `resources.assets`                  | `488`     | `ZNT/Misc/GenericBlend`                              |
| `resources.assets`                  | `489`     | `ZNT/Effects/WormHole`                               |
| `resources.assets`                  | `490`     | `ZNT/Effects/PortalSpawn`                            |
| `resources.assets`                  | `491`     | `ZNT/Effects/Dust Cloud`                             |
| `resources.assets`                  | `492`     | `ZNT/Effects/Haze`                                   |
| `resources.assets`                  | `493`     | `ZNT/Effects/Rain Splash`                            |
| `resources.assets`                  | `494`     | `ZNT/Effects/Heat Waves`                             |
| `resources.assets`                  | `495`     | `ZNT/Effects/Wind`                                   |

# tk2dSpriteCollectionData

> 精灵图集

## Sprite Create

> `*.sprite.info.json`

- `OrthoSize` : `float` 正交尺寸，默认取 `0.5`
- `TargetHeight` : `float` 目标高度，默认取 `12.0` ，对应游戏中一个网格的像素尺寸
- `Names` : `string[]` 精灵图名
- `Regions` : `UnityEngine.Rect[]` 精灵区域
- `Anchors` : `UnityEngine.Vector2[]` 精灵锚点
- `AttachPoints` : `Dictionary<int, tk2dSpriteDefinition.AttachPoint[]>` 附着点，用于确定特效和抛出点的位置
    * `name` : `string` 组件名称
    * `position` : `UnityEngine.Vector3`
    * `angle` : `float`
- `Material` : `UnityEngine.Material` 纹理
- `Name` : `string` 精灵图集名称
- `Transformed` : `bool` Regions 和 Anchors 默认取左上角为坐标原点，开启 Transformed 后取左下角为坐标原点

```json
{
  "OrthoSize": 0.5,
  "TargetHeight": 12.0,
  "Names": [
    "region_00",
    "region_01"
  ],
  "Regions": [
    {
      "x": 0.0,
      "y": 0.0,
      "width": 64.0,
      "height": 64.0
    },
    {
      "x": 64.0,
      "y": 64.0,
      "width": 64.0,
      "height": 64.0
    }
  ],
  "Anchors": [
    {
      "x": 32.0,
      "y": 32.0
    },
    {
      "x": 32.0,
      "y": 32.0
    }
  ],
  "AttachPoints": {
    "0": [
      {
        "name": "throw",
        "position": {
          "x": 0.7,
          "y": 0.8,
          "z": 0.0
        },
        "angle": 0.0
      }
    ]
  },
  "Material": "sprites_xxx_mat : UnityEngine.Material",
  "Name": "sprites_xxx",
  "Transformed": false
}
```

## Sprite Merge

> `*.sprite.merge.json`

- `Source` : `tk2dSpriteCollectionData` 原型图集，以其为蓝本，替换纹理
- `Name` : `string` 纹理名称
- `AttachPoints` : `Dictionary<int, tk2dSpriteDefinition.AttachPoint[]>` 附着点，用于确定特效和抛出点的位置
    * `name` : `string` 组件名称
    * `position` : `UnityEngine.Vector3`
    * `angle` : `float`
- `Material` : `UnityEngine.Material` 纹理

```json
{
  "Source": "sprites_boss : tk2dSpriteCollectionData",
  "Name": "sprites_xxx_311",
  "AttachPoints": {
    "574": [
      {
        "name": "throw",
        "position": {
          "x": 0.2,
          "y": 1.0,
          "z": 0.0
        },
        "angle": 0.0
      }
    ]
  },
  "Material": "sprites_xxx_mat : UnityEngine.Material"
}
```

## BuildIn

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

# tk2dSpriteAnimation

> 精灵动画

## Animation Create

> `*.animation.json`

- `clips` : `tk2dSpriteAnimationClip[]` 片段集
    * `name` : `string` 片段名
    * `frames` : `tk2dSpriteAnimationFrame[]` 片段帧集
    * `fps` : `float` 帧率
    * `loopStart` : `int` 循环起始点
    * `useableInLevelEditor` : `bool`
    * `staticAnimation` : `bool`
    * `wrapMode` : `tk2dSpriteAnimationClip.WrapMode`
        + `Loop`
        + `LoopSection`
        + `Once`
        + `PingPong`
        + `RandomFrame`
        + `RandomLoop`
        + `Single`
- `name` : `string` 精灵动画名称

tk2dSpriteAnimationFrame:

- `spriteCollection` : `tk2dSpriteCollectionData` 精灵图集
- `spriteId` : `int` 在精灵图集合中的序号
- `triggerEvent` : `bool` 触发事件
- `eventInfo` : `string` 事件名
- `eventInt` : `int` 事件参数
- `eventFloat` : `float` 事件参数
- `useAttachedEffects` : `bool`
- `attachedEffects` : `tk2dSpriteAnimationFrame.AttachedEffect[]`
    * `Name` : `string`
    * `VisualEffect` : `VisualEffect`
    * `RandomEffect` : `bool`
    * `RandomVisualEffects` : `List<VisualEffect>`
    * `AttachToPoint` : `bool`
    * `AttachIfUsed` : `tk2dSpriteAnimationFrame.AttachedEffect.AttachSetting`
        + `DoNothing`
        + `Add`
        + `Replace`
- `shaderAnimator` : `ShaderAnimator`
- `playSound` : `bool`
- `soundAsset` : [`FMODAsset`](FMODAsset.md)
- `soundPlayMode` : `tk2dSpriteAnimationFrame.SoundPlayMode`
    * `PlayOneshot`
    * `StartEvent`
    * `StopEvent`
    * `None`
- `stopEventOnAnimChange` : `bool`
- `preventSoundRestart` : `bool`
- `setSoundParam` : `bool`
- `soundParamName` : `string`
- `soundParamValue` : `float`

```json
{
  "clips": [
    {
      "name": "idle",
      "frames": [
        {
          "spriteCollection": "sprites_xxx : tk2dSpriteCollectionData",
          "spriteId": 0,
          "triggerEvent": false,
          "eventInfo": "",
          "eventInt": 0,
          "eventFloat": 0.0,
          "useAttachedEffects": false,
          "attachedEffects": [],
          "shaderAnimator": null,
          "playSound": true,
          "soundAsset": "event:/Environment/Chopper",
          "soundPlayMode": "PlayOneshot",
          "stopEventOnAnimChange": false,
          "preventSoundRestart": false,
          "setSoundParam": false,
          "soundParamName": "",
          "soundParamValue": 0.0
        },
        {
          "spriteCollection": "sprites_boss : tk2dSpriteCollectionData",
          "spriteId": 438,
          "triggerEvent": true,
          "eventInfo": "visual_effects",
          "eventInt": 0,
          "eventFloat": 0.0,
          "useAttachedEffects": true,
          "attachedEffects": [
            {
              "Name": "Effect 1",
              "VisualEffect": "BloodDeath_4 : VisualEffect",
              "RandomEffect": false,
              "RandomVisualEffects": [],
              "AttachToPoint": true,
              "AttachIfUsed": "DoNothing"
            }
          ],
          "shaderAnimator": null,
          "playSound": false,
          "soundAsset": null,
          "soundPlayMode": "PlayOneshot",
          "stopEventOnAnimChange": false,
          "preventSoundRestart": false,
          "setSoundParam": false,
          "soundParamName": "",
          "soundParamValue": 0.0
        }
      ],
      "fps": 16.0,
      "loopStart": 0,
      "useableInLevelEditor": false,
      "staticAnimation": false,
      "wrapMode": "Loop"
    }
  ],
  "name": "anim_xxx",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID  | Name                     |
|:-------------------|:--------|:-------------------------|
| `resources.assets` | `13719` | `anim_dsk`               |
| `resources.assets` | `13720` | `anim_ninja`             |
| `resources.assets` | `13724` | `anim_melee`             |
| `resources.assets` | `13725` | `anim_worker_1`          |
| `resources.assets` | `13731` | `anim_men_in_black`      |
| `resources.assets` | `13737` | `anim_vegetations`       |
| `resources.assets` | `13740` | `anim_girl_survivor_1`   |
| `resources.assets` | `13744` | `anim_blood`             |
| `resources.assets` | `13745` | `anim_soundman`          |
| `resources.assets` | `13746` | `anim_kamikaze`          |
| `resources.assets` | `13748` | `anim_daft_punk_1`       |
| `resources.assets` | `13749` | `anim_zombie_tank`       |
| `resources.assets` | `13750` | `anim_boss_2`            |
| `resources.assets` | `13754` | `anim_zombie_base`       |
| `resources.assets` | `13759` | `anim_sniper`            |
| `resources.assets` | `13760` | `anim_worker_4`          |
| `resources.assets` | `13765` | `anim_astrogoliath`      |
| `resources.assets` | `13766` | `anim_traps`             |
| `resources.assets` | `13769` | `anim_girl_hostage`      |
| `resources.assets` | `13770` | `anim_nudeguy`           |
| `resources.assets` | `13771` | `anim_daft_punk_2`       |
| `resources.assets` | `13772` | `anim_drone`             |
| `resources.assets` | `13774` | `anim_civil_2`           |
| `resources.assets` | `13776` | `anim_assault_survivor`  |
| `resources.assets` | `13782` | `anim_gunner`            |
| `resources.assets` | `13783` | `anim_props`             |
| `resources.assets` | `13784` | `anim_shotgun`           |
| `resources.assets` | `13791` | `anim_girl_3`            |
| `resources.assets` | `13795` | `anim_zombie_blocker`    |
| `resources.assets` | `13796` | `anim_priest`            |
| `resources.assets` | `13798` | `anim_worker_3`          |
| `resources.assets` | `13799` | `anim_zombie_climber`    |
| `resources.assets` | `13801` | `anim_boss_chemist`      |
| `resources.assets` | `13807` | `anim_civil_hostage`     |
| `resources.assets` | `13810` | `anim_rick`              |
| `resources.assets` | `13813` | `anim_civil_3`           |
| `resources.assets` | `13815` | `anim_explosions`        |
| `resources.assets` | `13817` | `anim_football`          |
| `resources.assets` | `13825` | `anim_granny`            |
| `resources.assets` | `13831` | `anim_homeless`          |
| `resources.assets` | `13835` | `anim_director`          |
| `resources.assets` | `13837` | `anim_shotgun_survivor`  |
| `resources.assets` | `13838` | `anim_boss_cinematic`    |
| `resources.assets` | `13845` | `anim_sniper_2`          |
| `resources.assets` | `13849` | `anim_men_in_black_2`    |
| `resources.assets` | `13855` | `anim_men_in_black_4`    |
| `resources.assets` | `13859` | `anim_bouncer`           |
| `resources.assets` | `13867` | `anim_machine_gunner`    |
| `resources.assets` | `13870` | `anim_preacher`          |
| `resources.assets` | `13872` | `anim_lumberjack`        |
| `resources.assets` | `13873` | `anim_nudegirl`          |
| `resources.assets` | `13877` | `anim_civil_2_torch`     |
| `resources.assets` | `13879` | `anim_worker_2`          |
| `resources.assets` | `13880` | `anim_human_icons`       |
| `resources.assets` | `13883` | `anim_driver`            |
| `resources.assets` | `13884` | `anim_swordwomen`        |
| `resources.assets` | `13893` | `anim_girl`              |
| `resources.assets` | `13895` | `anim_girl_2`            |
| `resources.assets` | `13898` | `anim_terminator`        |
| `resources.assets` | `13902` | `anim_toiletguy`         |
| `resources.assets` | `13905` | `anim_girl_3_garbage`    |
| `resources.assets` | `13906` | `anim_spaceman_2`        |
| `resources.assets` | `13907` | `anim_men_in_black_3`    |
| `resources.assets` | `13912` | `anim_girl_survivor_2`   |
| `resources.assets` | `13915` | `anim_virgin`            |
| `resources.assets` | `13916` | `anim_cheerleader`       |
| `resources.assets` | `13921` | `anim_assault_rifle_cop` |
| `resources.assets` | `13922` | `anim_civil`             |
| `resources.assets` | `13924` | `anim_civil_3_explosive` |
| `resources.assets` | `15157` | `anim_boss_1`            |
| `resources.assets` | `15158` | `anim_astronaut`         |
| `resources.assets` | `15159` | `anim_clown`             |
| `resources.assets` | `15871` | `anim_spacegirl_1`       |
| `resources.assets` | `15872` | `anim_doctor_girl_1`     |
| `resources.assets` | `16171` | `anim_crs`               |
| `resources.assets` | `17735` | `anim_supercomputer`     |
| `resources.assets` | `17879` | `anim_bishop`            |
| `resources.assets` | `17881` | `anim_chemist_pliers`    |
| `resources.assets` | `17882` | `anim_scientist_2`       |
| `resources.assets` | `17883` | `anim_chemist-chair`     |
| `resources.assets` | `17884` | `anim_hazmat`            |
| `resources.assets` | `17885` | `anim_doctor`            |
| `resources.assets` | `17886` | `anim_spaceman_1`        |
| `resources.assets` | `17887` | `anim_scientist_3`       |
| `resources.assets` | `17888` | `anim_chemist`           |
| `resources.assets` | `18277` | `anim_aliens_cinematic`  |

# VisualEffect

> 特效

## Visual Create

> `*.visual.json`

- `animation` : `AnimationSettings` 自定义动画
    * `PlayAnimation` : `bool`
    * `OverrideLibrary` : `bool` 关闭时使用预制体中的默认精灵动画 `anim_explosions`
    * `Library` : `tk2dSpriteAnimation`
    * `Clips` : `string[]`
- `prefab` : `UnityEngine.Transform` 预制体
    * `BarrelExplosion`
    * `MolotovExplosion`
- `name` : `string` 特效名称

```json
{
  "animation": {
    "PlayAnimation": true,
    "OverrideLibrary": true,
    "Library": "anim_blood : tk2dSpriteAnimation",
    "Clips": [
      "blood_explosion"
    ]
  },
  "prefab": "BarrelExplosion : UnityEngine.Transform",
  "despawnOnDisable": false,
  "despawnOnAnimChange": false,
  "name": "Blood",
  "hideFlags": "None"
}
```

## BuildIn

| Assets             | PathID  | Name                             | Prefab                           |
|:-------------------|:--------|:---------------------------------|:---------------------------------|
| `resources.assets` | `11862` | `AttractZombies`                 | `AttractRenderer`                |
| `resources.assets` | `11863` | `BlockerScreamWave`              | `AttractWave`                    |
| `resources.assets` | `11864` | `BloodDeath`                     | `DeathBlood`                     |
| `resources.assets` | `11865` | `BloodDeath_1`                   | `BloodDeath_1`                   |
| `resources.assets` | `11866` | `BloodDeath_2`                   | `BloodDeath_2`                   |
| `resources.assets` | `11867` | `BloodDeath_3`                   | `BloodDeath_3`                   |
| `resources.assets` | `11868` | `BloodDeath_4`                   | `BloodDeath_4`                   |
| `resources.assets` | `11869` | `BloodDeath_5`                   | `BloodDeath_5`                   |
| `resources.assets` | `11870` | `BloodDissolve`                  | `BloodDissolve`                  |
| `resources.assets` | `11871` | `BloodHit`                       | `BloodHit`                       |
| `resources.assets` | `11872` | `BloodHit_1`                     | `BloodHit_1`                     |
| `resources.assets` | `11873` | `BloodHit_2`                     | `BloodHit_2`                     |
| `resources.assets` | `11874` | `BloodHit_3`                     | `BloodHit_3`                     |
| `resources.assets` | `11875` | `BloodHit_4`                     | `BloodHit_4`                     |
| `resources.assets` | `11876` | `BloodRippedBase`                | `BloodRippedBase`                |
| `resources.assets` | `11877` | `BloodRippedCrawler`             | `BloodRippedCrawler`             |
| `resources.assets` | `11878` | `BloodRippedOverlord`            | `BloodRippedOverlord`            |
| `resources.assets` | `11879` | `BloodRippedTank`                | `BloodRippedTank`                |
| `resources.assets` | `11880` | `BloodSpill_1`                   | `BloodSpill_1`                   |
| `resources.assets` | `11881` | `BoomerBloodBase`                | `BoomerBloodBase`                |
| `resources.assets` | `11882` | `BoomerBloodTank`                | `BoomerBloodTank`                |
| `resources.assets` | `11883` | `ChemistHeart_BloodDeath_01`     | `ChemistHeart_BloodDeath_01`     |
| `resources.assets` | `11884` | `ChemistHeart_BloodHit_01`       | `ChemistHeart_BloodHit_01`       |
| `resources.assets` | `11885` | `ExplodeBloodBase`               | `ExplodeBloodBase`               |
| `resources.assets` | `11886` | `KamikazeDeath`                  | `KamikazeDeath`                  |
| `resources.assets` | `11887` | `BulkyDust`                      | `BulkyDust`                      |
| `resources.assets` | `11888` | `CrawlerDust`                    | `CrawlerDust`                    |
| `resources.assets` | `11889` | `PriestFire`                     | `PriestFire`                     |
| `resources.assets` | `11890` | `RepelWave`                      | `RepelWave`                      |
| `resources.assets` | `11891` | `SacrificeLight`                 | `SacrificeLight`                 |
| `resources.assets` | `11892` | `SoundCone`                      | `SoundCone`                      |
| `resources.assets` | `11893` | `SoundWave`                      | `SoundWave`                      |
| `resources.assets` | `11894` | `SpaceTimeTeleport`              | `SpaceTimeTeleport`              |
| `resources.assets` | `11895` | `TankDust`                       | `TankDust`                       |
| `resources.assets` | `11896` | `TankLanding`                    | `TankLanding`                    |
| `resources.assets` | `11897` | `ZombieDust`                     | `ZombieDust`                     |
| `resources.assets` | `11898` | `ZombieLight`                    | `ZombieLight`                    |
| `resources.assets` | `11899` | `BarrelExplosion`                | `BarrelExplosion`                |
| `resources.assets` | `11900` | `FleshExplosion`                 | `FleshExplosion`                 |
| `resources.assets` | `11901` | `GazolineTankExplosion`          | `BarrelExplosion`                |
| `resources.assets` | `11902` | `KamikazeExplosion`              | `KamikazeDeath`                  |
| `resources.assets` | `11903` | `MolotovExplosion`               | `MolotovExplosion`               |
| `resources.assets` | `11904` | `EyeDebris`                      | `EyeDebris`                      |
| `resources.assets` | `11905` | `ForceFieldImpact`               | `ForceFieldImpact`               |
| `resources.assets` | `11906` | `Inhale`                         | `Inhale`                         |
| `resources.assets` | `11907` | `Spit`                           | `Spit`                           |
| `resources.assets` | `11908` | `Vomit_1`                        | `Vomit_1`                        |
| `resources.assets` | `11909` | `Vomit_2`                        | `Vomit_2`                        |
| `resources.assets` | `11910` | `Vomit_3`                        | `Vomit_3`                        |
| `resources.assets` | `11911` | `Vomit_Blood`                    | `Vomit_Blood`                    |
| `resources.assets` | `11912` | `Vomit_Blood_dripping`           | `Vomit_Blood_dripping`           |
| `resources.assets` | `11913` | `Vomit_dripping_long`            | `Vomit_dripping_long`            |
| `resources.assets` | `11914` | `Vomit_dripping_short`           | `Vomit_dripping_short`           |
| `resources.assets` | `11915` | `AcidSplash_strong`              | `AcidSplash_strong`              |
| `resources.assets` | `11916` | `AlarmLight`                     | `RotatingBeacon`                 |
| `resources.assets` | `11917` | `BoomerBloodBlockerSub`          | `BoomerBloodBlockerSub`          |
| `resources.assets` | `11918` | `BoomerBloodBlockerSubExplosion` | `BoomerBloodBlockerSubExplosion` |
| `resources.assets` | `11919` | `BreakFloor`                     | `BreakFloor`                     |
| `resources.assets` | `11920` | `BurningFire`                    | `BurningFire`                    |
| `resources.assets` | `11921` | `CameraDestroyFX`                | `CameraDestroyFX`                |
| `resources.assets` | `11922` | `DoorDamage`                     | `DoorDamageFX`                   |
| `resources.assets` | `11923` | `DoorDestroy`                    | `DoorDestroyFX`                  |
| `resources.assets` | `11924` | `Drone_Plasma_Death`             | `Drone_Plasma_Death`             |
| `resources.assets` | `11925` | `LaserHit`                       | `LaserHit`                       |
| `resources.assets` | `11926` | `LaserShot_hit`                  | `LaserShot_hit`                  |
| `resources.assets` | `11927` | `MoonCanonExplosion`             | `MoonCanonExplosion`             |
| `resources.assets` | `11928` | `MoonCanonSmoke`                 | `MoonCanonSmoke`                 |
| `resources.assets` | `11929` | `SpitExplosion`                  | `SpitExplosion`                  |
| `resources.assets` | `11930` | `SpitOverlordMuzzle`             | `SpitOverlordMuzzle`             |
| `resources.assets` | `11931` | `SpitTankExplosion`              | `SpitTankExplosion`              |
| `resources.assets` | `11932` | `WalkFloor`                      | `WalkFloor`                      |
| `resources.assets` | `11933` | `WallDestroy`                    | `WallDestroyFX`                  |
| `resources.assets` | `11934` | `WaterSplash_strong`             | `WaterSplash_strong`             |
| `resources.assets` | `11935` | `Lightning`                      | `Lightning`                      |
| `resources.assets` | `11936` | `Mist`                           | `Mist`                           |
| `resources.assets` | `11937` | `Rain`                           | `Rain`                           |
| `resources.assets` | `11938` | `Snow`                           | `Snow`                           |