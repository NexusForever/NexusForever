using NexusForever.Game;
using NexusForever.Game.Abstract;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientChatModeratorHandler : IMessageHandler<IWorldSession, ClientChatModerator>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;
        private readonly IRealmContext realmContext;

        public ClientChatModeratorHandler(
            IInternalMessagePublisher messagePublisher,
            IRealmContext realmContext)
        {
            this.messagePublisher = messagePublisher;
            this.realmContext     = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatModerator chatModerator)
        {
            messagePublisher.PublishAsync(new ChatChannelMemberModeratorMessage
            {
                Source = session.Player.Identity.ToInternalIdentity(),
                Type   = chatModerator.Channel.Type,
                ChatId = chatModerator.Channel.ChatId != 0 ? chatModerator.Channel.ChatId : null,
                Target = new NexusForever.Network.Internal.Message.Shared.IdentityName
                {
                    Name      = chatModerator.CharacterName,
                    RealmName = realmContext.RealmName,
                },
                Set = chatModerator.Status
            });
        }
    }
}
