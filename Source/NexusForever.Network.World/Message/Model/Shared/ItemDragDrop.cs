using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class ItemDragDrop : IReadable, IWritable
    {
        public ulong Guid { get; set; }
        public ulong DragDrop { get; set; }

        public void Read(GamePacketReader reader)
        {
            Guid     = reader.ReadULong();
            DragDrop = reader.ReadULong();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(DragDrop);
        }
    }
}
