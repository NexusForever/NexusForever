using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerWarPartyBossTokens)]
    public class ServerWarPartyBossTokens : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public List<WarPartyBossToken> Tokens { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(Tokens.Count);
            Tokens.ForEach(token => token.Write(writer));
        }
    }
}
