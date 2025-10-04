using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Social;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Guild
{
    public class GuildChannelDataManager
    {
        private readonly Dictionary<GuildType, (ChatChannelType Main, ChatChannelType? Officer)> Channels = new()
        {
            { GuildType.Guild,     (ChatChannelType.Guild,     ChatChannelType.GuildOfficer) },
            { GuildType.Circle,    (ChatChannelType.Society,   null) },
            { GuildType.WarParty,  (ChatChannelType.WarParty,  ChatChannelType.WarPartyOfficer) },
            { GuildType.Community, (ChatChannelType.Community, null) }
        };

        public ChatChannelType? GetMainChannelType(GuildType type)
        {
            if (Channels.TryGetValue(type, out (ChatChannelType Main, ChatChannelType? Officer) channel))
                return channel.Main;
            return null;
        }

        public ChatChannelType? GetOfficerChannelType(GuildType type)
        {
            if (Channels.TryGetValue(type, out (ChatChannelType Main, ChatChannelType? Officer) channel))
                return channel.Officer;
            return null;
        }
    }
}
