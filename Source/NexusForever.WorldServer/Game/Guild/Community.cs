using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Game.Guild
{
    public partial class Community : GuildChat
    {
        public override uint MaxMembers => 20u;

        public Residence Residence { get; set; }

        /// <summary>
        /// Create a new <see cref="Community"/> using <see cref="GuildModel"/>
        /// </summary>
        public Community(GuildModel baseModel) 
            : base(baseModel)
        {
            InitialiseChatChannels(ChatChannelType.Community, null);
        }

        /// <summary>
        /// Create a new <see cref="Community"/> using the supplied parameters.
        /// </summary>
        public Community(string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(GuildType.Community, name, leaderRankName, councilRankName, memberRankName)
        {
            InitialiseChatChannels(ChatChannelType.Community, null);
        }

        /// <summary>
        /// Set <see cref="Community"/> privacy level.
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
