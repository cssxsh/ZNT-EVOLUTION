# LevelElement

> 关卡中的单位

## 结构

- LevelElement
  * name
  * Preview `UnityEngine.Sprite` 菜单图标
  * ElementType `Brush`/`Decor`
  * ...

- LevelElement<Tiles> (ElementType=Brush)
  * AllowedTileSystems `Foreground`/`Gameplay`/`Stairs`/`Triggers`/`Interior`/`InteriorDetails`/`Exterior`/`ExteriorDetails`/`BloodUp`/`BloodDown`/`BloodLeft`/`BloodRight`
  * Brush [Rotorz.Tile.Brush](./Brush.md)
  * <u>ref CustomAsset</u> 对应的角色或者装置

- LevelElement<Props> (ElementType=Decor)
  * AllowedDecorSystems `Foreground`/`Gameplay`/`Interior`/`Exterior`/`Middleground`/`Background`/`Sky`/`ExteriorParallax`/`ForegroundParallax`
  * DecorType `Static`/`Animated`/`Custom`
  * <u>ref SpriteCollection</u> `tk2dSpriteCollectionData`
  * SpriteIndex
  * <u>ref AnimationLibrary</u> `tk2dSpriteAnimation`
  * AnimClipId