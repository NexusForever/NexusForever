using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using System.Diagnostics.CodeAnalysis;

namespace NexusForever.Game.Spell
{
    public class SpellTargetInfo : ISpellTargetInfo
    {
        public class SpellTargetInfoComparer : IEqualityComparer<ISpellTargetInfo>
        {
            public bool Equals(ISpellTargetInfo target1, ISpellTargetInfo target2)
            {
                return target1.Entity.Guid == target2.Entity.Guid;
            }

            public int GetHashCode([DisallowNull] ISpellTargetInfo obj)
            {
                return obj.Entity.Guid.GetHashCode();
            }
        }

        public class SpellTargetEffectInfo : ISpellTargetEffectInfo
        {
            public class DamageDescription : IDamageDescription
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
            public IDamageDescription Damage { get; private set; }

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

        public SpellEffectTargetFlags Flags { get; set; }
        public IUnitEntity Entity { get; }
        public float Distance { get; }
        public List<ISpellTargetEffectInfo> Effects { get; } = new List<ISpellTargetEffectInfo>();
        public TargetSelectionState TargetSelectionState { get; set; } = TargetSelectionState.New;

        public SpellTargetInfo(SpellEffectTargetFlags flags, IUnitEntity entity)
        {
            Flags  = flags;
            Entity = entity;
        }

        public SpellTargetInfo(SpellEffectTargetFlags flags, IUnitEntity entity, float distance)
        {
            Flags    = flags;
            Entity   = entity;
            Distance = distance;
        }
    }
}
