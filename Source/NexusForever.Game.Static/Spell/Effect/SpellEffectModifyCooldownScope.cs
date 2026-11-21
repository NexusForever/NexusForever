namespace NexusForever.Game.Static.Spell.Effect
{
    /// <summary>
    /// Key space a <see cref="SpellEffectType.ModifySpellCooldown"/> effect matches cooldowns with.
    /// </summary>
    public enum SpellEffectModifyCooldownScope
    {
        BaseSpell       = 0,
        Spell4          = 1,
        SpellGroupId    = 2,
        SpellCooldownId = 3
    }
}
