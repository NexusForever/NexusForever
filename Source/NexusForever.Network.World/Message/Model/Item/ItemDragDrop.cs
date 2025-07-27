using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    public class ItemDragDrop : IReadable, IWritable
    {
        public ulong ItemGuid { get; set; }
        public InventoryId DragDrop { get; set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            DragDrop.Read(reader);
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            DragDrop.Write(writer);
        }
    }
}
