using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class TargetGuild : IReadable, IWritable
    {
        public ushort RealmId { get; set; }
        public ulong GuildId { get; set; }

        public void Read(GamePacketReader reader)
        {
            RealmId = reader.ReadUShort(14u);
            GuildId = reader.ReadULong();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(GuildId);
        }
    }
}
