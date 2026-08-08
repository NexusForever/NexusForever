using NexusForever.Database.Chat;
using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Group
{
    public class GroupMemberRemovedHandler : IHandleMessages<GroupMemberRemovedMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _context;
        private readonly ChatChannelManager _chatChannelManager;

        public GroupMemberRemovedHandler(
            ChatContext context,
            ChatChannelManager chatChannelManager)
        {
            _context            = context;
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        public async Task Handle(GroupMemberRemovedMessage message)
        {
            ChatChannelType type = message.Group.Flags.HasFlag(GroupFlags.OpenWorld) ? ChatChannelType.Party : ChatChannelType.Instance;

            ChatChannel channel = await _chatChannelManager.GetChatChannelAsync(type, ChatChannelReferenceType.Group, message.Group.Id);
            if (channel == null)
                return;

            await channel.RemoveMemberAsync(message.RemovedMember.Identity.ToChatIdentity(), null);

            await _context.SaveChangesAsync();
        }
    }
}
