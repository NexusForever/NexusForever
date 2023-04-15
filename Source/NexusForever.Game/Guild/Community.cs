using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Social;

namespace NexusForever.Game.Guild
{
    public partial class Community : GuildChat, ICommunity
    {
        public override uint MaxMembers => 20u;

        public IResidence Residence { get; set; }

        /// <summary>
        /// Create a new <see cref="ICommunity"/> using <see cref="GuildModel"/>
        /// </summary>
        public Community(GuildModel baseModel)
            : base(baseModel)
        {
            InitialiseChatChannels(ChatChannelType.Community, null);
        }

        /// <summary>
        /// Create a new <see cref="ICommunity"/> using the supplied parameters.
        /// </summary>
        public Community(string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(GuildType.Community, name, leaderRankName, councilRankName, memberRankName)
        {
            InitialiseChatChannels(ChatChannelType.Community, null);
        }

        /// <summary>
        /// Set <see cref="ICommunity"/> privacy level.
        /// </summary>
        public void SetCommunityPrivate(bool enabled)
        {
            if (enabled)
                SetFlag(GuildFlag.CommunityPrivate);
            else
                RemoveFlag(GuildFlag.CommunityPrivate);

            SendGuildFlagUpdate();
        }
    }
}
