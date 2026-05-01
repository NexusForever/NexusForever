using NexusForever.Database.Chat;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Guild;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Guild
{
    public class GuildMemberAddedHandler : IHandleMessages<GuildMemberAddedMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _chatContext;
        private readonly ChatChannelManager _chatChannelManager;
        private readonly GuildChannelDataManager _dataManager;

        public GuildMemberAddedHandler(
            ChatContext chatContext,
            ChatChannelManager chatChannelManager,
            GuildChannelDataManager dataManager)
        {
            _chatContext        = chatContext;
            _chatChannelManager = chatChannelManager;
            _dataManager        = dataManager;
        }

        #endregion

        public async Task Handle(GuildMemberAddedMessage message)
        {
            ChatChannelType? mainChannelType = _dataManager.GetMainChannelType(message.Type);
            if (mainChannelType != null && (message.Permissions & GuildRankPermission.MemberChat) != 0)
            {
                ChatChannel mainChannel = await _chatChannelManager.GetChatChannelAsync(mainChannelType.Value, ChatChannelReferenceType.Guild, message.GuildId);
                mainChannel?.AddMemberAsync(message.Member.ToChatIdentity());
            }

            ChatChannelType? officerChannelType = _dataManager.GetOfficerChannelType(message.Type);
            if (officerChannelType != null && (message.Permissions & GuildRankPermission.OfficerChat) != 0)
            {
                ChatChannel officerChannel = await _chatChannelManager.GetChatChannelAsync(officerChannelType.Value, ChatChannelReferenceType.Guild, message.GuildId);
                officerChannel?.AddMemberAsync(message.Member.ToChatIdentity());
            }

            await _chatContext.SaveChangesAsync();
        }
    }
}
