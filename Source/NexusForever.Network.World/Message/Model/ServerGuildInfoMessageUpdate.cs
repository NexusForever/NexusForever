using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildInfoMessageUpdate)]
    public class ServerGuildInfoMessageUpdate : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong GuildId { get; set; }
        public string AdditionalInfo { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(GuildId);
            writer.WriteStringWide(AdditionalInfo);
        }
    }
}
