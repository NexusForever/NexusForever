using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Guild;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    // Functionally the same as ClientGuildStandardModify
    [Message(GameMessageOpcode.ClientGuildSetStandard)]
    public class ClientGuildSetStandard : IReadable
    {
        public Identity GuildIdentity { get; set; } = new();
        public GuildStandard GuildStandard { get; set; } = new();

        public void Read(GamePacketReader reader)
        {
            GuildIdentity.Read(reader);
            GuildStandard.Read(reader);
        }
    }
}
