using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerRemoveLearnedSchematic)]
    public class ServerRemoveLearnedSchematic : IWritable
    {
        public uint TradeskillId { get; set; }
        public uint TradeskillSchematic2Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillId);
            writer.Write(TradeskillSchematic2Id);
        }
    }
}
