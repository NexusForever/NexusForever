using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Spell
{
    public class SpellTargetInfo : ISpellTargetInfo
    {
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
                public CombatResult CombatResult { get; set; }
            }

            public uint EffectId { get; }
            public bool DropEffect { get; set; } = false;
            public Spell4EffectsEntry Entry { get; }
            public IDamageDescription Damage { get; private set; }
            public List<ServerCombatLog> CombatLogs { get; private set; } = new List<ServerCombatLog>();

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

            public void AddDamage(IDamageDescription damage)
            {
                Damage = damage;
            }

            public void AddCombatLog(ServerCombatLog combatLog)
            {
                CombatLogs.Add(combatLog);
            }
        }

        public SpellEffectTargetFlags Flags { get; }
        public IUnitEntity Entity { get; }
        public List<ISpellTargetEffectInfo> Effects { get; } = new List<ISpellTargetEffectInfo>();

        public SpellTargetInfo(SpellEffectTargetFlags flags, IUnitEntity entity)
        {
            Flags  = flags;
            Entity = entity;
        }
    }
}
