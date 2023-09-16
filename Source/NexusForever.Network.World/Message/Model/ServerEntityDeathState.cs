using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityDeathState)]
    public class ServerEntityDeathState : IWritable
    {
        public uint UnitId { get; set; }
        public bool Dead { get; set; }
        // doesn't seem to be used by the client
        public byte Reason { get; set; }
        public uint RezHealth { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Dead);
            writer.Write(Reason, 5u);
            writer.Write(RezHealth);
        }
    }
}