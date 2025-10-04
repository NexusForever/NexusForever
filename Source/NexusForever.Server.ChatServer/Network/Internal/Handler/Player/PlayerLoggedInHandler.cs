using NexusForever.Database.Chat;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.ChatServer.Character;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Player
{
    public class PlayerLoggedInHandler : IHandleMessages<PlayerLoggedInMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly CharacterManager _characterManager;
        private readonly ChatChannelManager _chatChannelManager;

        public PlayerLoggedInHandler(
            ChatContext context,
            OutboxMessagePublisher messagePublisher,
            CharacterManager characterManager,
            ChatChannelManager chatChannelManager)
        {
            _context            = context;
            _messagePublisher   = messagePublisher;
            _characterManager   = characterManager;
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        public async Task Handle(PlayerLoggedInMessage message)
        {
            Character.Character character = await _characterManager.GetCharacterRemoteAsync(message.Identity.ToChatIdentity());
            if (character == null)
                return;

            character.IsOnline = true;

            await foreach (var chatChannel in character.GetChatChannelsAsync())
            {
                ChatChannelMember member = chatChannel.GetMember(message.Identity.ToChatIdentity());
                if (member == null)
                    continue;

                await _messagePublisher.PublishAsync(new ChatChannelMemberAddedMessage
                {
                    ChatChannel = await chatChannel.ToInternalChatChannelAsync(),
                    Member      = await member.ToInternalChatChannelMemberAsync(),
                });
            }

            await foreach (var channel in _chatChannelManager.GetChatChannelsAsync(ChatChannelReferenceType.Realm, message.Identity.RealmId))
                await channel.AddMemberAsync(character);

            await _context.SaveChangesAsync();
        }
    }
}
