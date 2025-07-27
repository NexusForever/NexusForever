using NexusForever.Game.Static.Item;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemFlags)]
    public class ServerItemFlags : IWritable
    {
        public ulong ItemGuid { get; set; }
        public ItemFlags Flags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(Flags, 8u);
        }
    }
}
