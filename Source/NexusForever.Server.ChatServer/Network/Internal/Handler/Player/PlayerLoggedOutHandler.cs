using NexusForever.Database.Chat;
using NexusForever.Game.Static.Chat;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.ChatServer.Character;
using NexusForever.Server.ChatServer.Chat;
using Rebus.Handlers;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler.Player
{
    public class PlayerLoggedOutHandler : IHandleMessages<PlayerLoggedOutMessage>
    {
        #region Dependency Injection

        private readonly ChatContext _context;
        private readonly CharacterManager _characterManager;
        private readonly ChatChannelManager _chatChannelManager;

        public PlayerLoggedOutHandler(
            ChatContext context,
            CharacterManager characterManager,
            ChatChannelManager chatChannelManager)
        {
            _context            = context;
            _characterManager   = characterManager;
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        public async Task Handle(PlayerLoggedOutMessage message)
        {
            Character.Character character = await _characterManager.GetCharacterAsync(message.Identity.ToChatIdentity());
            if (character == null)
                return;

            character.IsOnline = false;

            await foreach (ChatChannel channel in _chatChannelManager.GetChatChannelsAsync(ChatChannelReferenceType.Realm, character.Identity.RealmId))
                await channel.RemoveMemberAsync(character.Identity, null);

            await _context.SaveChangesAsync();
        }
    }
}
