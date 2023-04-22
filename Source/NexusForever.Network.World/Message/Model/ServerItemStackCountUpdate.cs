using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerItemStackCountUpdate)]
    public class ServerItemStackCountUpdate : IWritable
    {
        public ulong Guid { get; set; }
        public uint StackCount { get; set; }
        public ItemUpdateReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(StackCount);
            writer.Write(Reason, 6u);
        }
    }
}
