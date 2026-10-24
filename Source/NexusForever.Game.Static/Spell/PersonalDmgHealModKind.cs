namespace NexusForever.Game.Static.Spell
{
    /// <summary>
    /// Kind of modifier applied by a <see cref="SpellEffectType.PersonalDmgHealMod"/> effect, taken from
    /// its DataBits00 column.
    /// </summary>
    /// <remarks>
    /// This is NOT a <see cref="NexusForever.Game.Static.Entity.Property"/> id. The client's own row
    /// validator for effect type 151 is <see cref="SpellEffectType.UnitPropertyModifier"/>'s validator
    /// case with the UnitProperty2 lookup removed, so the client deliberately never resolves this column
    /// against a table.
    ///
    /// The shipped data uses nine values - 3, 4, 5, 6, 7, 8, 9, 12 and 13 - over 145 rows. Only 12 is
    /// named here, because only 12 has corroborating evidence: three of the shipped tooltips that
    /// reference one of its own effect rows describe it as healing the caster performs on the affected
    /// unit ("healing received from the caster", "your healing on allies affected by ..."). The other
    /// eight values are deliberately left unnamed rather than guessed; a handler that meets one logs it
    /// and applies nothing.
    ///
    /// Note the scope: the modifier is personal, i.e. it applies to the pair (caster, affected unit),
    /// not to the affected unit globally. A whole-entity healing multiplier is a different mechanic and
    /// the data expresses it with <see cref="SpellEffectType.UnitPropertyModifier"/> and the properties
    /// HealingMultiplierOutgoing (162, 128 shipped rows) and HealingMultiplierIncoming (163, 206 rows).
    /// </remarks>
    public enum PersonalDmgHealModKind : uint
    {
        /// <summary>
        /// Scales healing performed by the caster of the effect on the affected unit.
        /// </summary>
        HealingFromCaster = 12
    }
}
