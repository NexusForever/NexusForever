using NexusForever.Database.Chat;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Server;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Server
{
    public class ServerWorldOnlineHandler : IHandleMessages<ServerWorldOnlineMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _chatContext;
        private readonly ChatChannelManager _chatChannelManager;

        public ServerWorldOnlineHandler(
            ChatContext chatContext,
            ChatChannelManager chatChannelManager)
        {
            _chatContext        = chatContext;
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        public async Task Handle(ServerWorldOnlineMessage message)
        {
            await CreateChatChannel(ChatChannelType.Trade, message.RealmId);
            await CreateChatChannel(ChatChannelType.Nexus, message.RealmId);
            await _chatContext.SaveChangesAsync();
        }

        private async Task CreateChatChannel(ChatChannelType type, ushort realmId)
        {
            if (await _chatChannelManager.GetChatChannelAsync(type, ChatChannelReferenceType.Realm, realmId) != null)
                return;

            var channel = _chatChannelManager.CreateChatChannel(type, null, null);
            channel.ReferenceType = ChatChannelReferenceType.Realm;
            channel.ReferenceValue = realmId;
        }
    }
}
