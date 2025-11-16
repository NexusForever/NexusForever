using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.ICComm
{
    // Sent when client receives an ICComm message to a channel it is not in
    // Presumably to let server know to resend a message if the server thought the client was in the channel
    [Message(GameMessageOpcode.ClientICCommChannelNotJoined)]
    public class ClientICCommChannelNotJoined : IReadable
    {
        public ulong IccomId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            IccomId = reader.ReadULong();
        }
    }
}
