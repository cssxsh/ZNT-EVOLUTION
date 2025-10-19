# Vision

> 视觉

| Prefab       | Layers                                                           | WithoutTags    | WithoutTags                          |
|:-------------|:-----------------------------------------------------------------|:---------------|:-------------------------------------|
| `Human`      | `Zombie, Crate, Gameplay, Stairs Top, Block Humans, World Enemy` | `IgnoreAttack` | `Zombie, WorldEnemy`                 |
| `WorldEnemy` | `Zombie, Human, Gameplay, Stairs Top, Block Humans`              |                | `Human, Zombie`                      |
| `Zombie`     | `Human, Prop, World Enemy`                                       | `CannotAttack` | `Door, Human, Breakable, WorldEnemy` |
| `SentryGun`  | `3, 6, 7, Zombie, Block Explosion`                               |                | `Zombie`                             |