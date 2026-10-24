namespace NexusForever.Game.Abstract.Spell.Effect.Data
{
    public interface ISpellEffectPersonalDmgHealModData : ISpellEffectData
    {
        /// <summary>
        /// Which modifier this row carries; see PersonalDmgHealModKind. Not a Property id.
        /// </summary>
        uint Kind { get; }

        /// <summary>
        /// Mode of the modifier, mandatory and non-zero. Its meaning is unknown; the shipped data uses
        /// 1 (108 rows) and 2 (37 rows), and every kind 12 row uses 1.
        /// </summary>
        uint Mode { get; }

        /// <summary>
        /// Magnitude of the modifier, bit-cast from the DataBits02 column.
        /// </summary>
        float Magnitude { get; }
    }
}
