using NexusForever.Database.Chat;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Guild;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Guild
{
    public class GuildCreatedHandler : IHandleMessages<GuildCreatedMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _chatContext;
        private readonly ChatChannelManager _chatChannelManager;
        private readonly GuildChannelDataManager _dataManager;

        public GuildCreatedHandler(
            ChatContext chatContext,
            ChatChannelManager chatChannelManager,
            GuildChannelDataManager dataManager)
        {
            _chatContext        = chatContext;
            _chatChannelManager = chatChannelManager;
            _dataManager        = dataManager;
        }

        #endregion

        public async Task Handle(GuildCreatedMessage message)
        {
            ChatChannelType? mainChannelType = _dataManager.GetMainChannelType(message.Type);
            if (mainChannelType != null)
            {
                ChatChannel mainChannel = _chatChannelManager.CreateChatChannel(mainChannelType.Value, null, null);
                mainChannel.ReferenceType  = ChatChannelReferenceType.Guild;
                mainChannel.ReferenceValue = message.GuildId;
            }

            ChatChannelType? officerChannelType = _dataManager.GetOfficerChannelType(message.Type);
            if (officerChannelType != null)
            {
                ChatChannel officerChannel = _chatChannelManager.CreateChatChannel(officerChannelType.Value, null, null);
                officerChannel.ReferenceType  = ChatChannelReferenceType.Guild;
                officerChannel.ReferenceValue = message.GuildId;
            }

            await _chatContext.SaveChangesAsync();
        }
    }
}
