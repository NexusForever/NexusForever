using NexusForever.Game;
using NexusForever.Game.Abstract;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Internal.Message.Shared;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Chat;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientChatOwnerHandler : IMessageHandler<IWorldSession, ClientChatOwner>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;
        private readonly IRealmContext realmContext;

        public ClientChatOwnerHandler(
            IInternalMessagePublisher messagePublisher,
            IRealmContext realmContext)
        {
            this.messagePublisher = messagePublisher;
            this.realmContext     = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatOwner chatOwner)
        {
            messagePublisher.PublishAsync(new ChatChannelMemberOwnerMessage
            {
                Source = session.Player.Identity.ToInternalIdentity(),
                Type   = chatOwner.Channel.ChatChannelId,
                ChatId = chatOwner.Channel.ChatId != 0 ? chatOwner.Channel.ChatId : null,
                Target = new IdentityName
                {
                    Name      = chatOwner.CharacterName,
                    RealmName = realmContext.RealmName,
                }
            }).FireAndForgetAsync();
        }
    }
}
