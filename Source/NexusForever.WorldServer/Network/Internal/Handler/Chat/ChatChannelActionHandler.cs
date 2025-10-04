using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Internal.Message.Chat.Shared;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatChannelActionHandler : IHandleMessages<ChatChannelActionMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public ChatChannelActionHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(ChatChannelActionMessage message)
        {
            var chatAction = new ServerChatAction
            {
                Channel = new Channel
                {
                    Type   = message.Channel.Type,
                    ChatId = message.Channel.ChatId
                },
                Action    = message.Action,
                NameActor = message.Source.Character.IdentityName.Name
            };

            if (message.Target != null)
                chatAction.NameActedOn = message.Target.Character.IdentityName.Name;
            else
                chatAction.NameActedOn = string.Empty;

            foreach (ChatChannelMember member in message.Channel.Members)
            {
                IPlayer player = playerManager.GetPlayer(member.Identity.ToGameIdentity());
                player?.Session.EnqueueMessageEncrypted(chatAction);
            }

            return Task.CompletedTask;
        }
    }
}
