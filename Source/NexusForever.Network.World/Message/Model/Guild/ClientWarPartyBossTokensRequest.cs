using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ClientWarPartyBossTokensRequest)]
    public class ClientWarPartyBossTokensRequest : IReadable
    {
        public Identity GuildIdentity { get; private set; } = new(); // guild to request token info for

        public void Read(GamePacketReader reader)
        {
            GuildIdentity.Read(reader);
        }
    }
}
