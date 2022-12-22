using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class InventoryItem : IWritable
    {
        public Item Item { get; set; }
        public ItemUpdateReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Item.Write(writer);
            writer.Write(Reason, 6u);
        }
    }
}
