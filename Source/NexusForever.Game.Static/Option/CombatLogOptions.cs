namespace NexusForever.Game.Static.Option
{
    [Flags]
    public enum CombatLogOptions
    {
        DisableAbsorption     = 0x0001,
        DisableCCState        = 0x0002,
        DisableDamage         = 0x0004,
        DisableDeflect        = 0x0008,
        DisableDelayDeath     = 0x0010,
        DisableDispel         = 0x0020,
        DisableFallingDamage  = 0x0040,
        DisableHeal           = 0x0080,
        DisableImmunity       = 0x0100,
        DisableInterrupted    = 0x0200,
        DisableInterruptArmor = 0x0400,
        DisableTransference   = 0x0800,
        DisableVitalModifier  = 0x1000,
        DisableDeath          = 0x2000,
    };
}
