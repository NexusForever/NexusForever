using NexusForever.Database.Chat;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Guild;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Guild
{
    public class GuildMemberRankUpdatedHandler : IHandleMessages<GuildMemberRankUpdatedMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _chatContext;
        private readonly ChatChannelManager _chatChannelManager;
        private readonly GuildChannelDataManager _dataManager;

        public GuildMemberRankUpdatedHandler(
            ChatContext chatContext,
            ChatChannelManager chatChannelManager,
            GuildChannelDataManager dataManager)
        {
            _chatContext        = chatContext;
            _chatChannelManager = chatChannelManager;
            _dataManager        = dataManager;
        }

        #endregion

        public async Task Handle(GuildMemberRankUpdatedMessage message)
        {
            ChatChannelType? mainChannelType = _dataManager.GetMainChannelType(message.Type);
            if (mainChannelType != null)
            {
                ChatChannel mainChannel = await _chatChannelManager.GetChatChannelAsync(mainChannelType.Value, ChatChannelReferenceType.Guild, message.GuildId);
                if ((message.Permissions & GuildRankPermission.MemberChat) != 0)
                    mainChannel?.AddMemberAsync(message.Member.ToChatIdentity());
                else
                    mainChannel?.RemoveMemberAsync(message.Member.ToChatIdentity(), ChatChannelLeaveReason.Leave);
            }

            ChatChannelType? officerChannelType = _dataManager.GetOfficerChannelType(message.Type);
            if (officerChannelType != null)
            {
                ChatChannel officerChannel = await _chatChannelManager.GetChatChannelAsync(officerChannelType.Value, ChatChannelReferenceType.Guild, message.GuildId);
                if ((message.Permissions & GuildRankPermission.OfficerChat) != 0)
                    officerChannel?.AddMemberAsync(message.Member.ToChatIdentity());
                else
                    officerChannel?.RemoveMemberAsync(message.Member.ToChatIdentity(), ChatChannelLeaveReason.Leave);
            }

            await _chatContext.SaveChangesAsync();
        }
    }
}
