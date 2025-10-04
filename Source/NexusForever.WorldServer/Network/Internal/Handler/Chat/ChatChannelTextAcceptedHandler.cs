using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatChannelTextAcceptedHandler : IHandleMessages<ChatTextAcceptedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;
        private readonly IRealmContext realmContext;

        public ChatChannelTextAcceptedHandler(
            IPlayerManager playerManager,
            IRealmContext realmContext)
        {
            this.playerManager = playerManager;
            this.realmContext  = realmContext;
        }

        #endregion

        public Task Handle(ChatTextAcceptedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Source.ToGameIdentity());
            if (player == null)
                return Task.CompletedTask;

            var chatAccept = new ServerChatAccept
            {
                Guid          = player.Guid,
                Name          = player.Name,
                ChatMessageId = message.ChatMessageId
            };

            if (message.Target != null && message.TargetName != null)
            {
                chatAccept.Name = message.TargetName.Name;
                if (realmContext.RealmId != message.Target.RealmId)
                    chatAccept.RealmName = message.TargetName.RealmName;
            }
            
            player.Session.EnqueueMessageEncrypted(chatAccept);

            return Task.CompletedTask;
        }
    }
}
