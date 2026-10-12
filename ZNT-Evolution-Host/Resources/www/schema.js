/* =========================================================
 *  类型定义（Schema）
 *  ---------------------------------------------------------
 *  keys 中每个键可定义为：
 *    { type: 'text',    desc, pattern, message, placeholder }
 *    { type: 'bool',    desc }
 *    { type: 'number',  desc, min, max, step, placeholder }
 *    { type: 'enum',    desc, options: [...] }
 *    { type: 'flags',   desc, options: [...] }
 *    { type: 'range',   desc, min, max, placeholder }
 *    { type: 'vector2', desc }
 *    { type: 'vector3', desc }
 *  键值必须是上面的对象写法
 * ======================================================= */

const SCHEMA = {
    HumanAsset: {
        desc: "人类 / 角色类实体资源",
        keys: {
            "Attitude": {
                type: "enum",
                options: ["Combative", "Neutral", "Fearfull", "Ignore"]
            },
            "DangerRadius": {
                type: "number",
                min: 0.0
            },
            "AlertRadius": {
                type: "number",
                min: 0.0
            },
            "AlertRelayRadius": {
                type: "number",
                min: 0.0
            },
            "AlertRelayRatio": {
                type: "number",
                min: 0.0
            },
            "RelayAlertOverTime": {
                type: "bool"
            },
            "ParalyseDuration": {
                type: "number",
                min: 0.0
            },
            "AlertDuration": {
                type: "number",
                min: 0.0
            },
            "FleeDuration": {
                type: "number",
                min: 0.0
            },
            "ScaredDuration": {
                type: "number",
                min: 0.0
            },
            "Contaminable": {
                type: "bool"
            },
            "AllowMultipleAttackers": {
                type: "bool"
            },
            "GrabbedOnAttacked": {
                type: "bool"
            },
            "IgnoreDamages": {
                type: "bool",
                desc: "忽略攻击，取消硬直动画"
            },
            "InvincibleOnAttack": {
                type: "bool",
                desc: "攻击时无敌"
            },
            "ResistScream": {
                type: "bool"
            },
            "FleeBeforeZombieExplode": {
                type: "bool",
                desc: "僵尸爆炸前逃离"
            },
            "ReportAlert": {
                type: "bool"
            },
            "AllowRage": {
                type: "bool"
            },
            "FreezeOnRage": {
                type: "bool"
            },
            "RageDamageType": {
                type: "flags",
                desc: "攻击类型，可多选",
                options: [
                    "None", "Fall", "Bite", "Gun", "Rifle", "Shotgun", "Melee", "Sword", "Canon", "Explosion", "Spikes",
                    "Fire", "Electricity", "Laser", "Sentry", "Contamination", "Sacrifice", "Acid", "Radioactivity",
                    "Ripped", "Plasma", "Squashed", "TankDash", "MachineGun", "Crawler", "Tank", "Boomer", "Spit",
                    "HolyFire"
                ]
            },
            "RageRefillTimer": {
                type: "number",
                min: 0.0
            },
            "RageRefillRate": {
                type: "number",
                step: 1,
                min: 0
            },
            "RageHitCount": {
                type: "number",
                step: 1,
                min: 0
            },
            "RefillOnEnraged": {
                type: "bool"
            },
            "RageRepulsion": {
                type: "text",
                pattern: "^[^:]+ : ExplosionAsset$",
                placeholder: "xxxRepulse : ExplosionAsset",
                message: "格式为 '资源名 : 类型名'"
            },
            "VisionFollowTarget": {
                type: "bool"
            },
            "VisionAngleRange": {
                type: "range"
            },
            "OverrideOnAim": {
                type: "bool"
            },
            "AimFieldOfView": {
                type: "number",
                desc: "瞄准视野",
                min: 0.0
            },
            "AimRayCount": {
                type: "number",
                step: 1,
                min: 0
            },
            "AimDistance": {
                type: "number",
                desc: "瞄准距离",
                min: 0.0
            },
            "MaxFallHeight": {
                type: "number",
                desc: "最大下坠高度",
                min: 0.0
            },
            "RandomFlip": {
                type: "bool"
            },
            "FlipTimeRange": {
                type: "range"
            },
            "AdnGain": {
                type: "number",
                step: 1,
                min: 0
            },
            "RiseOnDeath": {
                type: "bool",
                desc: "死亡时感染"
            },
            "RiseAsset": {
                type: "text",
                desc: "感染后转变的单位",
                pattern: "^[^:]+ : (?:ZombieAsset|HumanAsset)$",
                placeholder: "Zombie : ZombieAsset",
                message: "格式为 '资源名 : 类型名'"
            },
            "TimeToRise": {
                type: "range"
            },
            "RemoveCorpse": {
                type: "bool"
            },
            "ReloadType": {
                type: "enum",
                desc: "换弹类型",
                options: ["Automatic", "ShellByShell", "Stamina"]
            },
            "StaminaRefillTimer": {
                type: "number",
                desc: "补弹时间"
            },
            "ReloadTime": {
                type: "number",
                desc: "换弹时间",
                min: 0.0
            },
            "DirectAim": {
                type: "bool",
                desc: "直接瞄准"
            },
            "AimStopTime": {
                type: "number",
                min: 0.0
            },
            "MagazineSize": {
                type: "number",
                desc: "弹匣容量",
                step: 1,
                min: 0
            },
            "RandomSeed": {
                type: "number",
                step: 1
            },
            "BlockOpponents": {
                type: "bool",
                desc: "阻挡"
            },
            "MaxOpponentsBlock": {
                type: "number",
                desc: "最大阻挡数量",
                step: 1,
                min: 0
            },
            "MoveTowardStaticTargets": {
                type: "bool"
            },
            "RemoveTargetOnKill": {
                type: "bool"
            },
            "Attachments[*]": {
                type: "text",
                desc: "附着",
                pattern: "^[^:]+ : UnityEngine\\.GameObject$",
                placeholder: "xxx : UnityEngine.GameObject",
                message: "格式为 '资源名 : 类型名'"
            },
            "ThrowableObjects[*]": {
                type: "text",
                desc: "投掷物，用 [序号] 指定条目，例如 ThrowableObjects[0]",
                pattern: "^[^:]+ : PhysicObjectAsset$",
                placeholder: "xxx : PhysicObjectAsset",
                message: "格式为 '资源名 : 类型名'"
            },
            "ExplosionAssets[*]": {
                type: "text",
                desc: "自爆，用 [序号] 指定条目，例如 ExplosionAssets[0]",
                pattern: "^[^:]+ : ExplosionAsset$",
                placeholder: "xxxExplosion : ExplosionAsset",
                message: "格式为 '资源名 : 类型名'"
            },
            "WaitAnimTime": {
                type: "range"
            },
            "AlertIconOffset": {
                type: "vector2"
            },
            "Voice": {
                type: "enum",
                desc: "语音类型",
                options: [
                    "None", "Woman1", "Woman2", "Man1", "Man2", "Survivor1", "Survivor2", "Bouncer", "Cop", "Hazmat",
                    "Doctor", "Computer", "Dj", "Clown", "Terminator", "DrugLord", "Gertrude", "Chemist",
                    "ChemistMutated", "ChemistFinal", "Michelle", "MachineGunner", "Director", "Preacher", "Homeless"
                ]
            },
            "CharacterType": {
                type: "enum",
                options: [
                    "Zombie", "Civilian", "ArmedCivilian", "Survivor", "Cop", "MenInBlack", "Inhuman", "Boss",
                    "Astronaut", "Cultist"
                ],
            },
            "Height": {
                type: "number",
                desc: "身高",
                min: 0.0
            },
            "Hp": {
                type: "number",
                min: 0.0
            },
            "Invincible": {
                type: "bool",
                desc: "无敌"
            },
            "DamageMultipliers[*]": {
                type: "number",
                desc: "伤害倍率，用 [DamageType] 指定条目，例如 DamageMultipliers[Spit]"
            },
            "Orientation": {
                type: "vector3"
            },
            "WalkSpeed": {
                type: "number",
                desc: "散步速度",
                min: 0.0
            },
            "RunSpeed": {
                type: "number",
                desc: "跑步速度",
                min: 0.0
            },
            "MoveOnStart": {
                type: "bool"
            },
            "CanClimb": {
                type: "bool",
                desc: "可爬行"
            },
            "CanStepOver": {
                type: "bool",
                desc: "可跨过"
            },
            "DeathVelocityThreshold": {
                type: "number",
                desc: "死亡速率阈值",
                min: 0.0
            },
            "JumpDeathVelocityThreshold": {
                type: "number",
                desc: "跳跃死亡速率阈值",
                min: 0.0
            },
            "ColliderSkinWidth": {
                type: "number",
                min: 0.0
            },
            "SlopeDetectionOffset": {
                type: "number",
                min: 0.0
            },
            "StairDetectionHeightRatio": {
                type: "number",
                desc: "楼梯高度判定",
                min: 0.0
            },
            "GeneralDirection": {
                type: "vector3"
            },
            "DeviationAngle": {
                type: "number",
                min: 0.0
            },
            "FieldOfView": {
                type: "number",
                desc: "视野",
                min: 0.0
            },
            "Distance": {
                type: "number",
                desc: "视野距离",
                min: 0.0
            },
            "RayCount": {
                type: "number",
                step: 1,
                min: 0
            },
            "DamageType": {
                type: "enum",
                options: [
                    "None", "Fall", "Bite", "Gun", "Rifle", "Shotgun", "Melee", "Sword", "Canon", "Explosion", "Spikes",
                    "Fire", "Electricity", "Laser", "Sentry", "Contamination", "Sacrifice", "Acid", "Radioactivity",
                    "Ripped", "Plasma", "Squashed", "TankDash", "MachineGun", "Crawler", "Tank", "Boomer", "Spit",
                    "HolyFire"
                ],
                placeholder: "Fire",
            },
            "EnemyTag": {
                type: "flags",
                desc: "敌对标签，可多选",
                options: [
                    "Door", "IgnoreAttack", "Character", "Human", "Zombie", "Interactable", "Breakable", "Tile",
                    "Decor", "Slope", "GameplayTile", "NoForce", "Indestructible", "Destructible", "CannotAttack",
                    "Sneaky", "StaticCharacter", "WorldEnemy", "IgnoreProjectile", "Corpse", "Vomit", "Vip",
                    "Projectile", "NoDanger", "IgnoreUiRaycast"
                ]
            },
            "HitMultipleTargets": {
                type: "bool",
                desc: "同时攻击多个目标"
            },
            "NextTargetsDamageMultiplier": {
                type: "number",
                min: 0.0
            },
            "MaxTargets": {
                type: "number",
                step: 1,
                min: 0
            },
            "Damage": {
                type: "number",
                min: 0.0
            },
            "AimRange": {
                type: "number",
                desc: "瞄准射程",
                min: 0.0
            },
            "AttackRange": {
                type: "number",
                desc: "攻击射程",
                min: 0.0
            },
            "DamageRange": {
                type: "number",
                desc: "破坏射程",
                min: 0.0
            },
            "MovingAttackRange": {
                type: "number",
                desc: "移动攻击射程",
                min: 0.0
            },
            "DamageFalloff": {
                type: "text",
                pattern: "^\\([^)]*\\)\\s*[-~]\\s*\\([^)]*\\)$",
                placeholder: "(0, 1) - (1, 1)",
                message: "形如 \"(0, 1) - (1, 1)\" 或 \"(0, 0, 0) ~ (1, 1, 1)\"",
            },
            "AttackFrequency": {
                type: "number",
                min: 0.0
            },
            "SpriteCollection": {
                type: "text",
                pattern: "^[^:]+ : tk2dSpriteCollectionData$",
                placeholder: "sprites_xxx : tk2dSpriteCollectionData",
                message: "格式为 '资源名 : 类型名'"
            },
            "SpriteIndex": {
                type: "number",
                step: 1,
                min: 0
            },
            "AnimationLibrary": {
                type: "text",
                pattern: "^[^:]+ : tk2dSpriteAnimation$",
                placeholder: "anim_xxx : tk2dSpriteAnimation",
                message: "格式为 \"资源名 : 类型名\"",
                desc: undefined,
            },
            "Animations": {
                type: "text",
                pattern: "^[^:]+ : CharacterAnimationAsset$",
                placeholder: "xxxAnimations : CharacterAnimationAsset",
                message: "格式为 \"资源名 : 类型名\"",
                desc: undefined,
            },
            "Prefab": {
                type: "enum",
                desc: "预制体",
                options: ["Human : UnityEngine.Transform", "WorldEnemy : UnityEngine.Transform"]
            },
            "Tag": {
                type: "flags",
                desc: "标签，可多选",
                options: [
                    "Door", "IgnoreAttack", "Character", "Human", "Zombie", "Interactable", "Breakable", "Tile",
                    "Decor", "Slope", "GameplayTile", "NoForce", "Indestructible", "Destructible", "CannotAttack",
                    "Sneaky", "StaticCharacter", "WorldEnemy", "IgnoreProjectile", "Corpse", "Vomit", "Vip",
                    "Projectile", "NoDanger", "IgnoreUiRaycast"
                ]
            },
            "Layer": {
                type: "enum",
                desc: "图层",
                options: [
                    "Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "Zombie", "Human", "Ignore Collisions",
                    "Ignore Characters", "Zombie Stopper", "Renderer", "Crate", "Gameplay", "Prop", "Foreground",
                    "One Way", "Exterior", "Stairs", "Stairs Top", "Middleground", "Background", "Sky", "Projectile",
                    "Spit", "Block Humans", "Block Zombies", "World Enemy", "Moving Trap", "Block Explosion"
                ]
            }
        }
    }
};

/* ---------------------------------------------------------
 *  示例数据：空列表时「载入示例数据」用它，导入框的占位提示也用它
 *  （不能塞进 SCHEMA 里 —— normalizeSchema 会把顶层的每个键当成一种资源类型）
 * ------------------------------------------------------- */
const SAMPLE = [
    {
        Type: "HumanAsset",
        Pattern: "CivilianFemale.*",
        Handles: {
            "RiseAsset": "Zombie : ZombieAsset",
        }
    }
];
