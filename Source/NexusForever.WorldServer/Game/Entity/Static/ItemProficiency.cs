using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Allows setting a <see cref="Player"/>'s <see cref="ItemProficiencies"/> flags to support class <see cref="Item"/>
    /// </summary>
    [Flags]
    public enum ItemProficiency
    {
        BattleArmor     = 1,
        HeavyArmor      = 2,
        MediumArmor     = 4,
        LightArmor      = 8,
        GreatWeapon     = 16,
        Shotgun         = 32,
        HeavyGun        = 64,
        Wrench          = 128,
        Resonators      = 256,
        BatteryPack     = 512,
        BerserkerWeapon = 1024,
        ShadowCrystal   = 2048,
        Pistols         = 4096,
        Sword           = 8192,
        Mace            = 16384,
        Instrument      = 32768,
        Stave           = 65536,
        SpellTome       = 131072,
        Psyblade        = 262144,
        MindGem         = 524288,
        Claws           = 1048576,
        Relic           = 2097152,
        Talisman        = 0
    }
}
