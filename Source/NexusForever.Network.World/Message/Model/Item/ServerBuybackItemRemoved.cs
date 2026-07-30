using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerBuybackItemRemoved)]
    public class ServerBuybackItemRemoved : IWritable
    {
        public uint UniqueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UniqueId);
        }
    }
}
