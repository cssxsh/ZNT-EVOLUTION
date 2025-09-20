# CharacterAnimationAsset

> 将动画片段和动作联系起来

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
  * <u>ref Library</u> `tk2dSpriteAnimation`
  * Clips `string[]` 动画片段名

## 实例

- `MeleeAnimations`
  * `AnimationLibrary` => `anim_ninja`
  * `CommonAnimations[Animations.Spawn]` => `[rise]`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Idle]` => `[stand, stand_random_1, stand_random_2, stand_random_3]`
  * `CommonAnimations[Animations.Turn]` => `[turn]`
  * `CommonAnimations[Animations.Walk]` => `[walk]`
  * `CommonAnimations[Animations.Run]` => `[run]`
  * `CommonAnimations[Animations.StepOver]` => `[stepover]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high]`
  * `CommonAnimations[Animations.AlertStart]` => `[alert_start]`
  * `CommonAnimations[Animations.AlertStartTurn]` => `[alert_start_turn]`
  * `CommonAnimations[Animations.Alerted]` => `[alert_stand]`
  * `CommonAnimations[Animations.AlertEnd]` => `[alert_end]`
  * `CommonAnimations[Animations.Scared]` => `[scared]`
  * `CommonAnimations[Animations.Paralised]` => `[paralised]`
  * `CommonAnimations[Animations.ParalisedEnd]` => `[paralised_stop]`
  * `CommonAnimations[Animations.Grabbed]` => `[grasped]`
  * `CommonAnimations[Animations.Attack]` => `[shoot, shoot_2]`
  * `CommonAnimations[Animations.AimStart]` => `[aim_start]`
  * `CommonAnimations[Animations.Aim]` => `[aim]`
  * `CommonAnimations[Animations.AimStop]` => `[aim_stop]`
  * `CommonAnimations[Animations.Reload]` => `[reload]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `CommonAnimations[Animations.Climb]` => `[climb]`
  * `CommonAnimations[Animations.ClimbStart]` => `[climb_start]`
  * `CommonAnimations[Animations.ClimbOut]` => `[climbout]`
  * `DefaultHitAnimation` => `[attacked]`
  * `HitAnimations[DamageType.Fire]` => `[]`
  * `HitAnimations[DamageType.Acid]` => `[]`
  * `HitAnimations[DamageType.Radioactivity]` => `[]`
  * `HitAnimations[DamageType.Spit]` => `[]`
  * `DefaultDeathAnimation` => `[death]`
  * `DeathAnimations[DamageType.Fall]` => `[fall_death]`
  * `DeathAnimations[DamageType.Contamination]` => `[contamination]`
  * ~~`DeathAnimations[DamageType.Plasma]` => `anim_zombie_base` => `[death_plasma]`~~
  * ~~`DeathAnimations[DamageType.Laser]` => `anim_zombie_base` => `[death_plasma]`~~
  * `DeathAnimations[DamageType.TankDash]` => `[fall_death]`
  * `DeathAnimations[DamageType.Squashed]` => `[fall_death]`
  * ~~`DeathAnimations[DamageType.Electricity]` => `anim_zombie_base` => `[death_electric]`~~
  * ~~`DeathAnimations[DamageType.Acid]` => `anim_zombie_base` => `[death_acid]`~~
  * `DeathAnimations[DamageType.Spikes]` => `[fall_death]`
  * ~~`DeathAnimations[DamageType.Radioactivity]` => `anim_zombie_base` => `[death_acid]`~~
  * ~~`DeathAnimations[DamageType.Fire]` => `anim_zombie_base` => `[death_fire]`~~
  * ~~`DeathAnimations[DamageType.HolyFire]` => `anim_zombie_base` => `[death_fire]`~~
  * `DeathAnimations[DamageType.Spit]` => `[contamination]`
  * `AirDeathAnimation` => `[]`
  * `AirDeathLandingAnimation` => `[]`

- `ArmedAnimations`
  * `AnimationLibrary` => `anim_shotgun`
  * `CommonAnimations[Animations.Spawn]` => `[rise]`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Idle]` => `[stand, stand_random_1, stand_random_2, stand_random_3]`
  * `CommonAnimations[Animations.Turn]` => `[turn]`
  * `CommonAnimations[Animations.Walk]` => `[walk]`
  * `CommonAnimations[Animations.Run]` => `[run]`
  * `CommonAnimations[Animations.StepOver]` => `[stepover]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high]`
  * `CommonAnimations[Animations.AlertStart]` => `[alert_start]`
  * `CommonAnimations[Animations.AlertStartTurn]` => `[alert_start_turn]`
  * `CommonAnimations[Animations.Alerted]` => `[alert_stand]`
  * `CommonAnimations[Animations.AlertEnd]` => `[alert_end]`
  * `CommonAnimations[Animations.Scared]` => `[scared]`
  * `CommonAnimations[Animations.Paralised]` => `[paralised]`
  * `CommonAnimations[Animations.ParalisedEnd]` => `[paralised_stop]`
  * `CommonAnimations[Animations.Grabbed]` => `[grasped]`
  * `CommonAnimations[Animations.Attack]` => `[shoot]`
  * `CommonAnimations[Animations.AimStart]` => `[aim_start]`
  * `CommonAnimations[Animations.Aim]` => `[aim]`
  * `CommonAnimations[Animations.AimStop]` => `[aim_stop]`
  * `CommonAnimations[Animations.Reload]` => `[reload]`
  * `CommonAnimations[Animations.ReloadEnd]` => `[reload_end]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `DefaultHitAnimation` => `[attacked]`
  * `HitAnimations[DamageType.Fire]` => `[]`
  * `HitAnimations[DamageType.Acid]` => `[]`
  * `HitAnimations[DamageType.Radioactivity]` => `[]`
  * `HitAnimations[DamageType.Spit]` => `[]`
  * `DefaultDeathAnimation` => `[death]`
  * `DeathAnimations[DamageType.Fall]` => `[fall_death]`
  * `DeathAnimations[DamageType.Contamination]` => `[contamination]`
  * ~~`DeathAnimations[DamageType.Plasma]` => `anim_zombie_base` => `[death_plasma]`~~
  * ~~`DeathAnimations[DamageType.Laser]` => `anim_zombie_base` => `[death_plasma]`~~
  * `DeathAnimations[DamageType.TankDash]` => `[fall_death]`
  * `DeathAnimations[DamageType.Squashed]` => `[fall_death]`
  * ~~`DeathAnimations[DamageType.Electricity]` => `anim_zombie_base` => `[death_electric]`~~
  * ~~`DeathAnimations[DamageType.Acid]` => `anim_zombie_base` => `[death_acid]`~~
  * `DeathAnimations[DamageType.Spikes]` => `[fall_death]`
  * ~~`DeathAnimations[DamageType.Fire]` => `anim_zombie_base` => `[death_fire]`~~
  * ~~`DeathAnimations[DamageType.HolyFire]` => `anim_zombie_base` => `[death_fire]`~~
  * ~~`DeathAnimations[DamageType.Radioactivity]` => `anim_zombie_base` => `[death_acid]`~~
  * `DeathAnimations[DamageType.Spit]` => `[contamination]`
  * `AirDeathAnimation` => `[]`
  * `AirDeathLandingAnimation` => `[]`

- `UnarmedAnimations`
  * `AnimationLibrary` => `anim_civil`
  * `CommonAnimations[Animations.Spawn]` => `[rise]`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Idle]` => `[stand, stand_random_1, stand_random_2, stand_random_3]`
  * `CommonAnimations[Animations.Turn]` => `[turn]`
  * `CommonAnimations[Animations.Walk]` => `[walk]`
  * `CommonAnimations[Animations.Run]` => `[run]`
  * `CommonAnimations[Animations.StepOver]` => `[stepover]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high]`
  * `CommonAnimations[Animations.AlertStart]` => `[alert_start]`
  * `CommonAnimations[Animations.AlertStartTurn]` => `[alert_start_turn]`
  * `CommonAnimations[Animations.Alerted]` => `[alert_stand]`
  * `CommonAnimations[Animations.AlertEnd]` => `[alert_end]`
  * `CommonAnimations[Animations.Scared]` => `[scared]`
  * `CommonAnimations[Animations.Paralised]` => `[paralised]`
  * `CommonAnimations[Animations.ParalisedEnd]` => `[paralised_stop]`
  * `CommonAnimations[Animations.Grabbed]` => `[grasped]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `DefaultHitAnimation` => `[attacked]`
  * `HitAnimations[DamageType.Fire]` => `[]`
  * `HitAnimations[DamageType.Acid]` => `[]`
  * `HitAnimations[DamageType.Radioactivity]` => `[]`
  * `HitAnimations[DamageType.Spit]` => `[]`
  * `DefaultDeathAnimation` => `[death]`
  * `DeathAnimations[DamageType.Fall]` => `[fall_death]`
  * `DeathAnimations[DamageType.Contamination]` => `[contamination]`
  * ~~`DeathAnimations[DamageType.Plasma]` => `anim_zombie_base` => `[death_plasma]`~~
  * ~~`DeathAnimations[DamageType.Laser]` => `anim_zombie_base` => `[death_plasma]`~~
  * `DeathAnimations[DamageType.TankDash]` => `[fall_death]`
  * `DeathAnimations[DamageType.Squashed]` => `[fall_death]`
  * ~~`DeathAnimations[DamageType.Electricity]` => `anim_zombie_base` => `[death_electric]`~~
  * ~~`DeathAnimations[DamageType.Acid]` => `anim_zombie_base` => `[death_acid]`~~
  * `DeathAnimations[DamageType.Spikes]` => `[fall_death]`
  * ~~`DeathAnimations[DamageType.Radioactivity]` => `anim_zombie_base` => `[death_acid]`~~
  * ~~`DeathAnimations[DamageType.Fire]` => `anim_zombie_base` => `[death_fire]`~~
  * ~~`DeathAnimations[DamageType.HolyFire]` => `anim_zombie_base` => `[death_fire]`~~
  * `DeathAnimations[DamageType.Spit]` => `[contamination]`
  * `AirDeathAnimation` => `[]`
  * `AirDeathLandingAnimation` => `[]`

- `BulkyMeleeAnimations`
  * `AnimationLibrary` => `anim_melee`
  * `CommonAnimations[Animations.Spawn]` => `[rise]`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Idle]` => `[stand, stand_random_1, stand_random_2, stand_random_3]`
  * `CommonAnimations[Animations.Turn]` => `[turn]`
  * `CommonAnimations[Animations.Walk]` => `[walk]`
  * `CommonAnimations[Animations.Run]` => `[run]`
  * `CommonAnimations[Animations.StepOver]` => `[stepover]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high]`
  * `CommonAnimations[Animations.AlertStart]` => `[alert_start]`
  * `CommonAnimations[Animations.AlertStartTurn]` => `[alert_start_turn]`
  * `CommonAnimations[Animations.Alerted]` => `[alert_stand]`
  * `CommonAnimations[Animations.AlertEnd]` => `[alert_end]`
  * `CommonAnimations[Animations.Scared]` => `[scared]`
  * `CommonAnimations[Animations.Paralised]` => `[paralised]`
  * `CommonAnimations[Animations.ParalisedEnd]` => `[paralised_stop]`
  * `CommonAnimations[Animations.Grabbed]` => `[grasped]`
  * `CommonAnimations[Animations.Attack]` => `[shoot, shoot_2]`
  * `CommonAnimations[Animations.AimStart]` => `[aim_start]`
  * `CommonAnimations[Animations.Aim]` => `[aim]`
  * `CommonAnimations[Animations.AimStop]` => `[aim_stop]`
  * `CommonAnimations[Animations.Reload]` => `[reload]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `DefaultHitAnimation` => `[attacked]`
  * `HitAnimations[DamageType.Fire]` => `[]`
  * `HitAnimations[DamageType.Acid]` => `[]`
  * `HitAnimations[DamageType.Radioactivity]` => `[]`
  * `HitAnimations[DamageType.Spit]` => `[]`
  * `DefaultDeathAnimation` => `[death]`
  * `DeathAnimations[DamageType.Fall]` => `[fall_death]`
  * `DeathAnimations[DamageType.Contamination]` => `[contamination]`
  * ~~`DeathAnimations[DamageType.Plasma]` => `anim_zombie_base` => `[death_plasma]`~~
  * ~~`DeathAnimations[DamageType.Laser]` => `anim_zombie_base` => `[death_plasma]`~~
  * `DeathAnimations[DamageType.TankDash]` => `[fall_death]`
  * `DeathAnimations[DamageType.Squashed]` => `[fall_death]`
  * ~~`DeathAnimations[DamageType.Electricity]` => `anim_men_in_black_2` => `[death_electricity]`~~
  * `DeathAnimations[DamageType.Spikes]` => `[fall_death]`
  * `AirDeathAnimation` => `[]`
  * `AirDeathLandingAnimation` => `[]`

- `DroneAnimations`
  * `AnimationLibrary` => `anim_shotgun`
  * `CommonAnimations[Animations.Spawn]` => `[rise]`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Idle]` => `[stand, stand_random_1, stand_random_2, stand_random_3]`
  * `CommonAnimations[Animations.Turn]` => `[turn]`
  * `CommonAnimations[Animations.Walk]` => `[walk]`
  * `CommonAnimations[Animations.Run]` => `[run]`
  * `CommonAnimations[Animations.StepOver]` => `[stepover]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high]`
  * `CommonAnimations[Animations.AlertStart]` => `[alert_start]`
  * `CommonAnimations[Animations.AlertStartTurn]` => `[alert_start_turn]`
  * `CommonAnimations[Animations.Alerted]` => `[alert_stand]`
  * `CommonAnimations[Animations.AlertEnd]` => `[alert_end]`
  * `CommonAnimations[Animations.Scared]` => `[scared]`
  * `CommonAnimations[Animations.Paralised]` => `[paralised]`
  * `CommonAnimations[Animations.ParalisedEnd]` => `[paralised_stop]`
  * `CommonAnimations[Animations.Grabbed]` => `[grasped]`
  * `CommonAnimations[Animations.Attack]` => `[shoot]`
  * `CommonAnimations[Animations.AimStart]` => `[aim_start]`
  * `CommonAnimations[Animations.Aim]` => `[aim]`
  * `CommonAnimations[Animations.AimStop]` => `[aim_stop]`
  * `CommonAnimations[Animations.Reload]` => `[reload]`
  * `CommonAnimations[Animations.ReloadEnd]` => `[reload_end]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `DefaultHitAnimation` => `[attacked]`
  * `HitAnimations[DamageType.Fire]` => `[]`
  * `HitAnimations[DamageType.Acid]` => `[]`
  * `HitAnimations[DamageType.Radioactivity]` => `[]`
  * `HitAnimations[DamageType.Spit]` => `[]`
  * `DefaultDeathAnimation` => `[death]`
  * ~~`DeathAnimations[DamageType.Fall]` => `anim_drone` => `[death]`~~
  * ~~`DeathAnimations[DamageType.Contamination]` => `anim_drone` => `[death]`~~
  * ~~`DeathAnimations[DamageType.Plasma]` => `anim_drone` => `[plasma_death]`~~
  * ~~`DeathAnimations[DamageType.TankDash]` => `anim_drone` => `[fall_death]`~~
  * ~~`DeathAnimations[DamageType.Squashed]` => `anim_drone` => `[fall_death]`~~
  * ~~`DeathAnimations[DamageType.Electricity]` => `anim_drone` => `[death]`~~
  * ~~`DeathAnimations[DamageType.Acid]` => `anim_drone` => `[death]`~~
  * ~~`DeathAnimations[DamageType.Spikes]` => `anim_drone` => `[fall_death]`~~
  * ~~`DeathAnimations[DamageType.Laser]` => `anim_drone` => `[plasma_death]`~~
  * `AirDeathAnimation` => `[]`
  * `AirDeathLandingAnimation` => `[]`

- `TerminatorAnimations`
  * `AnimationLibrary` => `anim_shotgun`
  * `CommonAnimations[Animations.Spawn]` => `[rise]`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Idle]` => `[stand, stand_random_1, stand_random_2, stand_random_3]`
  * `CommonAnimations[Animations.Turn]` => `[turn]`
  * `CommonAnimations[Animations.Walk]` => `[walk]`
  * `CommonAnimations[Animations.Run]` => `[run]`
  * `CommonAnimations[Animations.StepOver]` => `[stepover]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high]`
  * `CommonAnimations[Animations.AlertStart]` => `[alert_start]`
  * `CommonAnimations[Animations.AlertStartTurn]` => `[alert_start_turn]`
  * `CommonAnimations[Animations.Alerted]` => `[alert_stand]`
  * `CommonAnimations[Animations.AlertEnd]` => `[alert_end]`
  * `CommonAnimations[Animations.Scared]` => `[scared]`
  * `CommonAnimations[Animations.Paralised]` => `[paralised]`
  * `CommonAnimations[Animations.ParalisedEnd]` => `[paralised_stop]`
  * `CommonAnimations[Animations.Grabbed]` => `[grasped]`
  * `CommonAnimations[Animations.Attack]` => `[shoot]`
  * `CommonAnimations[Animations.AimStart]` => `[aim_start]`
  * `CommonAnimations[Animations.Aim]` => `[aim]`
  * `CommonAnimations[Animations.AimStop]` => `[aim_stop]`
  * `CommonAnimations[Animations.Reload]` => `[reload]`
  * `CommonAnimations[Animations.ReloadEnd]` => `[reload_end]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `DefaultHitAnimation` => `[attacked]`
  * `HitAnimations[DamageType.Fire]` => `[]`
  * `HitAnimations[DamageType.Acid]` => `[]`
  * `HitAnimations[DamageType.Radioactivity]` => `[]`
  * `HitAnimations[DamageType.Spit]` => `[]`
  * `DefaultDeathAnimation` => `[death]`
  * `AirDeathAnimation` => `[]`
  * `AirDeathLandingAnimation` => `[]`

- `ZombieAnimations`
  * `AnimationLibrary` => `anim_zombie_base`
  * `CommonAnimations[Animations.Spawn]` => `[rise_1, rise_2, rise_3, rise_4, space_time_teleport]`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Walk]` => `[walk_1, walk_2, walk_3, walk_4]`
  * `CommonAnimations[Animations.Run]` => `[run]`
  * `CommonAnimations[Animations.Sprint]` => `[sprint]`
  * `CommonAnimations[Animations.Decelerate]` => `[sprint_stop_1, sprint_stop_2, sprint_stop_3, sprint_stop_4]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low_1, fall_landing_low_2, fall_landing_low_3, fall_landing_low_4]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high_1, fall_landing_high_2, fall_landing_high_3, fall_landing_high_4]`
  * `CommonAnimations[Animations.FallLandingSprint]` => `[sprint_fall_landing]`
  * `CommonAnimations[Animations.Jump]` => `[jump_part1]`
  * `CommonAnimations[Animations.JumpFall]` => `[jump_part2]`
  * `CommonAnimations[Animations.Attack]` => `[attack]`
  * `CommonAnimations[Animations.AttackProp]` => `[attack_door]`
  * `CommonAnimations[Animations.AttackSprint]` => `[sprint_attack]`
  * `CommonAnimations[Animations.ContaminationRise]` => `[contamination]`
  * `CommonAnimations[Animations.Explode]` => `[transform_boomer]`
  * `CommonAnimations[Animations.Sacrifice]` => `[sacrifice]`
  * `CommonAnimations[Animations.Spit]` => `[spit]`
  * `CommonAnimations[Animations.Scream]` => `[transform_screamer]`
  * `CommonAnimations[Animations.Reborn]` => `[reborn_1, reborn_2, reborn_3, reborn_4]`
  * `CommonAnimations[Animations.StepOver]` => `[stepover_1, stepover_2, stepover_3, stepover_4]`
  * `CommonAnimations[Animations.StepOverSprint]` => `[sprint_stepover]`
  * `CommonAnimations[Animations.SpitAim]` => `[spit_aim]`
  * `CommonAnimations[Animations.Idle]` => `[stand]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `DefaultHitAnimation` => `[hit_1, hit_2, hit_3, hit_4]`
  * `HitAnimations[DamageType.Fire]` => `[]`
  * `HitAnimations[DamageType.Acid]` => `[]`
  * `HitAnimations[DamageType.Radioactivity]` => `[]`
  * `DefaultDeathAnimation` => `[death_1, death_2, death_3]`
  * `DeathAnimations[DamageType.Fall]` => `[death_fall]`
  * `DeathAnimations[DamageType.Electricity]` => `[death_electric]`
  * `DeathAnimations[DamageType.Fire]` => `[death_fire]`
  * `DeathAnimations[DamageType.Acid]` => `[death_acid]`
  * `DeathAnimations[DamageType.Sacrifice]` => `[death_sacrifice]`
  * `DeathAnimations[DamageType.Radioactivity]` => `[death_acid]`
  * `DeathAnimations[DamageType.Shotgun]` => `[death_shotgun, death_2]`
  * `DeathAnimations[DamageType.Sword]` => `[death_cut_1, death_cut_2, death_shotgun]`
  * `DeathAnimations[DamageType.Spikes]` => `[death_fall]`
  * `DeathAnimations[DamageType.Plasma]` => `[death_plasma]`
  * `DeathAnimations[DamageType.Ripped]` => `[death_ripped]`
  * `DeathAnimations[DamageType.MachineGun]` => `[death_1, death_2, death_3, death_shotgun]`
  * `DeathAnimations[DamageType.Squashed]` => `[death_fall]`
  * `DeathAnimations[DamageType.Explosion]` => `[death_air_falling]`
  * `DeathAnimations[DamageType.Canon]` => `[death_air_falling]`
  * `DeathAnimations[DamageType.Laser]` => `[death_plasma]`
  * `DeathAnimations[DamageType.HolyFire]` => `[death_fire]`
  * `AirDeathAnimation` => `[death_air_falling]`
  * `AirDeathLandingAnimation` => `[death_air_landing]`

- `CrawlerAnimations`
  * `AnimationLibrary` => `anim_zombie_climber`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Walk]` => `[walk_1]`
  * `CommonAnimations[Animations.Run]` => `[run]`
  * `CommonAnimations[Animations.StepOver]` => `[stepover_1]`
  * `CommonAnimations[Animations.StepOverSprint]` => `[sprint_stepover]`
  * `CommonAnimations[Animations.Sprint]` => `[sprint]`
  * `CommonAnimations[Animations.Decelerate]` => `[sprint_stop_1]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low_1]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high_1]`
  * `CommonAnimations[Animations.FallLandingSprint]` => `[sprint_fall_landing]`
  * `CommonAnimations[Animations.ClimbStart]` => `[climb_start]`
  * `CommonAnimations[Animations.ClimbStartRun]` => `[climb_start_run]`
  * `CommonAnimations[Animations.Climb]` => `[climb]`
  * `CommonAnimations[Animations.ClimbOut]` => `[climb_out]`
  * `CommonAnimations[Animations.ClimbOutRun]` => `[climb_out_run]`
  * `CommonAnimations[Animations.ClimbRun]` => `[climb_run]`
  * `CommonAnimations[Animations.Jump]` => `[jump_part1]`
  * `CommonAnimations[Animations.JumpFall]` => `[jump_part2]`
  * `CommonAnimations[Animations.JumpWallReception]` => `[jump_wall_reception]`
  * `CommonAnimations[Animations.Attack]` => `[attack]`
  * `CommonAnimations[Animations.AttackSprint]` => `[sprint_attack]`
  * `CommonAnimations[Animations.Sacrifice]` => `[sacrifice]`
  * `CommonAnimations[Animations.Explode]` => `[transform_boomer]`
  * `CommonAnimations[Animations.Scream]` => `[transform_screamer]`
  * `CommonAnimations[Animations.CrawlerTransformm]` => `[transform_climber]`
  * `CommonAnimations[Animations.ScreamClimb]` => `[transform_screamer_climbing]`
  * `CommonAnimations[Animations.SpitAim]` => `[spit_aim]`
  * `CommonAnimations[Animations.Spit]` => `[spit]`
  * `CommonAnimations[Animations.Idle]` => `[stand]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `DefaultHitAnimation` => `[hit_1]`
  * `HitAnimations[DamageType.Fire]` => `[]`
  * `HitAnimations[DamageType.Acid]` => `[]`
  * `HitAnimations[DamageType.Radioactivity]` => `[]`
  * `DefaultDeathAnimation` => `[death_1]`
  * `DeathAnimations[DamageType.Electricity]` => `[death_electric]`
  * ~~`DeathAnimations[DamageType.Laser]` => `anim_zombie_base` => `[death_plasma]`~~
  * `DeathAnimations[DamageType.Shotgun]` => `[death_shotgun, death_1]`
  * `DeathAnimations[DamageType.Spikes]` => `[fall_death]`
  * `DeathAnimations[DamageType.Sacrifice]` => `[sacrifice_death]`
  * ~~`DeathAnimations[DamageType.Plasma]` => `anim_zombie_base` => `[death_plasma]`~~
  * `DeathAnimations[DamageType.Ripped]` => `[death_ripped]`
  * `DeathAnimations[DamageType.Sword]` => `[death_laser_1]`
  * `DeathAnimations[DamageType.MachineGun]` => `[death_1, death_shotgun]`
  * `DeathAnimations[DamageType.Squashed]` => `[fall_death]`
  * `DeathAnimations[DamageType.Canon]` => `[death_air_falling]`
  * `DeathAnimations[DamageType.Explosion]` => `[death_air_falling]`
  * `DeathAnimations[DamageType.Fire]` => `[death_fire]`
  * `DeathAnimations[DamageType.Acid]` => `[death_acid]`
  * `DeathAnimations[DamageType.Radioactivity]` => `[death_acid]`
  * `DeathAnimations[DamageType.HolyFire]` => `[death_fire]`
  * `AirDeathAnimation` => `[death_air_falling]`
  * `AirDeathLandingAnimation` => `[death_air_landing]`

- `OverlordAnimations`
  * `AnimationLibrary` => `anim_zombie_blocker`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Walk]` => `[walk]`
  * `CommonAnimations[Animations.Sprint]` => `[sprint]`
  * `CommonAnimations[Animations.Explode]` => `[transform_boomer]`
  * `CommonAnimations[Animations.Scream]` => `[transform_screamer]`
  * `CommonAnimations[Animations.Sacrifice]` => `[sacrifice]`
  * `CommonAnimations[Animations.Jump]` => `[jump]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low_1]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high_1]`
  * `CommonAnimations[Animations.Run]` => `[sprint]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.OverlordTransfom]` => `[transform_blocker]`
  * `CommonAnimations[Animations.Idle]` => `[stand]`
  * `CommonAnimations[Animations.Spit]` => `[spit]`
  * `CommonAnimations[Animations.SpitAim]` => `[spit_aim]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `DefaultHitAnimation` => `[]`
  * `DefaultDeathAnimation` => `[death_1]`
  * `DeathAnimations[DamageType.Sacrifice]` => `[sacrifice_death]`
  * ~~`DeathAnimations[DamageType.Plasma]` => `anim_zombie_base` => `[death_plasma]`~~
  * `DeathAnimations[DamageType.Ripped]` => `[death_ripped]`
  * ~~`DeathAnimations[DamageType.Laser]` => `anim_zombie_base` => `[death_plasma]`~~
  * `DeathAnimations[DamageType.Fire]` => `[death_fire]`
  * `DeathAnimations[DamageType.Acid]` => `[death_acid]`
  * `DeathAnimations[DamageType.Radioactivity]` => `[death_acid]`
  * `DeathAnimations[DamageType.HolyFire]` => `[death_fire]`
  * `AirDeathAnimation` => `[]`
  * `AirDeathLandingAnimation` => `[]`

- `TankAnimations`
  * `AnimationLibrary` => `anim_zombie_tank`
  * `CommonAnimations[Animations.Stand]` => `[stand]`
  * `CommonAnimations[Animations.Walk]` => `[walk_1]`
  * `CommonAnimations[Animations.Run]` => `[run]`
  * `CommonAnimations[Animations.SprintStart]` => `[dash_start]`
  * `CommonAnimations[Animations.Sprint]` => `[dash]`
  * `CommonAnimations[Animations.Decelerate]` => `[dash_end]`
  * `CommonAnimations[Animations.StepOver]` => `[stepover_1]`
  * `CommonAnimations[Animations.Fall]` => `[fall]`
  * `CommonAnimations[Animations.FallLandingLow]` => `[fall_landing_low_1]`
  * `CommonAnimations[Animations.FallLandingHigh]` => `[fall_landing_high_1]`
  * `CommonAnimations[Animations.Jump]` => `[jump_part1]`
  * `CommonAnimations[Animations.JumpFall]` => `[jump_part2]`
  * `CommonAnimations[Animations.JumpReception]` => `[jump_part3]`
  * `CommonAnimations[Animations.Attack]` => `[attack]`
  * `CommonAnimations[Animations.AttackProp]` => `[attack_wall]`
  * `CommonAnimations[Animations.TankTransform]` => `[transform_tank]`
  * `CommonAnimations[Animations.Explode]` => `[transform_boomer]`
  * `CommonAnimations[Animations.Sacrifice]` => `[sacrifice]`
  * `CommonAnimations[Animations.Scream]` => `[transform_screamer]`
  * `CommonAnimations[Animations.HitWall]` => `[dash_stun]`
  * `CommonAnimations[Animations.Idle]` => `[stand]`
  * `CommonAnimations[Animations.Empty]` => `[empty]`
  * `CommonAnimations[Animations.Spit]` => `[spit]`
  * `CommonAnimations[Animations.SpitAim]` => `[spit_aim]`
  * `DefaultHitAnimation` => `[]`
  * `DefaultDeathAnimation` => `[death_1]`
  * `DeathAnimations[DamageType.Sacrifice]` => `[sacrifice_death]`
  * `DeathAnimations[DamageType.Fall]` => `[fall_death]`
  * `DeathAnimations[DamageType.Spikes]` => `[fall_death]`
  * `DeathAnimations[DamageType.Plasma]` => `[death_plasma]`
  * `DeathAnimations[DamageType.Ripped]` => `[death_ripped]`
  * `DeathAnimations[DamageType.Squashed]` => `[fall_death]`
  * `DeathAnimations[DamageType.Electricity]` => `[death_electricity]`
  * `DeathAnimations[DamageType.Laser]` => `[death_plasma]`
  * `DeathAnimations[DamageType.Fire]` => `[death_fire]`
  * `DeathAnimations[DamageType.Acid]` => `[death_acid]`
  * `DeathAnimations[DamageType.Radioactivity]` => `[death_acid]`
  * `DeathAnimations[DamageType.HolyFire]` => `[death_fire]`
  * `AirDeathAnimation` => `[]`
  * `AirDeathLandingAnimation` => `[]`