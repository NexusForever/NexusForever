using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerDisembark, MessageDirection.Server)]
    public class ServerDisembark : IWritable
    {
        public uint MountId { get; set; }
        public uint OwnerId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MountId);
            writer.Write(OwnerId);
        }
    }
}