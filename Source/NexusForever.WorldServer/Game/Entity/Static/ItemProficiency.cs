using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Allows setting a <see cref="Player"/>'s <see cref="ItemProficiencies"/> flags to support class <see cref="Item"/>
    /// </summary>
    [Flags]
    public enum ItemProficiency
    {
        Talisman        = 0x000000,
        BattleArmor     = 0x000001,
        HeavyArmor      = 0x000002,
        MediumArmor     = 0x000004,
        LightArmor      = 0x000008,
        GreatWeapon     = 0x000010,
        Shotgun         = 0x000020,
        HeavyGun        = 0x000040,
        Wrench          = 0x000080,
        Resonators      = 0x000100,
        BatteryPack     = 0x000200,
        BerserkerWeapon = 0x000400,
        ShadowCrystal   = 0x000800,
        Pistols         = 0x001000,
        Sword           = 0x002000,
        Mace            = 0x004000,
        Instrument      = 0x008000,
        Stave           = 0x010000,
        SpellTome       = 0x020000,
        Psyblade        = 0x040000,
        MindGem         = 0x080000,
        Claws           = 0x100000,
        Relic           = 0x200000
    }
}
