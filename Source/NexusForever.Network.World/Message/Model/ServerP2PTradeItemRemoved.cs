using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerP2PTradeItemRemoved)]
    public class ServerPTPTradeItemRemoved : IWritable
    {
        public ulong ItemGuid { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
        }
    }
}
