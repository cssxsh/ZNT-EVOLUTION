# tk2dSpriteCollectionData

> 精灵图集合类，将原图分割为一个一个小块，以便之后作为动画帧使用

## 结构

- tk2dSpriteCollectionData
  * name
  * spriteDefinitions `tk2dSpriteDefinition[]`
  * ...

- tk2dSpriteDefinition
  * name
  * <u>ref material</u> `UnityEngine.Material`
  * attachPoints `tk2dSpriteDefinition.AttachPoint[]` 附着点，作用为更新组件坐标
  * ...

- tk2dSpriteDefinition.AttachPoint
  * name
  * position `UnityEngine.Vector3`
  * angle