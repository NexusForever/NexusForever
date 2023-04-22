using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Social;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Social;

namespace NexusForever.Game.Guild
{
    public abstract class GuildChat : GuildBase, IGuildChat
    {
        private IChatChannel memberChannel;
        private IChatChannel officerChannel;

        /// <summary>
        /// Create a new <see cref="IGuildChat"/> from an existing database model.
        /// </summary>
        public GuildChat(GuildModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Create a new <see cref="IGuildChat"/> using the supplied parameters.
        /// </summary>
        public GuildChat(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(type, name, leaderRankName, councilRankName, memberRankName)
        {
        }

        /// <summary>
        /// Initialise member and office <see cref="IChatChannel"/>'s.
        /// </summary>
        public void InitialiseChatChannels(ChatChannelType? memberChannelType, ChatChannelType? officerChannelType)
        {
            if (memberChannelType.HasValue)
                memberChannel = GlobalChatManager.Instance.CreateChatChannel(memberChannelType.Value, Id, Name);
            if (officerChannelType.HasValue)
                officerChannel = GlobalChatManager.Instance.CreateChatChannel(officerChannelType.Value, Id, Name);
        }

        /// <summary>
        /// Invoked when a <see cref="IGuildMember"/> comes online.
        /// </summary>
        protected override void MemberOnline(IGuildMember member)
        {
            // add member to any chat channels if applicable
            if (member.Rank.HasPermission(GuildRankPermission.MemberChat))
                memberChannel?.Join(member.CharacterId);
            if (member.Rank.HasPermission(GuildRankPermission.OfficerChat))
                officerChannel?.Join(member.CharacterId);

            base.MemberOnline(member);
        }

        /// <summary>
        /// Invoked when a <see cref="IGuildMember"/> goes offline.
        /// </summary>
        protected override void MemberOffline(IGuildMember member)
        {
            // remove member to any chat channels if applicable
            if (member.Rank.HasPermission(GuildRankPermission.MemberChat))
                memberChannel?.Leave(member.CharacterId);
            if (member.Rank.HasPermission(GuildRankPermission.OfficerChat))
                officerChannel?.Leave(member.CharacterId);

            base.MemberOffline(member);
        }

        /// <summary>
        /// Rename <see cref="IGuildBase"/> with supplied name.
        /// </summary>
        public override void RenameGuild(string name)
        {
            base.RenameGuild(name);

            // update channel names with new guild name
            if (memberChannel != null)
                memberChannel.Name = name;
            if (officerChannel != null)
                officerChannel.Name = name;
        }
    }
}
