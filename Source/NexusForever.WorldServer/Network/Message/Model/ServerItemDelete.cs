using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerItemDelete)]
    public class ServerItemDelete : IWritable
    {
        public ulong Guid { get; set; }
        public byte Reason { get; set; } = 0x15;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Reason, 6u);
        }
    }
}
