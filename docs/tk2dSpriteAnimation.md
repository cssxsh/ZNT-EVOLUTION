# tk2dSpriteAnimation

> 动画类，定义了动画片段，片段由帧和音频组成

## 结构

- tk2dSpriteAnimation
    * name
    * clips

- tk2dSpriteAnimationClip
    * name 片段名
    * frames
    * fps
    * loopStart
    * useableInLevelEditor
    * staticAnimation
    * wrapMode `Loop`/`LoopSection`/`Once`/`PingPong`/`RandomFrame`/`RandomLoop`/`Single`

- tk2dSpriteAnimationFrame
    * ~~ref spriteCollection~~
    * spriteId 在精灵图集合中的序号
    * triggerEvent
    * eventInfo
    * eventInt
    * eventFloat
    * useAttachedEffects
    * attachedEffects
    * ~~ref shaderAnimator~~
    * playSound
    * ~~ref soundAsset~~
    * soundPlayMode `PlayOneshot`/`StartEvent`/`StopEvent`/`None`
    * stopEventOnAnimChange
    * preventSoundRestart
    * setSoundParam
    * soundParamName
    * soundParamValue

- tk2dSpriteAnimationFrame.AttachedEffect
    * Name
    * ~~ref VisualEffect~~
    * RandomEffect
    * ~~ref RandomVisualEffects~~
    * AttachToPoint
    * AttachIfUsed `DoNothing`/`Add`/`Replace`