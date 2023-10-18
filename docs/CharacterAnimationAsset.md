# CharacterAnimationAsset

> 将动画片段和动作联系起来

## 枚举

* `ArmedAnimations`
* `BulkyMeleeAnimations`
* `CrawlerAnimations`
* `DroneAnimations`
* `MeleeAnimations`
* `OverlordAnimations`
* `TankAnimations`
* `TerminatorAnimations`
* `UnarmedAnimations`
* `ZombieAnimations`

```csharp
public enum Animations
{
  Spawn,
  Stand,
  Idle,
  Turn,
  Walk,
  Run,
  StepOver,
  StepOverSprint,
  SprintStart,
  Sprint,
  Decelerate,
  Fall,
  FallLandingLow,
  FallLandingHigh,
  FallLandingSprint,
  ClimbStart,
  ClimbStartRun,
  Climb,
  ClimbOut,
  ClimbOutRun,
  ClimbRun,
  Jump,
  JumpFall,
  JumpReception,
  JumpWallReception,
  Attack,
  AttackProp,
  AttackSprint,
  Repulse,
  AimStart,
  Aim,
  AimStop,
  Reload,
  ReloadEnd,
  AlertStart,
  AlertStartTurn,
  Alerted,
  AlertEnd,
  Scared,
  Paralised,
  ParalisedEnd,
  ContaminationRise,
  CrawlerTransformm,
  TankTransform,
  OverlordTransfom,
  Explode,
  Sacrifice,
  Spit,
  Scream,
  HitWall,
  Grabbed,
  Reborn,
  ScreamClimb,
  SpitAim,
  Empty,
}
```

## 结构

- CharacterAnimationAsset
  * name
  * <u>ref AnimationLibrary</u> `tk2dSpriteCollectionData`
  * CommonAnimations `UnityDictionary<Animations, AnimationSettings>`
  * DefaultHitAnimation
  * HitAnimations `UnityDictionary<DamageType, AnimationSettings>`
  * DefaultDeathAnimation
  * DeathAnimations `UnityDictionary<DamageType, AnimationSettings>`
  * AirDeathAnimation
  * AirDeathLandingAnimation

- AnimationSettings
  * PlayAnimation `true`/`false`
  * OverrideLibrary `true`/`false`
  * <u>Library</u> `tk2dSpriteAnimation`
  * Clips 动画片段名 `string[]`