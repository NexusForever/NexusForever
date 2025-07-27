using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemDelete)]
    public class ServerItemDelete : IWritable
    {
        public ulong ItemGuid { get; set; }
        public ItemUpdateReason Reason { get; set; } = ItemUpdateReason.Loot; // Reason can be HousingCrate if item sent to crate from inventory

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(Reason, 6u);
        }
    }
}
