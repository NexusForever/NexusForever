using NexusForever.Game;
using NexusForever.Game.Abstract;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Internal.Message.Shared;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Chat
{
    public class ClientChatMuteHandler : IMessageHandler<IWorldSession, ClientChatMute>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;
        private readonly IRealmContext realmContext;

        public ClientChatMuteHandler(
            IInternalMessagePublisher messagePublisher,
            IRealmContext realmContext)
        {
            this.messagePublisher = messagePublisher;
            this.realmContext     = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatMute chatMute)
        {
            messagePublisher.PublishAsync(new ChatChannelMemberMuteMessage
            {
                Source = session.Player.Identity.ToInternalIdentity(),
                Type   = chatMute.Channel.Type,
                ChatId = chatMute.Channel.ChatId != 0 ? chatMute.Channel.ChatId : null,
                Target = new IdentityName
                {
                    Name      = chatMute.CharacterName,
                    RealmName = realmContext.RealmName,
                },
                Set = chatMute.Status
            }).FireAndForgetAsync();
        }
    }
}
