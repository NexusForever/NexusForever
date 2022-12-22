using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMailUnavailable)]
    public class ServerMailUnavailable : IWritable
    {
        public ulong MailId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MailId);
        }
    }
}
