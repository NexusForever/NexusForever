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
    public class ClientChatKickHandler : IMessageHandler<IWorldSession, ClientChatKick>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;
        private readonly IRealmContext realmContext;

        public ClientChatKickHandler(
            IInternalMessagePublisher messagePublisher,
            IRealmContext realmContext)
        {
            this.messagePublisher = messagePublisher;
            this.realmContext = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientChatKick chatKick)
        {
            messagePublisher.PublishAsync(new ChatChannelMemberKickMessage
            {
                Source = session.Player.Identity.ToInternalIdentity(),
                Type   = chatKick.Channel.ChatChannelId,
                ChatId = chatKick.Channel.ChatId != 0 ? chatKick.Channel.ChatId : null,
                Target = new IdentityName
                {
                    Name      = chatKick.CharacterName,
                    RealmName = realmContext.RealmName,
                }
            }).FireAndForgetAsync();
        }
    }
}
