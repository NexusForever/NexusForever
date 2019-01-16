using System.Collections.Generic;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Spell.Static;

namespace NexusForever.WorldServer.Game.Spell
{
    public class SpellTargetInfo
    {
        public class SpellTargetEffectInfo
        {
            public class DamageDescription
            {
                public DamageType DamageType { get; set; }
                public uint RawDamage { get; set; }
                public uint RawScaledDamage { get; set; }
                public uint AbsorbedAmount { get; set; }
                public uint ShieldAbsorbAmount { get; set; }
                public uint AdjustedDamage { get; set; }
                public uint OverkillAmount { get; set; }
                public bool KilledTarget { get; set; }
            }

            public uint EffectId { get; }
            public Spell4EffectsEntry Entry { get; }
            public DamageDescription Damage { get; private set; }

            public SpellTargetEffectInfo(uint effectId, Spell4EffectsEntry entry)
            {
                EffectId = effectId;
                Entry    = entry;
            }

            public void AddDamage(DamageType damageType, uint damage)
            {
                // TODO: handle this correctly
                Damage = new DamageDescription
                {
                    DamageType      = damageType,
                    RawDamage       = damage,
                    RawScaledDamage = damage,
                    AdjustedDamage  = damage
                };
            }
        }

        public SpellEffectTargetFlags Flags { get; }
        public UnitEntity Entity { get; }
        public List<SpellTargetEffectInfo> Effects { get; } = new List<SpellTargetEffectInfo>();

        public SpellTargetInfo(SpellEffectTargetFlags flags, UnitEntity entity)
        {
            Flags  = flags;
            Entity = entity;
        }
    }
}
