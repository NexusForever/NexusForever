using NexusForever.Database.Chat;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Server;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Server
{
    public class ServerWorldOfflineHandler : IHandleMessages<ServerWorldOfflineMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _chatContext;
        private readonly ChatChannelManager _chatChannelManager;

        public ServerWorldOfflineHandler(
            ChatContext chatContext,
            ChatChannelManager chatChannelManager)
        {
            _chatContext        = chatContext;
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        public async Task Handle(ServerWorldOfflineMessage message)
        {
            await foreach (ChatChannel channel in _chatChannelManager.GetChatChannelsAsync(ChatChannelReferenceType.Realm, message.RealmId))
                await channel.DisbandAsync();

            await _chatContext.SaveChangesAsync();
        }
    }
}
