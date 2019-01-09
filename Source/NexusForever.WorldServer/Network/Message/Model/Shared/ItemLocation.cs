using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
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
