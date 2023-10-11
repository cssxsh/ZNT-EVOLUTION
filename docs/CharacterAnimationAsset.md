# CharacterAnimationAsset

> 将动画片段和动作联系起来

## 结构

- CharacterAnimationAsset
  * name
  * AnimationLibrary
  * CommonAnimations `[Animations] => AnimationSettings`
  * DefaultHitAnimation
  * HitAnimations `[DamageType] => AnimationSettings`
  * DefaultDeathAnimation
  * DeathAnimations `[DamageType] => AnimationSettings`
  * AirDeathAnimation
  * AirDeathLandingAnimation

- AnimationSettings
  * PlayAnimation
  * OverrideLibrary
  * Library
  * Clips 动画片段名

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

```csharp
  None,
  Fall,
  Bite,
  Gun,
  Rifle,
  Shotgun,
  Melee,
  Sword,
  Canon,
  Explosion,
  Spikes,
  Fire,
  Electricity,
  Laser,
  Sentry,
  Contamination,
  Sacrifice,
  Acid,
  Radioactivity,
  Ripped,
  Plasma,
  Squashed,
  TankDash,
  MachineGun,
  Crawler,
  Tank,
  Boomer,
  Spit,
  HolyFire,
```