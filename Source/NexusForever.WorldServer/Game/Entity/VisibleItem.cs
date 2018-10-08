using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class VisibleItem : IWritable
    {
        public ItemSlot Slot { get; }
        public ushort DisplayId { get; }
        public ushort Unknown8 { get; }
        public uint UnknownC { get; }

        public VisibleItem(ItemSlot slot, ushort displayId)
        {
            Slot      = slot;
            DisplayId = displayId;
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Slot, 7);
            writer.Write(DisplayId, 15);
            writer.Write(Unknown8, 14);
            writer.Write(UnknownC);
        }
    }
}
