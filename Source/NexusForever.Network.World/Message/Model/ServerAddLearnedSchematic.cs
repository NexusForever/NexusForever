using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAddLearnedSchematic)]
    public class ServerAddLearnedSchematic : IWritable
    {
        public uint TradeskillId { get; set; }
        public uint TradeskillSchematic2Id { get; set; }
        public float Field_8 { get; set; }
        public float Field_C { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillId);
            writer.Write(TradeskillSchematic2Id);
            writer.Write(Field_8);
            writer.Write(Field_C);
        }
    }
}
