using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusForever.Database;
using NexusForever.Database.Chat.Model;
using NexusForever.Game.Static.Social;
using NexusForever.GameTable.Text.Filter;
using NexusForever.GameTable.Text.Static;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Server.ChatServer.Character;
using NexusForever.Server.ChatServer.Network.Internal.Handler;

namespace NexusForever.Server.ChatServer.Chat
{
    public class ChatChannel : IWrappedModel<ChatChannelModel>
    {
        public ChatChannelModel Model { get; private set; }

        public ulong ChatId
        {
            get => Model.ChatId;
            private set => Model.ChatId = value;
        }

        public ChatChannelType Type
        {
            get => Model.Type;
            set => Model.Type = value;
        }

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }

        public string Password
        {
            get => Model.Password;
            set => Model.Password = value;
        }

        public ChatChannelReferenceType? ReferenceType
        {
            get => Model.ReferenceType;
            set => Model.ReferenceType = value;
        }

        public ulong? ReferenceValue
        {
            get => Model.ReferenceValue;
            set => Model.ReferenceValue = value;
        }

        public Identity Owner
        {
            get => Model.Owner != null ? new Identity
            {
                Id      = Model.Owner.CharacterId,
                RealmId = Model.Owner.RealmId
            } : null;
            set
            {
                Model.Owner = new ChatChannelOwnerModel
                {
                    CharacterId = value.Id,
                    RealmId     = value.RealmId
                };
            }
        }

        private readonly Dictionary<Identity, ChatChannelMember> _members = [];

        #region Dependency Injection

        private readonly ILogger<ChatChannel> _log;
        private readonly IServiceProvider _serviceProvider;
        private readonly OutboxMessagePublisher _messagePublisher;
        private readonly CharacterManager _characterManager;
        private readonly ChatChannelManager _chatChannelManager;
        private readonly ITextFilterManager _textFilterManager;

        public ChatChannel(
            ILogger<ChatChannel> log,
            IServiceProvider serviceProvider,
            OutboxMessagePublisher messagePublisher,
            CharacterManager characterManager,
            ChatChannelManager chatChannelManager,
            ITextFilterManager textFilterManager)
        {
            _log                = log;
            _serviceProvider    = serviceProvider;
            _messagePublisher   = messagePublisher;
            _characterManager   = characterManager;
            _chatChannelManager = chatChannelManager;
            _textFilterManager  = textFilterManager;
        }

        #endregion

        /// <summary>
        /// Initialise a new chat channel with a default model.
        /// </summary>
        public void Initialise()
        {
            Model = new ChatChannelModel();
        }

        /// <summary>
        /// Initialise a chat channel with a model.
        /// </summary>
        /// <param name="model">Model to initialise the chat channel.</param>
        public void Initialise(ChatChannelModel model)
        {
            Model = model;

            foreach (ChatChannelMemberModel memberModel in model.Members)
            {
                var member = _serviceProvider.GetRequiredService<ChatChannelMember>();
                member.Initialise(memberModel);
                _members.Add(member.Identity, member);
            }
        }

        /// <summary>
        /// Get all members of the chat channel. 
        /// </summary>
        public IEnumerable<ChatChannelMember> GetMembers()
        {
            return _members.Values;
        }

        /// <summary>
        /// Get a member of the chat channel by identity.
        /// </summary>
        /// <param name="identity">Identity of the member to return.</param>
        public ChatChannelMember GetMember(Identity identity)
        {
            return _members.TryGetValue(identity, out ChatChannelMember member) ? member : null;
        }

        /// <summary>
        /// Get a member of the chat channel by identity name.
        /// </summary>
        /// <remarks>
        /// This is an asynchronous operation because it requires fetching the character entity.
        /// </remarks>
        /// <param name="identity">Identity name of the member to return.</param>
        public async Task<ChatChannelMember> GetMemberAsync(IdentityName identity)
        {
            foreach (ChatChannelMember member in _members.Values)
            {
                Character.Character character = await member.GetCharacterAsync();
                if (character == null)
                    continue;

                if (character.IdentityName == identity)
                    return member;
            }

            return null;
        }

        /// <summary>
        /// Add a character to the chat channel.
        /// </summary>
        /// <param name="character">Identity of the character to add to the chat channel.</param>
        public async Task AddMemberAsync(Identity character)
        {
            Character.Character characterEntity = await _characterManager.GetCharacterRemoteAsync(character);
            if (characterEntity == null)
                return;

            await AddMemberAsync(characterEntity);
        }

        /// <summary>
        /// Add a character to the chat channel.
        /// </summary>
        /// <param name="character">Character to add to the chat channel.</param>
        public async Task AddMemberAsync(Character.Character character)
        {
            if (GetMember(character.Identity) != null)
                return;

            var member = _serviceProvider.GetRequiredService<ChatChannelMember>();
            member.Initialise();
            member.Identity = character.Identity;
            member.ChatId = ChatId;

            if (Type == ChatChannelType.Custom && Owner == null)
                SetOwnerMember(member);

            _members.Add(character.Identity, member);
            Model.Members.Add(member.Model);

            character.AddChatChannel(this);

            await _messagePublisher.PublishAsync(new ChatChannelMemberAddedMessage
            {
                ChatChannel = await this.ToInternalChatChannelAsync(),
                Member      = await member.ToInternalChatChannelMemberAsync(),
            });

            _log.LogTrace("Added member {Identity} to chat channel {Type},{ChatId}.", character.Identity, Type, ChatId);
        }

        /// <summary>
        /// Remove a member from the chat channel.
        /// </summary>
        /// <param name="identity">Identity of the member to remove from the chat channel.</param>
        public async Task RemoveMemberAsync(Identity identity, ChatChannelLeaveReason? reason)
        {
            Character.Character character = await _characterManager.GetCharacterRemoteAsync(identity);
            if (character == null)
                return;

            await RemoveMemberAsync(character, reason);
        }

        /// <summary>
        /// Remove a member from the chat channel.
        /// </summary>
        /// <param name="character">Character to remove from the chat channel.</param>
        public async Task RemoveMemberAsync(Character.Character character, ChatChannelLeaveReason? reason)
        {
            ChatChannelMember member = GetMember(character.Identity);
            if (member == null)
                return;

            _members.Remove(character.Identity);
            Model.Members.Remove(member.Model);

            character.RemoveChatChannel(this);

            if (reason != null)
            {
                await _messagePublisher.PublishAsync(new ChatChannelMemberLeftMessage
                {
                    Channel = await this.ToInternalChatChannelAsync(),
                    Member  = await member.ToInternalChatChannelMemberAsync(),
                    Reason  = reason.Value
                });
            }

            if (Type == ChatChannelType.Custom && Owner == character.Identity)
            {
                ChatChannelMember newOwner = _members.Values.FirstOrDefault(m => (m.Flags & ChatChannelMemberFlags.Moderator) != 0)
                    ?? _members.Values.FirstOrDefault();

                if (newOwner == null)
                    await DisbandAsync();
                else
                    await SetOwnerMemberAsync(member, newOwner);
            }

            _log.LogTrace("Removed member {Identity} from chat channel {Type},{ChatId}.", character.Identity, Type, ChatId);
        }

        /// <summary>
        /// Disband the chat channel.
        /// </summary>
        public async Task DisbandAsync()
        {
            foreach (var member in _members)
                await RemoveMemberAsync(member.Key, null);

            _chatChannelManager.RemoveChatChannel(this);
        }

        /// <summary>
        /// Add a character to the chat channel.
        /// </summary>
        /// <param name="identity">Identity of the character to add to the chat channel.</param>
        /// <param name="password">Password for the channel.</param>
        public async Task<ChatResult> JoinAsync(Identity identity, string password)
        {
            Character.Character character = await _characterManager.GetCharacterRemoteAsync(identity);
            if (character == null)
                return ChatResult.InvalidCharacterName;

            return await JoinAsync(character, password);
        }

        /// <summary>
        /// Add a character to the chat channel.
        /// </summary>
        /// <param name="character">Character to add to the chat channel.</param>
        /// <param name="password">Password for the channel.</param>
        public async Task<ChatResult> JoinAsync(Character.Character character, string password)
        {
            if (Type != ChatChannelType.Custom)
                return ChatResult.DoesntExist;

            if (GetMember(character.Identity) != null)
                return ChatResult.AlreadyMember;

            if (!string.IsNullOrEmpty(Password) && Password != password)
                return ChatResult.BadPassword;

            await AddMemberAsync(character);
            return ChatResult.Ok;
        }

        /// <summary>
        /// Remove a member from the chat channel.
        /// </summary>
        /// <param name="identity">Identity of the member to remove from the chat channel.</param>
        public async Task<ChatResult> LeaveAsync(Identity identity)
        {
            if (Type != ChatChannelType.Custom)
                return ChatResult.DoesntExist;

            if (GetMember(identity) == null)
                return ChatResult.AlreadyMember;

            await RemoveMemberAsync(identity, ChatChannelLeaveReason.Leave);
            return ChatResult.Ok;
        }

        /// <summary>
        /// Kick a member from the chat channel.
        /// </summary>
        /// <param name="source">Identity of the member kicking the target from the chat channel.</param>
        /// <param name="target">Identity of the member being kicked from the chat channel.</param>
        public async Task<ChatResult> KickMemberAsync(Identity source, IdentityName target)
        {
            if (Type != ChatChannelType.Custom)
                return ChatResult.DoesntExist;

            ChatChannelMember sourceMember = GetMember(source);
            if (sourceMember == null)
                return ChatResult.NotMember;

            if ((sourceMember.Flags & (ChatChannelMemberFlags.Owner | ChatChannelMemberFlags.Moderator)) == 0)
                return ChatResult.NoPermissions;

            ChatChannelMember targetMember = await GetMemberAsync(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (sourceMember.Identity == targetMember.Identity)
                return ChatResult.InvalidCharacterName;

            await RemoveMemberAsync(targetMember.Identity, ChatChannelLeaveReason.Kicked);

            await PublishAction(sourceMember, targetMember, ChatChannelAction.Kicked);

            _log.LogTrace("Kicked member {Identity} from chat channel {Type},{ChatId}.", targetMember.Identity, Type, ChatId);

            return ChatResult.Ok;
        }

        /// <summary>
        /// List all members of the chat channel.
        /// </summary>
        /// <param name="identity">Identity of the member getting the list of members in the chat channel.</param>
        public async Task<ChatResult> ListMembersAsync(Identity identity)
        {
            ChatChannelMember member = GetMember(identity);
            if (member == null)
                return ChatResult.NotMember;

            await _messagePublisher.PublishAsync(new ChatChannelMembersMessage
            {
                ChatChannel = await this.ToInternalChatChannelAsync(),
                Member      = await member.ToInternalChatChannelMemberAsync()
            });

            return ChatResult.Ok;
        }

        /// <summary>
        /// Set the password for the chat channel.
        /// </summary>
        /// <param name="identity">Identity of the member setting the password for the chat channel.</param>
        /// <param name="password">New password to set for the chat channel.</param>
        public async Task<ChatResult> SetPasswordAsync(Identity identity, string password)
        {
            if (Type != ChatChannelType.Custom)
                return ChatResult.DoesntExist;

            ChatChannelMember member = GetMember(identity);
            if (member == null)
                return ChatResult.NotMember;

            if ((member.Flags & ChatChannelMemberFlags.Owner) == 0)
                return ChatResult.NoPermissions;

            if (!_textFilterManager.IsTextValid(password)
                || !_textFilterManager.IsTextValid(password, UserText.ChatCustomChannelPassword))
                return ChatResult.InvalidPasswordText;

            Password = string.IsNullOrEmpty(password) ? null : password;

            await PublishAction(member, null, string.IsNullOrEmpty(Password) ? ChatChannelAction.RemovePassword : ChatChannelAction.AddPassword);

            _log.LogTrace("Set password for chat channel {Type},{ChatId}.", Type, ChatId);

            return ChatResult.Ok;
        }

        /// <summary>
        /// Set the owner of the chat channel.
        /// </summary>
        /// <param name="source">Identity of the member setting the target as the owner of the chat channel.</param>
        /// <param name="target">Identity of the member to be set as the owner of the chat channel.</param>
        public async Task<ChatResult> SetOwnerMemberAsync(Identity source, IdentityName target)
        {
            if (Type != ChatChannelType.Custom)
                return ChatResult.DoesntExist;

            ChatChannelMember sourceMember = GetMember(source);
            if (sourceMember == null)
                return ChatResult.NotMember;

            if ((sourceMember.Flags & ChatChannelMemberFlags.Owner) == 0)
                return ChatResult.NoPermissions;

            ChatChannelMember targetMember = await GetMemberAsync(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (sourceMember.Identity == targetMember.Identity)
                return ChatResult.InvalidCharacterName;

            await SetOwnerMemberAsync(sourceMember, targetMember);

            return ChatResult.Ok;
        }

        private async Task SetOwnerMemberAsync(ChatChannelMember source, ChatChannelMember target)
        {
            source.Flags &= ~ChatChannelMemberFlags.Owner;
            SetOwnerMember(target);

            await PublishAction(source, target, ChatChannelAction.PassOwner);
        }

        private void SetOwnerMember(ChatChannelMember member)
        {
            Owner = member.Identity;
            member.Flags |= ChatChannelMemberFlags.Owner;

            _log.LogTrace("Set member {Identity} as owner of chat channel {Type},{ChatId}.", member.Identity, Type, ChatId);
        }

        /// <summary>
        /// Set a member as a moderator of the chat channel.
        /// </summary>
        /// <param name="source">Identity of the member setting the target as a moderator in the chat channel.</param>
        /// <param name="target">Identity of the member to have moderator set in the chat channel.</param>
        /// <param name="set">True of false value defining if the moderator is being applied or removed.</param>
        public async Task<ChatResult> SetModeratorMemberAsync(Identity source, IdentityName target, bool set)
        {
            if (Type != ChatChannelType.Custom)
                return ChatResult.DoesntExist;

            ChatChannelMember sourceMember = GetMember(source);
            if (sourceMember == null)
                return ChatResult.NotMember;

            if ((sourceMember.Flags & ChatChannelMemberFlags.Owner) == 0)
                return ChatResult.NoPermissions;

            ChatChannelMember targetMember = await GetMemberAsync(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (sourceMember.Identity == targetMember.Identity)
                return ChatResult.InvalidCharacterName;

            if (set)
                targetMember.Flags |= ChatChannelMemberFlags.Moderator;
            else
                targetMember.Flags &= ~ChatChannelMemberFlags.Moderator;

            await PublishAction(sourceMember, targetMember, set ? ChatChannelAction.AddModerator : ChatChannelAction.RemoveModerator);

            _log.LogTrace("{Action} moderator for member {Identity} in chat channel {Type},{ChatId}.",
                set ? "Added" : "Removed", targetMember.Identity, Type, ChatId);

            return ChatResult.Ok;
        }

        /// <summary>
        /// Set a member as muted in the chat channel.
        /// </summary>
        /// <param name="source">Identity of the member setting the target as muted in the chat channel.</param>
        /// <param name="target">Identity of the member to have a mute set in the chat channel.</param>
        /// <param name="set">True or false value defining if the mute i sbeing applied or removed.</param>
        public async Task<ChatResult> SetMuteMemberAsync(Identity source, IdentityName target, bool set)
        {
            if (Type != ChatChannelType.Custom)
                return ChatResult.DoesntExist;

            ChatChannelMember sourceMember = GetMember(source);
            if (sourceMember == null)
                return ChatResult.NotMember;

            if ((sourceMember.Flags & (ChatChannelMemberFlags.Owner | ChatChannelMemberFlags.Moderator )) == 0)
                return ChatResult.NoPermissions;

            ChatChannelMember targetMember = await GetMemberAsync(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (sourceMember.Identity == targetMember.Identity)
                return ChatResult.InvalidCharacterName;

            if (set)
                targetMember.Flags |= ChatChannelMemberFlags.Muted;
            else
                targetMember.Flags &= ~ChatChannelMemberFlags.Muted;

            await PublishAction(sourceMember, targetMember, set ? ChatChannelAction.Muted : ChatChannelAction.Unmuted);

            _log.LogTrace("{Action} mute for member {Identity} in chat channel {Type},{ChatId}.",
                set ? "Added" : "Removed", targetMember.Identity, Type, ChatId);

            return ChatResult.Ok;
        }

        private async Task PublishAction(ChatChannelMember source, ChatChannelMember target, ChatChannelAction action)
        {
            var actionMessage = new ChatChannelActionMessage
            {
                Channel = await this.ToInternalChatChannelAsync(),
                Source  = await source.ToInternalChatChannelMemberAsync(),    
                Action  = action
            };

            if (target != null)
                actionMessage.Target = await target.ToInternalChatChannelMemberAsync();

            await _messagePublisher.PublishAsync(actionMessage);
        }

        /// <summary>
        /// Send a text message to the chat channel.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="text"></param>
        /// <param name="chatMessageId"></param>
        public async Task<ChatResult> SendTextAsync(Identity identity, ChatChannelText text, ushort chatMessageId)
        {
            ChatChannelMember member = GetMember(identity);
            if (member == null)
                return ChatResult.NotMember;

            if ((member.Flags & ChatChannelMemberFlags.Muted) != 0)
                return ChatResult.NoSpeaking;

            if (!_textFilterManager.IsTextValid(text.Text)
                || !_textFilterManager.IsTextValid(text.Text, UserText.Chat))
                return ChatResult.InvalidMessageText;

            await _messagePublisher.PublishUrgentAsync(new ChatTextAcceptedMessage
            {
                Source        = identity.ToInternalIdentity(),
                ChatMessageId = chatMessageId
            });

            await _messagePublisher.PublishAsync(new ChatChannelTextMessage
            {
                ChatChannel = await this.ToInternalChatChannelAsync(),
                Sender      = await member.ToInternalChatChannelMemberAsync(),
                Text        = text.ToInternal()
            });

            return ChatResult.Ok;
        }
    }
}
