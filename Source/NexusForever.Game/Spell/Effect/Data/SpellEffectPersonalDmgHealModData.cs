using NexusForever.Game.Abstract.Spell.Effect.Data;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Spell.Effect.Data
{
    public class SpellEffectPersonalDmgHealModData : ISpellEffectPersonalDmgHealModData
    {
        public Spell4EffectsEntry Entry { get; private set; }
        public uint Kind { get; private set; }
        public uint Mode { get; private set; }
        public float Magnitude { get; private set; }

        public void Populate(Spell4EffectsEntry entry)
        {
            Entry     = entry;
            Kind      = entry.DataBits00;
            Mode      = entry.DataBits01;
            // DataBits02 is a float stored in a uint column (SpellEffectType.dataType02 is 0 for this
            // effect type, and every one of the 145 shipped values is a float bit pattern). Read as an
            // integer, the commonest value 1.1f would be 1,066,192,077.
            Magnitude = BitConverter.UInt32BitsToSingle(entry.DataBits02);
        }
    }
}
