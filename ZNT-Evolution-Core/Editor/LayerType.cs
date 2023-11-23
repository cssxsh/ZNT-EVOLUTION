// ReSharper disable InconsistentNaming

namespace ZNT.Evolution.Core.Editor
{
    public enum LayerType
    {
        Default = 0x00,
        TransparentFX = 0x01,
        Ignore_Raycast = 0x02,
        _3 = 0x03,
        Water = 0x04,
        UI = 0x05,
        _6 = 0x06,
        _7 = 0x07,
        Zombie = 0x08,
        Human = 0x09,
        Ignore_Collisions = 0x0A,
        Ignore_Characters = 0x0B,
        Zombie_Stopper = 0x0C,
        Renderer = 0x0D,
        Crate = 0x0E,
        Gameplay = 0x0F,
        Prop = 0x10,
        Foreground = 0x11,
        One_Way = 0x12,
        Exterior = 0x13,
        Stairs = 0x14,
        Stairs_Top = 0x15,
        Middleground = 0x16,
        Background = 0x17,
        Sky = 0x18,
        Projectile = 0x19,
        Spit = 0x1A,
        Block_Humans = 0x1B,
        Block_Zombies = 0x1C,
        World_Enemy = 0x1D,
        Moving_Trap = 0x1E,
        Block_Explosion = 0x1F
    }
}