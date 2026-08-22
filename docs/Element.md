# `Rotorz.Tile.OrientedBrush`

> 笔刷

一般情况不需要手动导入笔刷，插件会根据 `LevelElement.LinkedElement.Element` 复制创建笔刷

## Brush Create

> `*.brush.info.json`

- `Name` : `string` 笔刷名称
- `ForceLegacySideways` : `bool`
- `ApplyPrefabTransform` :`bool`
- `UserFlags` : `int`
- `Coalesce` : `Rotorz.Tile.Coalesce`
    * `None`
    * `Own`
    * `Other`
    * `Any`
    * `Own`
    * `Groups`
    * `OwnAndGroups`
- `Prefab` : `UnityEngine.GameObject` 预制体

```json
{
  "Name": "brush_human_xxx",
  "ForceLegacySideways": false,
  "ApplyPrefabTransform": false,
  "UserFlags": 0,
  "Coalesce": "None",
  "Prefab": "HumanXXX : UnityEngine.GameObject"
}
```

## Brush Merge

> `*.brush.merge.json`

- `Source` : `Rotorz.Tile.OrientedBrush` 原型笔刷，以其为蓝本，复制其参数
- `Name` : `string` 笔刷名称
- `Prefab` : `UnityEngine.GameObject` 预制体

```json
{
  "Source": "brush_human_astrogoliath : Rotorz.Tile.OrientedBrush",
  "Name": "brush_human_xxx",
  "Prefab": "HumanXXX : UnityEngine.GameObject"
}
```

# `UnityEngine.Sprite`

> 预览

## Load Image

> `*.preview.tga`, `*.preview.png`, `*.preview.exr`

图片尺寸应为 `128x128`

## Preview Create

> `*.preview.info.json`

- `Name` : `string` 预览名称
- `Texture` : `UnityEngine.Texture2D` 预览贴图
- `Rect` : `UnityEngine.Rect` 截取的区域，取左下角为坐标原点
- `Pivot` : `UnityEngine.Vector2`
- `PixelsPerUnit` : `int`

```json
{
  "Name": "downtown_shop_1",
  "Texture": "sprites_downtown_shop_atlas : UnityEngine.Texture2D",
  "Rect": {
    "x": 98.0,
    "y": 2.0,
    "width": 60.0,
    "height": 60.0
  },
  "Pivot": {
    "x": 0.5,
    "y": 0.5
  },
  "PixelsPerUnit": 100
}
```

# `LevelElement`

> 关卡单位

## Element Create

> `*.element.json`

- `title` : `string` 在编辑器中显示的标题
- `tags` : [`Tag`](Asset.md#tag) 附加标签
- `type` : `LevelElement.Type`
    * `Brush` 对应 `Tiles`
    * `Decor` 对应 `Props`
- `devOnly` : `bool`
- `useable` : `bool`
- `replaceBy` : `LevelElement`
- `OverrideSortingLayer` : `bool`
- `SortingLayer` : [`SortingLayer`](Asset.md#sorting-layer),
- `CastShadows` : `bool`
- `ReceiveShadows` : `bool`
- `CustomAsset` : `CustomAssetObject` <Brush> 对象参数
- `DecorType` : `LevelElement.DecorStyle`
    * `Static`
    * `Animated`
    * `Custom`
- `DecorPrefab` : `UnityEngine.GameObject` 预制体
    * `StaticDecor`
    * `AnimatedDecor`
    * `ScrollingDecor`
- `AllowedDecorSystems` : `DecorSystemLayer.LayerType`
    * `Foreground`
    * `Gameplay`
    * `Interior`
    * `Exterior`
    * `Middleground`
    * `Background`
    * `Sky`
    * `ExteriorParallax`
    * `ForegroundParallax`
- `SpriteCollection` : `tk2dSpriteCollectionData` <Decor>
- `SpriteIndex` : `int` <Decor>
- `SpriteName` : `string`
- `AnimationLibrary` : `tk2dSpriteAnimation` <Decor>
- `AnimClipId` : `int` <Decor>
- `AttachPoints` : `List<AttachPoint>` <Decor>
    * `Name` : `string`
    * `Attachement` : `UnityEngine.GameObject`
    * `Offset` : `UnityEngine.Vector3`
    * `Rotation` : `UnityEngine.Vector3`
- `AddIdentifier` : `bool`
- `PaintingMode` : `LevelElement.PaintMode` <Brush>
    * `OneByOne`
    * `AllowStrokes`
    * `AllowStrokesWithShapes`
- `Brush` : `Rotorz.Tile.Brush` 笔刷，需要和 LevelElement 一一对应，置为 `null` 时会尝试自动生成
- `Size` : `UnityEngine.Vector2` <Brush> 地图中占格子的大小
- `Pivot` : `UnityEngine.Vector2` <Brush> 地图中占格子的中心
- `AllowedTileSystems` : `TileSystemLayer.LayerType`
    * `Foreground`
    * `Gameplay`
    * `Stairs`
    * `Triggers`
    * `Interior`
    * `InteriorDetails`
    * `Exterior`
    * `ExteriorDetails`
    * `BloodUp`
    * `BloodDown`
    * `BloodLeft`
    * `BloodRight`
- `LinkElement` : `bool`
- `LinkedElement` : `LinkedElement`
    * `Layer` : `TileSystemLayer.LayerType`
    * `Element` : `LevelElement` 插件化用为原型存放位置
- `Preview` : `UnityEngine.Sprite` 菜单栏中的预览
- `ShowEditorIcon` : `bool` 编辑地图时显示为图标
- `IconFollowObject` : `bool`
- `EditorIconColor` : `UnityEngine.Color`
- `EditorSpriteCollection` : `tk2dSpriteCollectionData`
- `EditorIcon` : `int`
- `AddColliderInEditor` : `bool` <Decor>
- `FitEditorCollider` : `bool` <Brush>
- `AddObjectSettings` : `bool` 可设置对性属性
- `assetId` : `string` 设置为 `null` 时, 取 `name` 的值
- `name` : `string` 单位名称

```json
{
  "title": "Human Astrogoliath",
  "tags": 0,
  "type": "Brush",
  "devOnly": false,
  "useable": true,
  "replaceBy": null,
  "OverrideSortingLayer": false,
  "SortingLayer": "",
  "CastShadows": true,
  "ReceiveShadows": true,
  "CustomAsset": "Astrogoliath : HumanAsset",
  "DecorType": "Static",
  "DecorPrefab": null,
  "AllowedDecorSystems": 0,
  "SpriteCollection": null,
  "SpriteIndex": 0,
  "SpriteName": "",
  "AnimationLibrary": null,
  "AnimClipId": 0,
  "AttachPoints": [],
  "AddIdentifier": false,
  "PaintingMode": "OneByOne",
  "Brush": "brush_human_astrogoliath : Rotorz.Tile.OrientedBrush",
  "Size": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 1.0,
    "y": 2.0
  },
  "Pivot": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 1.0
  },
  "AllowedTileSystems": "Gameplay",
  "LinkElement": false,
  "LinkedElement": {
    "Layer": "Foreground",
    "Element": null
  },
  "Preview": "90611173e2ac39b4b9f4f954581f9e78 : UnityEngine.Sprite",
  "ShowEditorIcon": false,
  "IconFollowObject": false,
  "EditorIconColor": {
    "$type": "UnityEngine.Color, UnityEngine.CoreModule",
    "r": 1.0,
    "g": 1.0,
    "b": 1.0,
    "a": 1.0
  },
  "EditorSpriteCollection": null,
  "EditorIcon": 0,
  "AddColliderInEditor": true,
  "FitEditorCollider": true,
  "AddObjectSettings": true,
  "assetId": "90611173e2ac39b4b9f4f954581f9e78",
  "name": "human_astrogoliath",
  "hideFlags": "None"
}
```

```json
{
  "title": "Scrolling Rails",
  "tags": 0,
  "type": "Decor",
  "devOnly": false,
  "useable": true,
  "replaceBy": null,
  "OverrideSortingLayer": false,
  "SortingLayer": "",
  "CastShadows": false,
  "ReceiveShadows": false,
  "CustomAsset": null,
  "DecorType": "Custom",
  "DecorPrefab": "ScrollingDecor : UnityEngine.GameObject",
  "AllowedDecorSystems": "Gameplay",
  "SpriteCollection": "sprites_rails : tk2dSpriteCollectionData",
  "SpriteIndex": 1,
  "SpriteName": "",
  "AnimationLibrary": null,
  "AnimClipId": 0,
  "AddIdentifier": false,
  "PaintingMode": "OneByOne",
  "Brush": null,
  "Size": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 1.0,
    "y": 1.0
  },
  "Pivot": {
    "$type": "UnityEngine.Vector2, UnityEngine.CoreModule",
    "x": 0.0,
    "y": 0.0
  },
  "AllowedTileSystems": 0,
  "LinkElement": false,
  "LinkedElement": {
    "Layer": "Foreground",
    "Element": null
  },
  "Preview": "b7583caf1da573e49bc35c12f4569d3c : UnityEngine.Sprite",
  "ShowEditorIcon": false,
  "IconFollowObject": false,
  "EditorIconColor": {
    "$type": "UnityEngine.Color, UnityEngine.CoreModule",
    "r": 1.0,
    "g": 1.0,
    "b": 1.0,
    "a": 1.0
  },
  "EditorSpriteCollection": null,
  "EditorIcon": 0,
  "AddColliderInEditor": false,
  "FitEditorCollider": true,
  "AddObjectSettings": false,
  "assetId": "b7583caf1da573e49bc35c12f4569d3c",
  "name": "rails",
  "hideFlags": "None"
}
```