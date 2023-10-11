# LevelElement

> 关卡中的单位

## 结构

- LevelElement
  * name
  * Preview 菜单图标
  * ElementType `Brush`/`Decor`
  * ...

- LevelElement<Tiles> (ElementType=Brush)
  * AllowedTileSystems `Foreground`/`Gameplay`/`Stairs`/`Triggers`/`Interior`/`InteriorDetails`/`Exterior`/`ExteriorDetails`/`BloodUp`/`BloodDown`/`BloodLeft`/`BloodRight`
  * Brush 笔刷
  * CustomAsset 对应的角色或者装置

- LevelElement<Props> (ElementType=Decor)
  * AllowedDecorSystems `Foreground`/`Gameplay`/`Interior`/`Exterior`/`Middleground`/`Background`/`Sky`/`ExteriorParallax`/`ForegroundParallax`
  * DecorType `Static`/`Animated`/`Custom`
  * SpriteCollection
  * SpriteIndex
  * AnimationLibrary