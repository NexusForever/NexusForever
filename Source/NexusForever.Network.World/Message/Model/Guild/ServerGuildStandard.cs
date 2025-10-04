using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildStandard)]
    public class ServerGuildStandard : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public GuildStandard GuildStandard { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            GuildStandard.Write(writer);
        }
    }
}
