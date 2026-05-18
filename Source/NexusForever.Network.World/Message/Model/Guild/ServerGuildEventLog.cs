using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildEventLog)]
    public class ServerGuildEventLog : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public bool IsBankLog  { get; set; } // true = is bank log information, false = regular guild log information
        public List<GuildEventLog> LogEntry { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(IsBankLog);
            writer.Write(LogEntry.Count);
            foreach (GuildEventLog entry in LogEntry)
            {
                entry.Write(writer);
            }
        }
    }
}
