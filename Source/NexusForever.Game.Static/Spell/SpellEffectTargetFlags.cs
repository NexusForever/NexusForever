namespace NexusForever.Game.Static.Spell
{
    [Flags]
    public enum SpellEffectTargetFlags
    {
        None      = 0x00,
        Caster    = 0x01,
        Target    = 0x02,
        Telegraph = 0x04,
        Unknown08 = 0x08
    }
}
