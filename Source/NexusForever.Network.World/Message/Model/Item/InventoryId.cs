using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    // Similar to InventoryId used in the client UI, but with an additional Count field and the packed structure
    // is 64 bits in size rather than 32 bits.
    public class InventoryId : IReadable, IWritable
    {
        public byte BagSlot { get; set; }
        public InventoryLocation Location { get; set; }
        public ushort Count { get; set; } // used in client UI but never sent to or from server, can always be 0 in messages

        public void Write(GamePacketWriter writer)
        {
            ulong location = (ulong)BagSlot | ((ulong)Location << 8) | ((ulong)Count << 16) ;
        }

        public void Read(GamePacketReader reader)
        {
            ulong locationCompact = reader.ReadULong();

            BagSlot = (byte)(locationCompact & 0xFF);
            Location = (InventoryLocation)((locationCompact >> 8) & 0xFF);
            Count = (ushort)((locationCompact >> 16) & 0xFFFF);
        }
    }
}
