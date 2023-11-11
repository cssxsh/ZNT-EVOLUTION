# LevelElementLoader

> 关卡单位导入

为了加快加载速度，不直接从 `json` 加载 `tk2dSpriteCollectionData`  
而是利用 `SpriteInfo` 或 `SpriteMerge` 间接生成

## LoadFormFolder

> 加载 `LevelElement`

- `SpriteInfo` 包含坐标信息, 构造自定义的 `tk2dSpriteCollectionData`
- `SpriteMerge` 包含原型 `tk2dSpriteCollectionData`，通过替换原本的贴图，构造 `tk2dSpriteCollectionData`

### LoadBrushFormFolder

> 加载 `Tile`

- AssetBundle `resources.bundle`
  * UnityEngine.Material `sprites` <- UnityEngine.Shader, UnityEngine.Texture2D
  * UnityEngine.Sprite `preview` <- UnityEngine.Texture2D
  * Rotorz.Tile.OrientedBrush `brush` <- UnityEngine.GameObject
- SpriteInfo `sprite.info.json`
  * string[] `Names`
  * UnityEngine.Rect[] `Regions`
  * UnityEngine.Vector2 `Anchor`
- SpriteMerge `sprite.merge.json`
  * tk2dSpriteCollectionData `Source`
  * string `Name`
- tk2dSpriteAnimation `animation.json`
  * string `name`
  * tk2dSpriteAnimationClip[] `clips` <- tk2dSpriteCollectionData
- CustomAsset `asset.json`
  * string `name`
  * string `HierarchyName`
  * ...
- LevelElement `element.json`
  * string `name`
  * string `Title`
  * UnityEngine.Sprite `Preview` <= `preview`
  * Rotorz.Tile.OrientedBrush `Brush` <= `brush`
  * CustomAssetObject `CustomAsset` <= `asset.json` <- `animation.json`, tk2dSpriteCollectionData

#### DecorAsset

> 特殊处理后可以一次加载多个 `LevelElement`

- DecorAsset `asset.json`
  * string `name` <- `index` <- `animation.json`
  * string `HierarchyName`
  * tk2dSpriteAnimation `Animation` <= `animation.json`
  * string `ActivateAnimation` <- `index` <- `animation.json`
  * string `DeactivateAnimation` <- `index` <- `animation.json`
  * string `ActiveAnimation` <- `index` <- `animation.json`
  * string `InactiveAnimation` <- `index` <- `animation.json`

- LevelElement `element.json`
  * string `name` <- `index` <- `animation.json`
  * string `Title` <- `index` <- `animation.json`
  * UnityEngine.Sprite `Preview` <= `preview`
  * Rotorz.Tile.OrientedBrush `Brush` <- `brush`, `index` <- `animation.json`
  * DecorAsset `CustomAsset` <- `asset.json`, `index` <- `animation.json`

### LoadDecorFormFolder

#### Single

> 单个

- AssetBundle `resources.bundle`
  * UnityEngine.Material `sprites` <- UnityEngine.Shader, UnityEngine.Texture2D
  * UnityEngine.Sprite `preview` <- UnityEngine.Texture2D
- LevelElement `element.json`
  * string `name`
  * string `Title`
  * UnityEngine.Sprite `Preview` <= `preview`
  * tk2dSpriteCollectionData `SpriteCollection` <- `sprites`

#### Multiple

> 多个

- AssetBundle `resources.bundle`
  * UnityEngine.Material `sprites` <- UnityEngine.Shader, UnityEngine.Texture2D
  * UnityEngine.Sprite `preview` <- UnityEngine.Texture2D
- SpriteInfo `sprite.info.json`
  * string[] `Names`
  * UnityEngine.Rect[] `Regions`
- LevelElement `element.json`
  * string `name` <- `index` <- `sprite.info.json`
  * string `Title` <- `index`, `Names` <- `sprite.info.json`
  * UnityEngine.Sprite `Preview` <= `preview`
  * tk2dSpriteCollectionData `SpriteCollection` <- `sprite.info.json`, `sprites`

#### Animated

> 动画

- AssetBundle `resources.bundle`
  * UnityEngine.Material `sprites` <- UnityEngine.Shader, UnityEngine.Texture2D
  * UnityEngine.Sprite `preview` <- UnityEngine.Texture2D
- SpriteInfo `sprite.info.json`
  * string[] `Names`
  * UnityEngine.Rect[] `Regions`
- tk2dSpriteAnimation `animation.json`
  * string `name`
  * tk2dSpriteAnimationClip[] `clips` <- tk2dSpriteCollectionData <- `sprite.info.json`, `sprites`
- LevelElement `element.json`
  * string `name`
  * string `Title`
  * UnityEngine.Sprite `Preview`
  * tk2dSpriteCollectionData `SpriteCollection` <- `sprite.info.json`, `sprites`
  * tk2dSpriteAnimation `AnimationLibrary` <= `animation.json`

## ApplyFormFolder

> 修改

- `AnimationAddition` 包含操作目标和要添加的动画片段

### Animation

> 修改动画

- AssetBundle `resources.bundle`
  * UnityEngine.Material `sprites` <- UnityEngine.Shader, UnityEngine.Texture2D
- SpriteInfo `sprite.info.json`
  * string[] `Names`
  * UnityEngine.Rect[] `Regions`
  * UnityEngine.Vector2 `Anchor`
- AnimationAddition `animation.addition.json`
  * string[] `Targets`
  * tk2dSpriteAnimationClip[] `Clips` <- tk2dSpriteCollectionData <- `sprite.info.json`, `sprites`

## Supplement

### CustomAssetObjectPatch

#### MovingObjectAsset

> 警车有特殊处理，在转向时切换动画

#### TriggerAsset

> 地雷菜单是通过 MineTrapEditor 实现的

### DecorToBrush

> 参照直升机，救火直升机会对应添加一个 Tile