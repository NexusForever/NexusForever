using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildEventLogDeleteEntry)]
    public class ServerGuildEventLogDeleteEntry : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public ulong EventLogId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(EventLogId);
        }
    }
}
