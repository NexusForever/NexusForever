using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
