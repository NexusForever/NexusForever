using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
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
}