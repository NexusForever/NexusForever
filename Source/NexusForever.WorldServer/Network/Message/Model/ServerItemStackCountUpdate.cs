using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
