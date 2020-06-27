using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class Item : IWritable
    {
        public class UnknownStructure : IWritable
        {
            public byte Unknown0 { get; set; }
            public uint Unknown4 { get; set; }
            public uint Unknown8 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 3u);
                writer.Write(Unknown4);
                writer.Write(Unknown8);
            }
        }

        public class UnknownStructure2 : IWritable
        {
            public ushort Unknown0 { get; set; }
            public ulong Unknown8 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 14u);
                writer.Write(Unknown8);
            }
        }

        public ulong Guid { get; set; }
        public ulong Unknown0 { get; set; }
        public uint ItemId { get; set; }
        public ItemLocation LocationData { get; set; }
        public uint StackCount { get; set; }
        public uint Charges { get; set; }
        public ulong RandomCircuitData { get; set; }
        public uint RandomGlyphData { get; set; }
        public ulong ThresholdData { get; set; }
        public float Durability { get; set; }
        public uint Unknown44 { get; set; }
        public byte Unknown48 { get; set; }
        public uint DyeData { get; set; }
        public uint DynamicFlags { get; set; }
        public uint ExpirationTimeLeft { get; set; }
        public UnknownStructure[] Unknown58 { get; set; }
        public uint Unknown70 { get; set; }
        public List<uint> Microchips { get; } = new List<uint>();
        public List<uint> Glyphs { get; } = new List<uint>();
        public List<UnknownStructure2> Unknown88 { get; } = new List<UnknownStructure2>();
        public uint EffectiveItemLevel { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Unknown0);
            writer.Write(ItemId, 18u);
            LocationData.Write(writer);
            writer.Write(StackCount);
            writer.Write(Charges);
            writer.Write(RandomCircuitData);
            writer.Write(RandomGlyphData);
            writer.Write(ThresholdData);
            writer.Write(Durability);
            writer.Write(Unknown44);
            writer.Write(Unknown48);
            writer.Write(DyeData);
            writer.Write(DynamicFlags);
            writer.Write(ExpirationTimeLeft);

            for (uint i = 0u; i < Unknown58.Length; i++)
                Unknown58[i].Write(writer);

            writer.Write(Unknown70, 18u);

            writer.Write(Microchips.Count, 3u);
            Microchips.ForEach(m => writer.Write(m));
            writer.Write(Glyphs.Count, 4u);
            Glyphs.ForEach(g => writer.Write(g));
            writer.Write(Glyphs.Count, 6u);
            Unknown88.ForEach(u => u.Write(writer));

            writer.Write(EffectiveItemLevel);
        }
    }
}
