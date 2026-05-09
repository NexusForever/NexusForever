using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.ICComm
{
    [Message(GameMessageOpcode.ServerICCommDirectedMessage)]
    public class ServerICCommDirectedMessage : IWritable
    {
        public ulong IccomId { get; set; }
        public string Message { get; set; }
        public string SenderName { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(IccomId);
            writer.WriteString(Message);
            writer.WriteStringWide(SenderName);
        }
    }
}

