using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGuildInfoMessage)]
    public class ServerGuildInfoMessage : IWritable
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
