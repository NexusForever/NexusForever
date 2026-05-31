using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.GameTable.Static;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.World.Message.Model.Chat;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatChannelMemberAddedHandler : IHandleMessages<ChatChannelMemberAddedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;
        private readonly IGameTableManager gameTableManager;

        public ChatChannelMemberAddedHandler(
            IPlayerManager playerManager,
            IGameTableManager gameTableManager)
        {
            this.playerManager    = playerManager;
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public Task Handle(ChatChannelMemberAddedMessage message)
        {
            ChatChannelEntry entry = gameTableManager.ChatChannel.GetEntry((ulong)message.ChatChannel.Type);
            if (entry == null)
                return Task.CompletedTask;

            if ((entry.Flags & ChatChannelEntryFlags.ServerSide) == 0)
                return Task.CompletedTask;

            IPlayer player = playerManager.GetPlayer(message.Member.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerChatJoin
            {
                Channel = new Channel
                {
                    ChatChannelId = message.ChatChannel.Type,
                    ChatId = message.ChatChannel.ChatId,
                },
                Name        = message.ChatChannel.Name,
                MemberCount = (uint)message.ChatChannel.Members.Count

            });

            return Task.CompletedTask;
        }
    }
}
