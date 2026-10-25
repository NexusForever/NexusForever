namespace NexusForever.Game.Static.Spell.Effect
{
    /// <summary>
    /// Arithmetic a <see cref="SpellEffectType.ModifySpellCooldown"/> effect applies to a cooldown.
    /// </summary>
    public enum SpellEffectModifyCooldownMode
    {
        /// <summary>
        /// Multiply by the magnitude, clamped so the cooldown can only become shorter.
        /// </summary>
        MultiplyClamped = 0,

        /// <summary>
        /// Multiply by the magnitude, unclamped.
        /// </summary>
        Multiply        = 1,

        /// <summary>
        /// Add the magnitude in milliseconds, floored at zero.
        /// </summary>
        AddMilliseconds = 2,

        /// <summary>
        /// Replace the cooldown with the magnitude in milliseconds.
        /// </summary>
        SetMilliseconds = 3
    }
}
