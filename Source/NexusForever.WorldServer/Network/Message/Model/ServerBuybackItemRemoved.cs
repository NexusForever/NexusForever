using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
