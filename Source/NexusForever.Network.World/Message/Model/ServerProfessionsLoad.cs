using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerProfessionsLoad)]
    public class ServerProfessionsLoad : IWritable
    {
        public class SchematicUnknown : IWritable
        {
            public uint TradeskillSchematic2Id { get; set; }
            public float Field_4 { get; set; }
            public float Field_8 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(TradeskillSchematic2Id);
                writer.Write(Field_4);
                writer.Write(Field_8);
            }
        }

        List<TradeskillInfo> Tradeskills { get; set; } = new List<TradeskillInfo>();
        List<uint> LearnedSchematics { get; set; } = new List<uint>(); // TradeskillSchematic2Id
        List<SchematicUnknown> SchematicUnknowns { get; set; } = new List<SchematicUnknown>();
        List<uint> UnknownArray { get; set; } = new List<uint>();
        public uint RelearnCooldown { get; set; } // Sent as an offset from the time now, to the finish time, in milliseconds.

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Tradeskills.Count);
            foreach (var tradeskill in Tradeskills)
            {
                tradeskill.Write(writer);
            }

            writer.Write(LearnedSchematics.Count);
            foreach (var schematic in LearnedSchematics)
            {
                writer.Write(schematic);
            }

            writer.Write(SchematicUnknowns.Count);
            foreach (var unknown in SchematicUnknowns)
            {
                unknown.Write(writer);
            }

            writer.Write(UnknownArray.Count);
            foreach (var id in UnknownArray)
            {
                writer.Write(id);
            }

            writer.Write(RelearnCooldown);
        }
    }
}
