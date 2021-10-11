using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;

namespace NexusForever.WorldServer.Game.Guild
{
    public abstract class GuildChat : GuildBase
    {
        private ChatChannel memberChannel;
        private ChatChannel officerChannel;

        /// <summary>
        /// Create a new <see cref="GuildChat"/> from an existing database model.
        /// </summary>
        public GuildChat(GuildModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Create a new <see cref="GuildChat"/> using the supplied parameters.
        /// </summary>
        public GuildChat(GuildType type, string name, string leaderRankName, string councilRankName, string memberRankName)
            : base(type, name, leaderRankName, councilRankName, memberRankName)
        {
        }

        /// <summary>
        /// Initialise member and office <see cref="ChatChannel"/>'s.
        /// </summary>
        public void InitialiseChatChannels(ChatChannelType? memberChannelType, ChatChannelType? officerChannelType)
        {
            if (memberChannelType.HasValue)
                memberChannel = GlobalChatManager.Instance.CreateChatChannel(memberChannelType.Value, Id, Name);
            if (officerChannelType.HasValue)
                officerChannel = GlobalChatManager.Instance.CreateChatChannel(officerChannelType.Value, Id, Name);
        }

        /// <summary>
        /// Invoked when a <see cref="GuildMember"/> comes online.
        /// </summary>
        protected override void MemberOnline(GuildMember member)
        {
            // add member to any chat channels if applicable
            if (member.Rank.HasPermission(GuildRankPermission.MemberChat))
                memberChannel?.Join(member.CharacterId);
            if (member.Rank.HasPermission(GuildRankPermission.OfficerChat))
                officerChannel?.Join(member.CharacterId);

            base.MemberOnline(member);
        }

        /// <summary>
        /// Invoked when a <see cref="GuildMember"/> goes offline.
        /// </summary>
        protected override void MemberOffline(GuildMember member)
        {
            // remove member to any chat channels if applicable
            if (member.Rank.HasPermission(GuildRankPermission.MemberChat))
                memberChannel?.Leave(member.CharacterId);
            if (member.Rank.HasPermission(GuildRankPermission.OfficerChat))
                officerChannel?.Leave(member.CharacterId);

            base.MemberOffline(member);
        }

        /// <summary>
        /// Rename <see cref="GuildBase"/> with supplied name.
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
