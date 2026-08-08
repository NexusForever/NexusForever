using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Sent by the client to acknowledge a server movement control ticket.
    // Sends back the ticket value sent in ServerMovementControl (0x0636).
    [Message(GameMessageOpcode.ClientMovementControlReturnAck)]
    public class ClientMovementControlReturnAck : IReadable
    {
        public uint Ticket { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Ticket = reader.ReadUInt();
        }
    }
}
