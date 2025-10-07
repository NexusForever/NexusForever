using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    // Not seen in sniffs. Seems that ServerGuildInit and ServerGuildJoin initialise a standard set of 10 guild bank tabs.
    // In theory seems you can have more than 10 guild bank tabs using this.
    [Message(GameMessageOpcode.ServerGuildBankTabCount)]
    public class ServerGuildBankTabCount : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public uint Count { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(Count);
        }
    }
}
