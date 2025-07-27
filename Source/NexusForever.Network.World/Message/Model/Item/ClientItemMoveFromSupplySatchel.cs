using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientItemMoveFromSupplySatchel)]
    public class ClientItemMoveFromSupplySatchel : IReadable
    {
        public ushort TradeskillMaterialId { get; private set; }
        public uint Amount { get; private set; }
        public ItemLocation To { get; private set; } = new ItemLocation(); // Always sends BagSlot = -1 and Location = 300. Assumption is this means drop in first available bag slot
                                                                           // or on existing stack in player's bag
        public void Read(GamePacketReader reader)
        {
            TradeskillMaterialId = reader.ReadUShort(14u);
            Amount = reader.ReadUInt();
            To.Read(reader);
        }
    }
}
