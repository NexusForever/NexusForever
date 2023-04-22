using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCostumeItemUnlockMultiple)]
    public class ServerCostumeItemUnlockMultiple : IWritable
    {
        public CostumeUnlockResult Result { get; set; }
        public List<uint> ItemsIds { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 32u);
            writer.Write(ItemsIds.Count);
            ItemsIds.ForEach(i => writer.Write(i));
        }
    }
}
