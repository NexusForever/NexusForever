using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class ItemLocation : IReadable, IWritable
    {
        public InventoryLocation Location { get; set; }
        public uint BagIndex { get; set; }

        public void Read(GamePacketReader reader)
        {
            Location = reader.ReadEnum<InventoryLocation>(9u);
            BagIndex = reader.ReadUInt();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Location, 9u);
            writer.Write(BagIndex);
        }
    }
}
