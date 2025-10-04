using NexusForever.Database.Chat;
using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.ChatServer.Character;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Group
{
    public class GroupMemberAddedHandler : IHandleMessages<GroupMemberAddedMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _context;
        private readonly ChatChannelManager _chatChannelManager;
        private readonly CharacterManager _characterManager;

        public GroupMemberAddedHandler(
            ChatContext context,
            ChatChannelManager chatChannelManager,
            CharacterManager characterManager)
        {
            _context            = context;
            _chatChannelManager = chatChannelManager;
            _characterManager   = characterManager;
        }

        #endregion

        public async Task Handle(GroupMemberAddedMessage message)
        {
            ChatChannelType type = message.Group.Flags.HasFlag(GroupFlags.OpenWorld) ? ChatChannelType.Party : ChatChannelType.Instance;

            ChatChannel channel = await _chatChannelManager.GetChatChannelAsync(type, ChatChannelReferenceType.Group, message.Group.Id);
            if (channel == null)
            {
                channel = _chatChannelManager.CreateChatChannel(type, name: null, password: null);
                channel.ReferenceType  = ChatChannelReferenceType.Group;
                channel.ReferenceValue = message.Group.Id;
                await _context.SaveChangesAsync();
            }

            Character.Character character = await _characterManager.GetCharacterAsync(message.AddedMember.Identity.ToChatIdentity());
            if (character == null)
            {
                // if we don't have the character local, no need to get the character from the API since the group message contains all of the information we need to construct
                var groupCharacter = message.AddedMember.ToDatabaseCharacter();
                character = _characterManager.AddCharacter(groupCharacter);
            }

            await channel.AddMemberAsync(character);

            await _context.SaveChangesAsync();
        }
    }
}
