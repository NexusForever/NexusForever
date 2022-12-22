using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCostumeItemUnlock)]
    public class ServerCostumeItemUnlock : IWritable
    {
        public uint ItemId { get; set; }
        public CostumeUnlockResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemId, 18u);
            writer.Write(Result, 32u);
        }
    }
}
