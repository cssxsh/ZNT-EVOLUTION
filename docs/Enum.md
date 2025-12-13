# Enum

> 常用枚举

## 攻击类型

```csharp
public enum DamageType
{
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
}
```

## 声音类型

```csharp
public enum Voice
{
  None,
  Woman1,
  Woman2,
  Man1,
  Man2,
  Survivor1,
  Survivor2,
  Bouncer,
  Cop,
  Hazmat,
  Doctor,
  Computer,
  Dj,
  Clown,
  Terminator,
  DrugLord,
  Gertrude,
  Chemist,
  ChemistMutated,
  ChemistFinal,
  Michelle,
  MachineGunner,
  Director,
  Preacher,
  Homeless,
}
```

## 动画类型

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

## 角色类型

```csharp
public enum CharacterType
{
  // 僵尸
  Zombie,
  // 平民
  Civilian,
  // 武装平民
  ArmedCivilian,
  // 幸存者
  Survivor,
  // 警察
  Cop,
  // 黑衣人
  MenInBlack,
  // 非人
  Inhuman,
  // 老闆
  Boss,
  // 宇航员
  Astronaut,
  // 邪教徒
  Cultist,
}
```