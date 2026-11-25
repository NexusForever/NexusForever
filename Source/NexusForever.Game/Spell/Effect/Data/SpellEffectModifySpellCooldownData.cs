using NexusForever.Game.Abstract.Spell.Effect.Data;
using NexusForever.Game.Static.Spell.Effect;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Spell.Effect.Data
{
    public class SpellEffectModifySpellCooldownData : ISpellEffectModifySpellCooldownData
    {
        public Spell4EffectsEntry Entry { get; private set; }
        public SpellEffectModifyCooldownScope Scope { get; private set; }
        public uint Key { get; private set; }
        public SpellEffectModifyCooldownMode Mode { get; private set; }
        public float Magnitude { get; private set; }

        public void Populate(Spell4EffectsEntry entry)
        {
            Entry     = entry;
            Scope     = (SpellEffectModifyCooldownScope)entry.DataBits00;
            Key       = entry.DataBits01;
            Mode      = (SpellEffectModifyCooldownMode)entry.DataBits02;
            Magnitude = BitConverter.UInt32BitsToSingle(entry.DataBits03);
        }
    }
}
