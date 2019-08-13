using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
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
