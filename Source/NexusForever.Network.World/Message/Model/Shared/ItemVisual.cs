using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class ItemVisual : IWritable
    {
        public ItemSlot Slot { get; set; }
        public ushort DisplayId { get; set; }
        public ushort ColourSetId { get; set; }
        public int DyeData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Slot, 7);
            writer.Write(DisplayId, 15);
            writer.Write(ColourSetId, 14);
            writer.Write(DyeData);
        }
    }
}
