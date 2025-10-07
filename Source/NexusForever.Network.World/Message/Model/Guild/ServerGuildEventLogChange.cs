using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    // Can add a new entry or change an existing entry
    [Message(GameMessageOpcode.ServerGuildEventLogChange)]
    public class ServerGuildEventLogChange : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public GuildEventLog LogEntry { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            LogEntry.Write(writer);
        }
    }
}
