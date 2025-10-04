using NexusForever.Database.Chat;
using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Group
{
    public class GroupDisbandedHandler : IHandleMessages<GroupDisbandedMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _context;
        private readonly ChatChannelManager _chatChannelManager;

        public GroupDisbandedHandler(
            ChatContext context,
            ChatChannelManager chatChannelManager)
        {
            _context            = context;
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        public async Task Handle(GroupDisbandedMessage message)
        {
            ChatChannelType type = message.Group.Flags.HasFlag(GroupFlags.OpenWorld) ? ChatChannelType.Party : ChatChannelType.Instance;

            ChatChannel channel = await _chatChannelManager.GetChatChannelAsync(type, ChatChannelReferenceType.Group, message.Group.Id);
            if (channel == null)
                return;

            await channel.DisbandAsync();

            await _context.SaveChangesAsync();
        }
    }
}
