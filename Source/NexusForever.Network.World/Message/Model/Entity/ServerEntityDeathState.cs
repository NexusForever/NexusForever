using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityDeathState)]
    public class ServerEntityDeathState : IWritable
    {
        public uint UnitId { get; set; }
        public bool IsDead { get; set; }
        public byte Reason { get; set; } // unused by client
        public uint RezHealth { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(IsDead);
            writer.Write(Reason, 5u);
            writer.Write(RezHealth);
        }
    }
}