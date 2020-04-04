using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerSupplySatchelUpdate)]
    public class ServerSupplySatchelUpdate : IWritable
    {
        public ushort MaterialId { get; set; }
        public ushort StackCount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MaterialId, 14u);
            writer.Write(StackCount);
        }
    }
}
