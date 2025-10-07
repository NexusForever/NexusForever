using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    // Functionally the same as ClientGuildStandardModify
    [Message(GameMessageOpcode.ClientGuildSetStandard)]
    public class ClientGuildSetStandard : IReadable
    {
        public Identity GuildIdentity { get; private set; } = new();
        public GuildStandard GuildStandard { get; private set; } = new();

        public void Read(GamePacketReader reader)
        {
            GuildIdentity.Read(reader);
            GuildStandard.Read(reader);
        }
    }
}
