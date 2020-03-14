using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Spell.Static;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class TargetInfo : IWritable // same used for 0x0818
    {
        public class EffectInfo : IWritable
        {
            public class DamageDescription : IWritable // same used for 0x07F6
            {
                public class UnknownStructure3 : IWritable
                {
                    public uint Unknown0 { get; set; }
                    public uint Unknown1 { get; set; }
                    public uint Unknown2 { get; set; }
                    public uint Unknown3 { get; set; }
                    public uint Unknown4 { get; set; }
                    public uint Unknown5 { get; set; }
                    public uint Unknown6 { get; set; }
                    public byte Unknown7 { get; set; }

                    public void Write(GamePacketWriter writer)
                    {
                        writer.Write(Unknown0);
                        writer.Write(Unknown1);
                        writer.Write(Unknown2);
                        writer.Write(Unknown3);
                        writer.Write(Unknown4);
                        writer.Write(Unknown5);
                        writer.Write(Unknown6);
                        writer.Write(Unknown7, 3u);
                    }
                }

                public uint RawDamage { get; set; }
                public uint RawScaledDamage { get; set; }
                public uint AbsorbedAmount { get; set; }
                public uint ShieldAbsorbAmount { get; set; }
                public uint AdjustedDamage { get; set; }
                public uint OverkillAmount { get; set; }
                public uint Unknown6 { get; set; }
                public bool KilledTarget { get; set; }
                public CombatResult CombatResult { get; set; }
                public DamageType DamageType { get; set; }

                public List<UnknownStructure3> unknownStructure3 { get; set; } = new List<UnknownStructure3>();

                public void Write(GamePacketWriter writer)
                {
                    writer.Write(RawDamage);
                    writer.Write(RawScaledDamage);
                    writer.Write(AbsorbedAmount);
                    writer.Write(ShieldAbsorbAmount);
                    writer.Write(AdjustedDamage);
                    writer.Write(OverkillAmount);
                    writer.Write(Unknown6);
                    writer.Write(KilledTarget);
                    writer.Write(CombatResult, 4u);
                    writer.Write(DamageType, 3u);

                    writer.Write(unknownStructure3.Count, 8u);
                    unknownStructure3.ForEach(u => u.Write(writer));
                }
            }

            public uint Spell4EffectId { get; set; }
            public uint EffectUniqueId { get; set; }
            public uint DelayTime { get; set; }
            public int TimeRemaining { get; set; }
            public byte InfoType { get; set; }

            public DamageDescription DamageDescriptionData { get; set; } = new DamageDescription();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Spell4EffectId, 19u);
                writer.Write(EffectUniqueId);
                writer.Write(DelayTime);
                writer.Write(TimeRemaining);
                writer.Write(InfoType, 2u);

                if (InfoType == 1)
                    DamageDescriptionData.Write(writer);
                else
                    writer.Write(0u, 1u);
            }
        }

        public uint UnitId { get; set; }
        public byte Ndx { get; set; }
        public byte TargetFlags { get; set; }
        public ushort InstanceCount { get; set; }
        public CombatResult CombatResult { get; set; }

        public List<EffectInfo> EffectInfoData { get; set; } = new List<EffectInfo>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Ndx);
            writer.Write(TargetFlags);
            writer.Write(InstanceCount);
            writer.Write(CombatResult, 4u);

            writer.Write(EffectInfoData.Count, 8u);
            EffectInfoData.ForEach(u => u.Write(writer));
        }
    }
}
