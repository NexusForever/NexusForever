using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Social;

namespace NexusForever.Game.Guild
{
    public class WarParty : GuildChat, IWarParty
    {
        public override uint MaxMembers => 30u;

        /// <summary>
        /// Create a new <see cref="IWarParty"/> using <see cref="GuildModel"/>
        /// </summary>
        public WarParty(GuildModel baseModel)
            : base(baseModel)
        {
            InitialiseChatChannels(ChatChannelType.WarParty, ChatChannelType.WarPartyOfficer);
        }

        /// <summary>
        /// Create a new <see cref="IWarParty"/> using the supplied parameters.
        /// </summary>
        public WarParty(string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(GuildType.WarParty, name, leaderRankName, councilRankName, memberRankName)
        {
            InitialiseChatChannels(ChatChannelType.WarParty, ChatChannelType.WarPartyOfficer);
        }
    }
}
