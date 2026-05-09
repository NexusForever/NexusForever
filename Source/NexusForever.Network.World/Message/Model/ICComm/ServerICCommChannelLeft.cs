using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.ICComm
{
    [Message(GameMessageOpcode.ServerICCommChannelLeft)]
    public class ServerICCommChannelLeft : IWritable
    {
        public ulong IccomId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(IccomId);
        }
    }
}
