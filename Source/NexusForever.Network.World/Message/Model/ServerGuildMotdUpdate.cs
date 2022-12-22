using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildMotdUpdate)]
    public class ServerGuildMotdUpdate : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong GuildId { get; set; }
        public string MessageOfTheDay { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(GuildId);
            writer.WriteStringWide(MessageOfTheDay);
        }
    }
}
