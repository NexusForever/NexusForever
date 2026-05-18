namespace NexusForever.Game.Static.Entity
{
    /// <summary>
    /// Used by Database Unit Property Defaults and <see cref="SpellEffectType.UnitPropertyModifier"/> effect (DataBits01)
    /// </summary>
    public enum ModType
    {
        /// <summary>
        /// Used in Database for Class and Player Base Properties.
        /// </summary>
        BaseAmount = 0,

        /// <summary>
        /// Modifier is multiplicative of current value.
        /// </summary>
        Percentage = 1,

        /// <summary>
        /// Modifier is a flat value to adjust the current value by.
        /// </summary>
        FlatValue = 2,

        /// <summary>
        /// Modifier scales based on Effective Level.
        /// </summary>
        LevelScale = 3
    }
}
