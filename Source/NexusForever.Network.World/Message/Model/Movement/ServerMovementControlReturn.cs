using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // If there is a new unit for the client to assume control, the server will send this message.
    [Message(GameMessageOpcode.ServerMovementControlReturn)]
    public class ServerMovementControlReturn : IWritable
    {
        public uint Ticket { get; set; }
        public bool Immediate { get; set; } // executes any queued movement commands
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Ticket);
            writer.Write(Immediate);
            writer.Write(UnitId);
        }
    }
}
