using System.Linq;
using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Chat;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.GameTable.Static;
using NexusForever.Network.Internal.Message.Chat;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatChannelTextHandler : IHandleMessages<ChatChannelTextMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;
        private readonly IChatFormatManager chatFormatManager;
        private readonly IGameTableManager gameTableManager;

        public ChatChannelTextHandler(
            IPlayerManager playerManager,
            IChatFormatManager chatFormatManager,
            IGameTableManager gameTableManager)
        {
            this.playerManager     = playerManager;
            this.chatFormatManager = chatFormatManager;
            this.gameTableManager  = gameTableManager;
        }

        #endregion

        public async Task Handle(ChatChannelTextMessage message)
        {
            ChatChannelEntry entry = gameTableManager.ChatChannel.GetEntry((ulong)message.ChatChannel.Type);
            if (entry == null)
                return;

            foreach (var member in message.ChatChannel.Members)
            {
                if (member.Identity == message.Sender.Identity)
                    continue;

                IPlayer player = playerManager.GetPlayer(member.Identity.ToGameIdentity());
                if (player == null)
                    continue;

                var builder = new ChatMessageBuilder();
                builder.Type = message.ChatChannel.Type;

                if ((entry.Flags & ChatChannelEntryFlags.ServerSide) != 0)
                    builder.ChatId = message.ChatChannel.ChatId;
                
                builder.Text    = message.Text.Text;
                builder.Formats = chatFormatManager.ToNetwork(message.Text.Format).ToList();

                builder.FromName = message.Sender.Character.IdentityName.Name;
                if (member.Identity.RealmId != message.Sender.Identity.RealmId)
                    builder.FromRealm = message.Sender.Character.IdentityName.RealmName;

                if ((entry.Flags & ChatChannelEntryFlags.ShowChatBubble) != 0)
                {
                    uint? guid = await player.SynchroniseAsync<uint?>(() =>
                    {
                        IPlayer sender = player.GetVisiblePlayer(message.Sender.Identity.ToGameIdentity());
                        if (sender != null)
                            return sender.Guid;

                        return null;
                    });

                    if (guid != null)
                        builder.Guid = guid.Value;
                }

                player.Session.EnqueueMessageEncrypted(builder.Build());
            }
        }
    }
}
