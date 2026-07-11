# Layer

> 图层

| Index  | Mask         |        Name         |
|:-------|:-------------|:-------------------:|
| `0x00` | `0x00000001` |      `Default`      |
| `0x01` | `0x00000002` |   `TransparentFX`   |
| `0x02` | `0x00000004` |  `Ignore Raycast`   |
| `0x03` | `0x00000008` |         `3`         |
| `0x04` | `0x00000010` |       `Water`       |
| `0x05` | `0x00000020` |        `UI`         |
| `0x06` | `0x00000040` |         `6`         |
| `0x07` | `0x00000080` |         `7`         |
| `0x08` | `0x00000100` |      `Zombie`       |
| `0x09` | `0x00000200` |       `Human`       |
| `0x0A` | `0x00000400` | `Ignore Collisions` |
| `0x0B` | `0x00000800` | `Ignore Characters` |
| `0x0C` | `0x00001000` |  `Zombie Stopper`   |
| `0x0D` | `0x00002000` |     `Renderer`      |
| `0x0E` | `0x00004000` |       `Crate`       |
| `0x0F` | `0x00008000` |     `Gameplay`      |
| `0x10` | `0x00010000` |       `Prop`        |
| `0x11` | `0x00020000` |    `Foreground`     |
| `0x12` | `0x00040000` |      `One Way`      |
| `0x13` | `0x00080000` |     `Exterior`      |
| `0x14` | `0x00100000` |      `Stairs`       |
| `0x15` | `0x00200000` |    `Stairs Top`     |
| `0x16` | `0x00400000` |   `Middleground`    |
| `0x17` | `0x00800000` |    `Background`     |
| `0x18` | `0x01000000` |        `Sky`        |
| `0x19` | `0x02000000` |    `Projectile`     |
| `0x1A` | `0x04000000` |       `Spit`        |
| `0x1B` | `0x08000000` |   `Block Humans`    |
| `0x1C` | `0x10000000` |   `Block Zombies`   |
| `0x1D` | `0x20000000` |    `World Enemy`    |
| `0x1E` | `0x40000000` |    `Moving Trap`    |
| `0x1F` | `0x80000000` |  `Block Explosion`  |

## Layer Collision

> 碰撞

| Collision | 0x00 | 0x01 | 0x02 | 0x03 | 0x04 | 0x05 | 0x06 | 0x07 | 0x08 | 0x09 | 0x0A | 0x0B | 0x0C | 0x0D | 0x0E | 0x0F | 0x10 | 0x11 | 0x12 | 0x13 | 0x14 | 0x15 | 0x16 | 0x17 | 0x18 | 0x19 | 0x1A | 0x1B | 0x1C | 0x1D | 0x1E | 0x1F |
|:---------:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|:----:|
| **0x00**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |      |  Y   |  Y   |  Y   |      |      |      |  Y   |  Y   |      |      |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x01**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x02**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x03**  |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |
| **0x04**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |      |      |  Y   |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |  Y   |      |      |  Y   |      |      |
| **0x05**  |      |      |      |  Y   |      |  Y   |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x06**  |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |
| **0x07**  |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |  Y   |
| **0x08**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |      |      |      |      |  Y   |      |  Y   |  Y   |  Y   |      |      |      |  Y   |  Y   |      |      |      |  Y   |      |      |  Y   |      |  Y   |      |
| **0x09**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |      |      |      |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |  Y   |  Y   |  Y   |      |      |      |      |
| **0x0A**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x0B**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |
| **0x0C**  |      |      |      |  Y   |      |      |  Y   |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |  Y   |      |      |
| **0x0D**  |      |      |      |  Y   |  Y   |      |  Y   |  Y   |      |      |      |      |      |  Y   |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |  Y   |      |
| **0x0E**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |  Y   |  Y   |  Y   |  Y   |      |      |      |      |  Y   |      |  Y   |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x0F**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |  Y   |  Y   |      |  Y   |      |      |  Y   |      |  Y   |      |      |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x10**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |  Y   |      |      |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |  Y   |      |  Y   |      |      |      |      |      |      |
| **0x11**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x12**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x13**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x14**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |  Y   |      |      |  Y   |      |      |      |      |      |      |      |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x15**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |  Y   |      |  Y   |  Y   |  Y   |  Y   |      |      |      |      |      |      |  Y   |      |  Y   |  Y   |      |      |  Y   |  Y   |      |
| **0x16**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x17**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |  Y   |      |  Y   |      |      |      |      |  Y   |      |      |      |      |      |      |      |      |      |      |
| **0x18**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x19**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |  Y   |      |      |      |      |  Y   |  Y   |  Y   |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |      |      |  Y   |      |      |
| **0x1A**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |      |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |  Y   |      |      |      |  Y   |      |  Y   |
| **0x1B**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |
| **0x1C**  |      |      |      |  Y   |      |      |  Y   |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |  Y   |      |      |
| **0x1D**  |  Y   |      |      |  Y   |  Y   |      |  Y   |  Y   |      |      |      |      |  Y   |      |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |  Y   |  Y   |      |  Y   |      |      |      |
| **0x1E**  |  Y   |      |      |  Y   |      |      |  Y   |  Y   |  Y   |      |      |      |      |  Y   |  Y   |  Y   |      |      |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |
| **0x1F**  |      |      |      |  Y   |      |      |  Y   |  Y   |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |      |  Y   |      |      |      |      |      |

# Sorting Layer

| ID           | Name                  | Value |
|:-------------|:----------------------|:-----:|
| `0x08D7B4B9` | `SkyDecor`            | `-12` |
| `0xC22B02A3` | `BackgroundDecor`     | `-11` |
| `0x88F263AD` | `MiddlegroundDecor`   | `-10` |
| `0x220BEC37` | `ExteriorDecor`       | `-9`  |
| `0xA3EA8441` | `ExteriorTile`        | `-8`  |
| `0x09EEEB53` | `ExteriorDetailsTile` | `-7`  |
| `0x03B836D7` | `WallBackDecor`       | `-6`  |
| `0xFDB611FD` | `Rain`                | `-5`  |
| `0x387DDD9F` | `WallTile`            | `-4`  |
| `0xC81B7ABF` | `WallFrontDecor`      | `-3`  |
| `0xE5855F21` | `WallFrontTile`       | `-2`  |
| `0x9B92996F` | `GameplayDecor`       | `-1`  |
| `0x00000000` | `Default`             |  `0`  |
| `0x0F73C973` | `Elevator`            |  `1`  |
| `0xC82E234F` | `Door`                |  `2`  |
| `0x5374895F` | `Prop`                |  `3`  |
| `0x270E1B19` | `Blood`               |  `4`  |
| `0x4F949767` | `Corpse`              |  `5`  |
| `0x2322C27B` | `Static Zombie`       |  `6`  |
| `0x6813A8E1` | `Human`               |  `7`  |
| `0x45A8730B` | `Bonus`               |  `8`  |
| `0x11A0103F` | `Zombie`              |  `9`  |
| `0xEE2FF0B5` | `Zombie Overlord`     | `10`  |
| `0xFCC6DE09` | `Trap`                | `11`  |
| `0x160BBE75` | `StairsTile`          | `12`  |
| `0xBE07F37F` | `Water`               | `13`  |
| `0x311FDB2F` | `GameplayTile`        | `14`  |
| `0x2095F59D` | `BloodUpTile`         | `15`  |
| `0x9D109DA9` | `BloodDownTile`       | `16`  |
| `0x8EA23237` | `BloodLeftLeftTile`   | `17`  |
| `0x8297AB09` | `BloodDownRightTile`  | `18`  |
| `0x3604A1DD` | `ForegroundDecor`     | `19`  |
| `0x624661BB` | `GameplayFrontTile`   | `20`  |
| `0x94841565` | `Foreground Parallax` | `21`  |
| `0xBFD74D83` | `FrontEffect`         | `22`  |
| `0xF16F8A89` | `Mask`                | `23`  |
| `0x94DD6639` | `LevelEditor`         | `24`  |
| `0x4629EB59` | `UI`                  | `25`  |