# LevelElement

> 关卡中的单位

## 结构

- LevelElement
  * name
  * Preview `UnityEngine.Sprite` 菜单图标
  * ElementType `Brush`/`Decor`
  * Tags `标签`
  * ...

- LevelElement<Tiles> (ElementType=Brush)
  * AllowedTileSystems `Foreground`/`Gameplay`/`Stairs`/`Triggers`/`Interior`/`InteriorDetails`/`Exterior`/`ExteriorDetails`/`BloodUp`/`BloodDown`/`BloodLeft`/`BloodRight`
  * Brush [Rotorz.Tile.Brush](./Brush.md) 笔刷
  * <u>ref CustomAsset</u> [CustomAssetObject](./CustomAssetObject.md) 对应的角色或者装置

- LevelElement<Props> (ElementType=Decor)
  * AllowedDecorSystems `Foreground`/`Gameplay`/`Interior`/`Exterior`/`Middleground`/`Background`/`Sky`/`ExteriorParallax`/`ForegroundParallax`
  * DecorType `Static`/`Animated`/`Custom`
  * <u>ref SpriteCollection</u> [tk2dSpriteCollectionData](./tk2dSpriteCollectionData.md)
  * SpriteIndex
  * <u>ref AnimationLibrary</u> [tk2dSpriteAnimation](./tk2dSpriteAnimation.md)
  * AnimClipId