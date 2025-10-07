using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class Identity : IReadable, IWritable
    {
        public ushort RealmId { get; set; }
        public ulong Id { get; set; }

        public void Read(GamePacketReader reader)
        {
            RealmId = reader.ReadUShort(14u);
            Id = reader.ReadULong();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(Id);
        }

        public override string ToString()
        {
            return $"{RealmId}:{Id}";
        }
    }
}
