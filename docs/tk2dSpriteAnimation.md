# tk2dSpriteAnimation

> 动画类，定义了动画片段，片段由帧和音频组成

## 结构

- tk2dSpriteAnimation
  * name
  * clips `tk2dSpriteAnimationClip[]`

- tk2dSpriteAnimationClip
  * name 片段名
  * frames `tk2dSpriteAnimationFrame[]`
  * fps
  * loopStart
  * useableInLevelEditor `true`/`false` 在地图编辑器可选
  * staticAnimation `true`/`false`
  * wrapMode `Loop`/`LoopSection`/`Once`/`PingPong`/`RandomFrame`/`RandomLoop`/`Single`

- tk2dSpriteAnimationFrame
  * <u>ref spriteCollection</u> `tk2dSpriteCollectionData`
  * spriteId 在精灵图集合中的序号
  * triggerEvent `true`/`false`
  * eventInfo 事件名
  * eventInt 事件参数
  * eventFloat 事件参数
  * useAttachedEffects `true`/`false`
  * attachedEffects `tk2dSpriteAnimationFrame.AttachedEffect[]`
  * <u>ref shaderAnimator</u> `ShaderAnimator`
  * playSound `true`/`false`
  * <u>ref soundAsset</u> `FMODAsset`
  * soundPlayMode `PlayOneshot`/`StartEvent`/`StopEvent`/`None`
  * stopEventOnAnimChange `true`/`false`
  * preventSoundRestart `true`/`false`
  * setSoundParam `true`/`false`
  * soundParamName
  * soundParamValue

- tk2dSpriteAnimationFrame.AttachedEffect
  * Name
  * <u>ref VisualEffect</u> `VisualEffect`
  * RandomEffect `true`/`false`
  * <u>ref RandomVisualEffects</u> `List<VisualEffect>`
  * AttachToPoint `true`/`false`
  * AttachIfUsed `DoNothing`/`Add`/`Replace`