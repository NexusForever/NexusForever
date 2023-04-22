using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityInteractiveUpdate)]
    public class ServerEntityInteractiveUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public bool InUse { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(InUse);
        }
    }
}
