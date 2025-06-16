using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // If there is a new unit for the client to assume control of, the server will send this message.
    // When the 
    [Message(GameMessageOpcode.ServerMovementControlRefresh)]
    public class ServerMovementControl : IWritable
    {
        public uint Ticket { get; set; }
        public bool Immediate { get; set; }
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Ticket);
            writer.Write(Immediate);
            writer.Write(UnitId);
        }
    }
}
