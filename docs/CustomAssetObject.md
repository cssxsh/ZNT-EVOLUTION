# CustomAssetObject

> 主要有 HumanAsset, ZombieAsset, DecorAsset

## 结构

- HumanAsset
  * name
  * <u>ref SpriteCollection</u> `tk2dSpriteCollectionData`
  * <u>ref AnimationLibrary</u> `tk2dSpriteAnimation` 定义了角色的各种动作
  * <u>ref RiseAsset</u> `CustomAssetObject` 转变类型
  * MagazineSize 弹匣容量
  * ReloadType `Automatic`/`ShellByShell`/`Stamina` 换弹类型
  * ReloadTime 换弹时间
  * StaminaRefillTimer 补弹时间
  * Invincible `true`/`false` 无敌
  * InvincibleOnAttack `true`/`false` 攻击时无敌
  * IgnoreDamages `true`/`false` 忽略攻击，取消硬直动画
  * DamageMultipliers `UnityDictionary<DamageType, float>` 伤害倍率
  * RiseOnDeath `true`/`false` 死亡时感染
  * RiseAsset `CustomAssetObject` 感染后转变的单位
  * DirectAim `true`/`false` 直接瞄准
  * BlockOpponents `true`/`false` 阻挡
  * MaxOpponentsBlock 最大阻挡数量
  * Attachments `UnityDictionary<string, UnityEngine.GameObject>` 附件
    * `Attachments["moving_attack"]` 移动攻击 `Gertrude Moving Attack`/`DrugLord Moving Attack`/`Priest Moving Attack`/`Lumberjack Moving Attack`
    * `Attachments["shield_attack"]` 护盾攻击 `VirginShield`
    * `Attachments["shield_effect"]` 护盾效果 `RepelWave`
    * `Attachments["attach_laser"]` 附加激光 `LaserAttachment`
  * <u>ref ThrowableObjects</u> `PhysicObjectAsset[]` 可抛物品
  * Voice `Voice` 语音类型
  * AimFieldOfView 瞄准视野
  * FieldOfView 视野
  * AimDistance 瞄准距离
  * Distance 视野距离
  * AimRange 瞄准射程
  * AttackRange 攻击射程
  * DamageRange 破坏射程
  * MovingAttackRange 移动攻击射程
  * Height 身高
  * StairDetectionHeightRatio 楼梯高度判定
  * CanClimb 可爬行
  * CanStepOver 可跨过
  * MaxFallHeight 最大下坠高度
  * FleeBeforeZombieExplode 僵尸爆炸前逃离
  * WalkSpeed 散步速度
  * RunSpeed 跑步速度 (`SprintSpeed = RunSpeed + 1`)
  * DeathVelocityThreshold 死亡速率阈值 (耐冲击)
  * JumpDeathVelocityThreshold 跳跃死亡速率阈值 (耐冲击)
  * ...

- DecorAsset
  * name
  * <u>ref Animation</u> `tk2dSpriteAnimation`
  * ActivateAnimation
  * DeactivateAnimation
  * UseStaticAnimation `true`/`false`
  * ActiveAnimation
  * InactiveAnimation
  * playSounds `true`/`false`
  * <u>ref sound</u> `FMODAsset`

- BreakablePropAsset
  * name
  * <u>ref Animation</u> `tk2dSpriteAnimation`
  * ActiveAnimation
  * InactiveAnimation
  * HitAnimation
  * BrokenAnimation
  * ColliderSize `UnityEngine.Vector2`
  * ColliderOffset `UnityEngine.Vector2`
  * MaxHp
  * UserInteractable `true`/`false`
  * MenuOffset `UnityEngine.Vector2`

- SentryGunAsset
  * name
  * Hp
  * Invincible `true`/`false`
  * DamageMultipliers `UnityDictionary<DamageType, float>` 伤害倍率
  * <u>ref AllDetection</u> `DetectionAsset`
  * <u>ref HumanDetection</u> `DetectionAsset`
  * <u>ref ZombieDetection</u> `DetectionAsset`
  * GeneralDirection `UnityEngine.Vector3`
  * DeviationAngle
  * FieldOfView
  * Distance
  * RayCount
  * DamageType `DamageType`
  * HitMultipleTargets `true`/`false`
  * NextTargetsDamageMultiplier
  * MaxTargets
  * Damage
  * CriticalMutliplier
  * AimRange
  * AttackRange
  * DamageRange
  * AttackFrequency
  * DefaultDamageChance
  * CriticalDamageChance
  * MissChance
  * <u>ref ThrowableObjects</u> `PhysicObjectAsset[]`
  * <u>ref SpriteCollection</u> `tk2dSpriteCollectionData`
  * SpriteIndex
  * <u>ref Animation</u> `tk2dSpriteAnimation`
  * ActiveAnimation
  * InactiveAnimation
  * ActivateAnimation
  * DeactivateAnimation
  * FireAnimation
  * Turn
  * HitAnimation
  * BreakAnimation

- TriggerAsset
  * name
  * type `Receiver`/`Trap`/`Trigger`/`Both`
  * OverrideAnimations `true`/`false`
  * PlaySounds `true`/`false`
  * DetectedLayers `UnityEngine.LayerMask`
  * CheckTags `true`/`false`
  * DetectedTags `Tag` 作用对象分类
  * resizeMode `None`/`Horizontal`/`Vertical`/`Both`
  * clampMethod `None`/`Relative`/`Absolute`
  * minSize `UnityEngine.Vector2`
  * roundToNeareset
  * RendererColor `UnityEngine.Color`
  * RenderMode `Tiled`/`SlicedAndTiled`/`Sliced`/`None`
  * PixelsPerMeter
  * <u>ref EffectPrefab</u> `ResizableParticleSystem`
  * <u>ref Animation</u> `tk2dSpriteAnimation`
  * ActiveAnimation
  * InactiveAnimation
  * ActivateAnimation
  * DeactivateAnimation
  * <u>ref ActivateEvent</u> `FMODAsset`
  * EventFollowObject `true`/`false`
  * Type `DamageType`
  * Mode `Damage`/`Kill`
  * KillDelay
  * Damage
  * DamageRate
  * Name

- MovingObjectAsset
  * name
  * <u>ref library</u> `tk2dSpriteAnimation`
  * MoveAnimation
  * StandAnimation
  * StopAnimation
  * DisableAnimation
  * HitAnimation
  * DestroyAnimation
  * <u>ref StandSound</u> `FMODAsset`
  * <u>ref DisableSound</u> `FMODAsset`
  * <u>ref MoveSound</u> `FMODAsset`
  * <u>ref StopSound</u> `FMODAsset`
  * <u>ref HitSound</u> `FMODAsset`
  * HideOnDisable `true`/`false`
  * AttachOnCollide `true`/`false`
  * ColliderType `None`/`Box`/`Circle`
  * ColliderIsTrigger `true`/`false`
  * <u>ref ColliderMaterial</u> `UnityEngine.PhysicsMaterial2D`
  * ColliderOffset `UnityEngine.Vector2`
  * BoxSize `UnityEngine.Vector2`
  * CircleRadius
  * ApplyDamage `true`/`false`
  * DamageShape `Box`/`Circle`
  * DamageOffset `UnityEngine.Vector2`
  * DamageSize `UnityEngine.Vector2`
  * DamageRadius
  * DamageType `DamageType`
  * DamageAppliedTo `UnityEngine.LayerMask`
  * Speed
  * MoveOnStart `true`/`false`

- PhysicObjectAsset
  * name
  * <u>ref library</u> `tk2dSpriteAnimation`
  * playAnimation `true`/`false`
  * MoveAnimation
  * HitAnimation
  * <u>ref RollingSound</u> `FMODAsset`
  * <u>ref HitSound</u> `FMODAsset`
  * ColliderRadius
  * ColliderOffset `UnityEngine.Vector2`
  * AttachToParent `true`/`false`
  * StartDirection `UnityEngine.Vector2`
  * StartForce
  * StartAngularVelocity
  * DamageRadius
  * DamageOffset `UnityEngine.Vector2`
  * DamageCharacterOnTrigger `true`/`false`
  * DamageCharacterOnCollide `true`/`false`
  * DamageAmount
  * DamageType `DamageType`
  * <u>ref Explosion</u> `ExplosionAsset`,
  * <u>ref ExplodeEffect</u> `VisualEffect`
  * ExplodeOn `Wall`/`Ground`/`Ceiling`/`Target` 代码扩展 `Zombie`/`Climber`/`Blocker`/`Tank`/`IgnoreHuman`