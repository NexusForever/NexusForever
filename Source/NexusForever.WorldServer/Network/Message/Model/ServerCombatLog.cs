using System;
using System.Collections.Generic;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Combat.Static;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCombatLog)]
    public class ServerCombatLog : IWritable
    {
        public CombatLogType Type { get; set; } // 6u

        public virtual void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 6u);
        }
    }
    
    public class CombatLogCastData : IWritable
    {
        public uint CasterId { get; set; }
        public uint TargetId { get; set; }
        public uint SpellId { get; set; } // 18u
        public CombatResult CombatResult { get; set; } // 4u

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CasterId);
            writer.Write(TargetId);
            writer.Write(SpellId, 18u);
            writer.Write(CombatResult, 4u);
        }
    }

    public class CombatLogAbsorption : ServerCombatLog, IWritable
    {
        public uint AbsorptionAmount { get; set; }
        public CombatLogCastData CastData { get; set; }

        public CombatLogAbsorption()
        {
            Type = CombatLogType.Absorption;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);
        }
    }

    public class CombatLogCCState : ServerCombatLog, IWritable
    {
        public byte State { get; set; } // 5u
        public bool BRemoved { get; set; }
        public uint InterruptArmorTaken { get; set; }
        public byte Result { get; set; } // 4u
        public ushort Unknown0 { get; set; } // 14u
        public CombatLogCastData CastData { get; set; }

        public CombatLogCCState()
        {
            Type = CombatLogType.CCState;
        }

        public override void Write(GamePacketWriter writer)
        {
            Type = CombatLogType.CCState;

            base.Write(writer);

            writer.Write(State, 5u);
            writer.Write(BRemoved);
            writer.Write(InterruptArmorTaken);
            writer.Write(Result, 4u);
            writer.Write(Unknown0, 14u);
            CastData.Write(writer);
        }
    }

    public class CombatLogCCStateBreak : ServerCombatLog, IWritable
    {
        public uint CasterId { get; set; }
        public byte State { get; set; } // 5u

        public CombatLogCCStateBreak()
        {
            Type = CombatLogType.CCStateBreak;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(CasterId);
            writer.Write(State);
        }
    }

    public class CombatLogDamage : ServerCombatLog, IWritable
    {
        public uint MitigatedDamage { get; set; }
        public uint RawDamage { get; set; }
        public uint Shield { get; set; }
        public uint Absorption { get; set; }
        public uint Overkill { get; set; }
        public uint Glance { get; set; }
        public bool BTargetVulnerable { get; set; }
        public bool BKilled { get; set; }
        public bool BPeriodic { get; set; }
        public DamageType DamageType { get; set; } // 3u
        public SpellEffectType EffectType { get; set; } // 8u
        public CombatLogCastData CastData { get; set; }

        public CombatLogDamage()
        {
            Type = CombatLogType.Damage;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(MitigatedDamage);
            writer.Write(RawDamage);
            writer.Write(Shield);
            writer.Write(Absorption);
            writer.Write(Overkill);
            writer.Write(Glance);
            writer.Write(BTargetVulnerable);
            writer.Write(BKilled);
            writer.Write(BPeriodic);
            writer.Write(DamageType, 3u);
            writer.Write(EffectType, 8u);
            CastData.Write(writer);
        }
    }

    public class CombatLogDamageShield : CombatLogDamage
    {
        public CombatLogDamageShield()
        {
            Type = CombatLogType.DamageShields;
        }
    }

    public class CombatLogReflect : ServerCombatLog, IWritable
    {
        public uint DamageAmount { get; set; }
        public uint RawDamage { get; set; }
        public uint Shield { get; set; }
        public uint Overkill { get; set; }
        public uint Glance { get; set; }
        public uint Absorption { get; set; }
        public bool Unknown0 { get; set; }
        public bool BKilled { get; set; }
        public DamageType DamageType { get; set; } // 3u
        public SpellEffectType EffectType { get; set; } // 8u
        public CombatLogCastData CastData { get; set; }

        public CombatLogReflect()
        {
            Type = CombatLogType.Reflect;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(DamageAmount);
            writer.Write(RawDamage);
            writer.Write(Shield);
            writer.Write(Overkill);
            writer.Write(Glance);
            writer.Write(Absorption);
            writer.Write(Unknown0);
            writer.Write(BKilled);
            writer.Write(DamageType, 3u);
            writer.Write(EffectType, 8u);
            CastData.Write(writer);
        }
    }

    public class CombatLogMultiHit : ServerCombatLog, IWritable
    {
        public uint MitigatedDamage { get; set; }
        public uint RawDamage { get; set; }
        public uint Shield { get; set; }
        public uint Absorption { get; set; }
        public uint Overkill { get; set; }
        public uint Unknown0 { get; set; }
        public bool BTargetVulnerable { get; set; }
        public bool BKilled { get; set; }
        public bool BPeriodic { get; set; }
        public DamageType DamageType { get; set; } // 3u
        public SpellEffectType EffectType { get; set; } // 8u
        public CombatLogCastData CastData { get; set; }

        public CombatLogMultiHit()
        {
            Type = CombatLogType.MultiHit;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(MitigatedDamage);
            writer.Write(RawDamage);
            writer.Write(Shield);
            writer.Write(Absorption);
            writer.Write(Overkill);
            writer.Write(Unknown0);
            writer.Write(BTargetVulnerable);
            writer.Write(BKilled);
            writer.Write(BPeriodic);
            writer.Write(DamageType, 3u);
            writer.Write(EffectType, 8u);
            CastData.Write(writer);
        }
    }

    public class CombatLogMultiHitShields : ServerCombatLog, IWritable
    {
        public uint MitigatedDamage { get; set; }
        public uint RawDamage { get; set; }
        public uint Shield { get; set; }
        public uint Absorption { get; set; }
        public uint Overkill { get; set; }
        public uint Unknown0 { get; set; }
        public bool BTargetVulnerable { get; set; }
        public bool BKilled { get; set; }
        public bool BPeriodic { get; set; }
        public DamageType DamageType { get; set; } // 3u
        public SpellEffectType EffectType { get; set; } // 8u
        public CombatLogCastData CastData { get; set; }

        public CombatLogMultiHitShields()
        {
            Type = CombatLogType.MultiHitShields;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(MitigatedDamage);
            writer.Write(RawDamage);
            writer.Write(Shield);
            writer.Write(Absorption);
            writer.Write(Overkill);
            writer.Write(Unknown0);
            writer.Write(BTargetVulnerable);
            writer.Write(BKilled);
            writer.Write(BPeriodic);
            writer.Write(DamageType, 3u);
            writer.Write(EffectType, 8u);
            CastData.Write(writer);
        }
    }

    public class CombatLogFallingDamage : ServerCombatLog, IWritable
    {
        public uint CasterId { get; set; }
        public uint Damage { get; set; }

        public override void Write(GamePacketWriter writer)
        {
            Type = CombatLogType.FallingDamage;
            base.Write(writer);

            writer.Write(CasterId);
            writer.Write(Damage);
        }
    }

    public class CombatLogLifesteal : ServerCombatLog, IWritable
    {
        public uint Unknown0 { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }

        public CombatLogLifesteal()
        {
            Type = CombatLogType.Lifesteal;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(Unknown0);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
        }
    }

    public class CombatLogDelayDeath : ServerCombatLog, IWritable
    {
        public CombatLogCastData CastData { get; set; }

        public CombatLogDelayDeath()
        {
            Type = CombatLogType.DelayDeath;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);
            CastData.Write(writer);
        }
    }

    public class CombatLogDispel : ServerCombatLog, IWritable
    {
        public bool BRemovesSingleInstance { get; set; }
        public uint InstancesRemoved { get; set; }
        public uint SpellRemovedId { get; set; } // 18u

        public CombatLogDispel()
        {
            Type = (CombatLogType)11;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(BRemovesSingleInstance);
            writer.Write(InstancesRemoved);
            writer.Write(SpellRemovedId, 18u);
        }
    }

    public class CombatLogHeal : ServerCombatLog, IWritable
    {
        public uint HealAmount { get; set; }
        public uint Overheal { get; set; }
        public uint Unknown0 { get; set; }
        public SpellEffectType EffectType { get; set; } // 8u
        public CombatLogCastData CastData { get; set; }

        public CombatLogHeal()
        {
            Type = CombatLogType.Heal;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(HealAmount);
            writer.Write(Overheal);
            writer.Write(Unknown0);
            writer.Write(EffectType, 8u);
            CastData.Write(writer);
        }
    }

    public class CombatLogMultiHeal : ServerCombatLog, IWritable
    {
        public uint HealAmount { get; set; }
        public uint Overheal { get; set; }
        public uint Absorption { get; set; }
        public SpellEffectType EffectType { get; set; } // 8u
        public CombatLogCastData CastData { get; set; }

        public CombatLogMultiHeal()
        {
            Type = CombatLogType.MultiHeal;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(HealAmount);
            writer.Write(Overheal);
            writer.Write(Absorption);
            writer.Write(EffectType, 8u);
            CastData.Write(writer);
        }
    }

    public class CombatLogModifyInterruptArmor : ServerCombatLog, IWritable
    {
        public uint Amount { get; set; }
        public CombatLogCastData CastData { get; set; }

        public CombatLogModifyInterruptArmor()
        {
            Type = CombatLogType.ModifyInterruptArmor;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);
            writer.Write(Amount);
            CastData.Write(writer);
        }
    }

    public class CombatLogTransference : ServerCombatLog, IWritable
    {
        public uint DamageAmount { get; set; }
        public DamageType DamageType { get; set; } // 3u
        public uint Shield { get; set; }
        public uint Absorption { get; set; }
        public uint Overkill { get; set; }
        public uint GlanceAmount { get; set; }
        public bool BTargetVulnerable { get; set; }
        public List<CombatLogCastData> CastDatas { get; set; } = new List<CombatLogCastData>();

        public CombatLogTransference()
        {
            Type = CombatLogType.Transference;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(DamageAmount);
            writer.Write(DamageType, 3u);
            writer.Write(Shield);
            writer.Write(Absorption);
            writer.Write(Overkill);
            writer.Write(GlanceAmount);
            writer.Write(BTargetVulnerable);
            writer.Write(CastDatas.Count);

            foreach (CombatLogCastData castData in CastDatas)
                castData.Write(writer);
        }
    }

    public class CombatLogVitalModifier : ServerCombatLog, IWritable
    {
        public float Amount { get; set; }
        public uint VitalModified { get; set; } // 5u - TODO: Replace with Vital enum
        public bool BShowCombatLog { get; set; }
        public CombatLogCastData CastData { get; set; }

        public CombatLogVitalModifier()
        {
            Type = CombatLogType.VitalModifier;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(Amount);
            writer.Write(VitalModified, 5u);
            writer.Write(BShowCombatLog);
            CastData.Write(writer);
        }
    }

    public class CombatLogDeflect : ServerCombatLog, IWritable
    {
        public bool BMultiHit { get; set; }
        public CombatLogCastData CastData { get; set; }

        public CombatLogDeflect()
        {
            Type = CombatLogType.Deflect;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(BMultiHit);
            CastData.Write(writer);
        }
    }

    public class CombatLogImmune : ServerCombatLog, IWritable
    {
        public CombatLogCastData CastData { get; set; }

        public CombatLogImmune()
        {
            Type = CombatLogType.Immune;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            CastData.Write(writer);
        }
    }

    public class CombatLogInterrupted : ServerCombatLog, IWritable
    {
        public uint InterruptingSpellId { get; set; } // 18u
        public ushort Reason { get; set; } // 9u
        public CombatLogCastData CastData { get; set; }

        public CombatLogInterrupted()
        {
            Type = CombatLogType.Interrupted;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(InterruptingSpellId, 18u);
            writer.Write(Reason, 9u);
            CastData.Write(writer);
        }
    }

    public class CombatLogKillStreak : ServerCombatLog, IWritable
    {
        public uint UnitId { get; set; }
        public byte StatType { get; set; } // 5u
        public uint StreakAmount { get; set; }

        public CombatLogKillStreak()
        {
            Type = CombatLogType.KillStreak;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(UnitId);
            writer.Write(StatType, 5u);
            writer.Write(StreakAmount);
        }
    }

    public class CombatLogKillPvP : ServerCombatLog, IWritable
    {
        public CombatLogCastData CastData { get; set; }

        public CombatLogKillPvP()
        {
            Type = CombatLogType.KillPvP;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            CastData.Write(writer);
        }
    }

    public class CombatLogDeath : ServerCombatLog, IWritable
    {
        public uint UnitId { get; set; }

        public CombatLogDeath()
        {
            Type = CombatLogType.Death;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(UnitId);
        }
    }

    public class CombatLogResurrect : ServerCombatLog, IWritable
    {
        public uint UnitId { get; set; }

        public CombatLogResurrect()
        {
            Type = CombatLogType.Resurrect;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(UnitId);
        }
    }

    public class CombatLogStealth : ServerCombatLog, IWritable
    {
        public uint UnitId { get; set; }
        public bool BExiting { get; set; }

        public CombatLogStealth()
        {
            Type = CombatLogType.Stealth;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(UnitId);
            writer.Write(BExiting);
        }
    }

    public class CombatLogMount : ServerCombatLog, IWritable
    {
        public bool BDismounted { get; set; }
        public CombatLogCastData CastData { get; set; }

        public CombatLogMount()
        {
            Type = CombatLogType.Mount;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(BDismounted);
            CastData.Write(writer);
        }
    }

    public class CombatLogPet : ServerCombatLog, IWritable
    {
        public bool BDismissed { get; set; }
        public bool BKilled { get; set; }
        public CombatLogCastData CastData { get; set; }

        public CombatLogPet()
        {
            Type = CombatLogType.Pet;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(BDismissed);
            writer.Write(BKilled);
            CastData.Write(writer);
        }
    }

    public class CombatLogExperience : ServerCombatLog, IWritable
    {
        public uint UnitId { get; set; }
        public uint Xp { get; set; }
        public uint RestXp { get; set; }
        public uint ElderPoints { get; set; }
        public uint RestElderPoints { get; set; }

        public CombatLogExperience()
        {
            Type = CombatLogType.Experience;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(UnitId);
            writer.Write(Xp);
            writer.Write(RestXp);
            writer.Write(ElderPoints);
            writer.Write(RestElderPoints);
        }
    }

    public class CombatLogModifying : ServerCombatLog, IWritable
    {
        public uint CasterId { get; set; }
        public uint HostItemId { get; set; } // 18u
        public uint SocketedItemId { get; set; }
        public uint UnsocketedItemId { get; set; }

        public CombatLogModifying()
        {
            Type = CombatLogType.Modifying;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(CasterId);
            writer.Write(HostItemId, 18u);
            writer.Write(SocketedItemId);
            writer.Write(UnsocketedItemId);
        }
    }

    public class CombatLogDurabilityLoss : ServerCombatLog, IWritable
    {
        public uint UnitId { get; set; }
        public float Amount { get; set; }

        public CombatLogDurabilityLoss()
        {
            Type = CombatLogType.DurabilityLoss;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(UnitId);
            writer.Write(Amount);
        }
    }

    public class CombatLogCrafting : ServerCombatLog, IWritable
    {
        public uint Unknown0 { get; set; }
        public uint Unknown1 { get; set; } // 18u

        public CombatLogCrafting()
        {
            Type = CombatLogType.Crafting;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(Unknown0);
            writer.Write(Unknown1, 18u);
        }
    }

    public class CombatLogLAS : ServerCombatLog, IWritable
    {
        public uint Unknown0 { get; set; }

        public CombatLogLAS()
        {
            Type = CombatLogType.LAS;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(Unknown0);
        }
    }

    public class CombatLogBuildSwitch : ServerCombatLog, IWritable
    {
        public uint UnitId { get; set; }
        public byte NewSpecIndex { get; set; } // 3u

        public CombatLogBuildSwitch()
        {
            Type = CombatLogType.BuildSwitch;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(UnitId);
            writer.Write(NewSpecIndex, 3u);
        }
    }

    public class CombatLogDatacube : ServerCombatLog, IWritable
    {
        public uint UnitId { get; set; }
        public byte DatacubeType { get; set; } // 3u
        public bool HasPieces { get; set; }

        public CombatLogDatacube()
        {
            Type = CombatLogType.Datacube;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(UnitId);
            writer.Write(DatacubeType, 3u);
            writer.Write(HasPieces);
        }
    }

    public class CombatLogHealingAbsorption : ServerCombatLog, IWritable
    {
        public uint Amount { get; set; }

        public CombatLogHealingAbsorption()
        {
            Type = CombatLogType.HealingAbsorption;
        }

        public override void Write(GamePacketWriter writer)
        {
            base.Write(writer);

            writer.Write(Amount);
        }
    }
}