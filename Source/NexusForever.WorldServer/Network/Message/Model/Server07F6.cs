using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07F6, MessageDirection.Server)]
    public class Server07F6 : IWritable
    {
        public class UnknownStructure3 : IWritable
        {
            public uint Unknown0 { get; set; } = 0;
            public uint Unknown1 { get; set; } = 0;
            public uint Unknown2 { get; set; } = 0;
            public uint Unknown3 { get; set; } = 0;
            public uint Unknown4 { get; set; } = 0;
            public uint Unknown5 { get; set; } = 0;
            public uint Unknown6 { get; set; } = 0;
            public byte Unknown7 { get; set; } = 0;

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

        public class DamageDescription : IWritable // same used for 0x07F4
        {
            public uint RawDamage { get; set; }
            public uint RawScaledDamage { get; set; }
            public uint AbsorbedAmount { get; set; }
            public uint ShieldAbsorbAmount { get; set; }
            public uint AdjustedDamage { get; set; }
            public uint OverkillAmount { get; set; }
            public uint Unknown6 { get; set; }
            public bool KilledTarget { get; set; }
            public byte CombatResult { get; set; }
            public byte DamageType { get; set; }

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

        public uint ServerUniqueId { get; set; }
        public uint Spell4EffectId { get; set; } = 0;
        public uint UnitId { get; set; } = 0;
        public uint TargetId { get; set; } = 0;

        public DamageDescription DamageDescriptionData { get; set; } = new DamageDescription();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
            writer.Write(Spell4EffectId, 19);
            writer.Write(UnitId);
            writer.Write(TargetId);
            DamageDescriptionData.Write(writer);
        }
    }
}
