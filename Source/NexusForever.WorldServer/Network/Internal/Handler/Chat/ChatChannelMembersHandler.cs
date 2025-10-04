using System.Linq;
using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatChannelMembersHandler : IHandleMessages<ChatChannelMembersMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public ChatChannelMembersHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(ChatChannelMembersMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Member.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerChatList
            {
                Type      = message.ChatChannel.Type,
                ChannelId = message.ChatChannel.ChatId,
                Names     = message.ChatChannel.Members.Select(m => m.Character.IdentityName.Name).ToList(),
                Flags     = message.ChatChannel.Members.Select(m => m.Flags).ToList(),
                More      = false
            });

            return Task.CompletedTask;
        }
    }
}
