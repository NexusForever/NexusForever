using System.Collections;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Character;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Social;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Game.Text.Filter;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NLog;

namespace NexusForever.Game.Social
{
    public class ChatChannel : IChatChannel
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [Flags]
        public enum ChatChannelSaveMask
        {
            None     = 0x00,
            Create   = 0x01,
            Delete   = 0x02,
            Password = 0x04
        }

        public ChatChannelType Type { get; }
        public ulong Id { get; }
        public string Name { get; set; }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                saveMask |= ChatChannelSaveMask.Password;
            }
        }
        private string password;

        private readonly Dictionary<ulong, IChatChannelMember> members = new();

        private ChatChannelSaveMask saveMask;

        /// <summary>
        /// Returns if <see cref="IChatChannel"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & ChatChannelSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="IChatChannel"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & ChatChannelSaveMask.Delete) != 0;

        /// <summary>
        /// Enqueue <see cref="IChatChannel"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= ChatChannelSaveMask.Delete;
            else
                saveMask &= ~ChatChannelSaveMask.Delete;
        }

        public bool IsGuildChannel()
        {
            return Type is ChatChannelType.Guild or ChatChannelType.GuildOfficer or ChatChannelType.Community or
                ChatChannelType.Society or ChatChannelType.WarParty or ChatChannelType.WarPartyOfficer or
                ChatChannelType.Nexus or ChatChannelType.Trade;
        }

        /// <summary>
        /// Create a new <see cref="IChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
        /// </summary>
        public ChatChannel(ChatChannelType type, ulong id, string name, string password = null)
        {
            Type     = type;
            Id       = id;
            Name     = name;
            Password = password;

            saveMask = ChatChannelSaveMask.Create;
        }

        /// <summary>
        /// Create a new <see cref="IChatChannel"/> from an existing database model.
        /// </summary>
        public ChatChannel(ChatChannelModel model)
        {
            Type     = (ChatChannelType)model.Type;
            Id       = model.Id;
            Name     = model.Name;
            Password = model.Password;

            foreach (ChatChannelMemberModel memberModel in model.Members)
            {
                var member = new ChatChannelMember(memberModel);
                members.Add(member.CharacterId, member);
            }

            saveMask = ChatChannelSaveMask.None;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != ChatChannelSaveMask.None)
            {
                var model = new ChatChannelModel
                {
                    Type = (byte)Type,
                    Id   = Id
                };

                if ((saveMask & ChatChannelSaveMask.Create) != 0)
                {
                    model.Name     = Name;
                    model.Password = password;
                    context.Add(model);
                }
                else if ((saveMask & ChatChannelSaveMask.Delete) != 0)
                    context.Remove(model);
                else
                {
                    EntityEntry<ChatChannelModel> entity = context.Attach(model);
                    if ((saveMask & ChatChannelSaveMask.Password) != 0)
                    {
                        model.Password = password;
                        entity.Property(p => p.Password).IsModified = true;
                    }
                }

                saveMask = ChatChannelSaveMask.None;
            }

            foreach (IChatChannelMember member in members.Values)
                member.Save(context);
        }

        /// <summary>
        /// Returns if supplied character is a member of the <see cref="IChatChannel"/>.
        /// </summary>
        public bool IsMember(ulong characterId)
        {
            return members.ContainsKey(characterId);
        }

        /// <summary>
        /// Invoked when a <see cref="IPlayer"/> comes online.
        /// </summary>
        public void OnMemberLogin(IPlayer player)
        {
            IChatChannelMember member = GetMember(player.CharacterId);
            if (member == null)
                return;

            member.IsOnline = true;

            player.Session.EnqueueMessageEncrypted(new ServerChatJoin
            {
                Channel     = new Channel
                {
                    Type   = Type,
                    ChatId = Id
                },
                Name        = Name,
                MemberCount = (uint)members.Count
            });
        }

        /// <summary>
        /// Invoked when a <see cref="IPlayer"/> goes offline.
        /// </summary>
        public void OnMemberLogout(IPlayer player)
        {
            IChatChannelMember member = GetMember(player.CharacterId);
            if (member == null)
                return;

            member.IsOnline = false;
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can join the <see cref="IChatChannel"/> with supplied password.
        /// </summary>
        public ChatResult CanJoin(IPlayer player, string password)
        {
            IChatChannelMember member = GetMember(player.CharacterId);
            if (member != null)
                return ChatResult.AlreadyMember;

            if (!string.IsNullOrEmpty(Password) && Password != password)
                return ChatResult.BadPassword;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Add a new <see cref="IPlayer"/> to the <see cref="IChatChannel"/> with supplied password.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoin(IPlayer, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Join(IPlayer player, string password)
        {
            if (!string.IsNullOrEmpty(Password) && Password != password)
                return;

            Join(player.CharacterId);

            player.Session.EnqueueMessageEncrypted(new ServerChatJoin
            {
                Channel     = new Channel
                {
                    Type   = Type,
                    ChatId = Id
                },
                Name        = Name,
                MemberCount = (uint)members.Count
            });
        }

        /// <summary>
        /// Add a new character to <see cref="IChatChannel"/>.
        /// </summary>
        public void Join(ulong characterId)
        {
            // make member owner if they are the first character to channel
            var flags = ChatChannelMemberFlags.None;
            if (members.Count == 0)
                flags |= ChatChannelMemberFlags.Owner;

            if (members.TryGetValue(characterId, out IChatChannelMember member))
            {
                if (!member.PendingDelete)
                    throw new InvalidOperationException($"Member {characterId} for chat channel {Type},{Id} already exists!");

                member.EnqueueDelete(false);
                member.Flags = flags;
            }
            else
            {
                member = new ChatChannelMember(Id, characterId, flags);
                members.Add(characterId, member);
            }

            member.IsOnline = true;

            log.Trace($"Added member {characterId} to chat channel {Type},{Id}.");
        }

        /// <summary>
        /// Remove an existing <see cref="IPlayer"/> from <see cref="IChatChannel"/> with supplied <see cref="ChatChannelLeaveReason"/>.
        /// </summary>
        public void Leave(IPlayer player, ChatChannelLeaveReason reason)
        {
            Leave(player.CharacterId);

            player.Session.EnqueueMessageEncrypted(new ServerChatLeave
            {
                Channel = new Channel
                {
                    Type   = Type,
                    ChatId = Id
                },
                Leave   = reason
            });
        }

        /// <summary>
        /// Remove an existing character from <see cref="IChatChannel"/>.
        /// </summary>
        public void Leave(ulong characterId)
        {
            if (!members.TryGetValue(characterId, out IChatChannelMember member))
                return;

            member.IsOnline = false;

            if (member.PendingCreate)
                members.Remove(characterId);
            else
                member.EnqueueDelete(true);

            GlobalChatManager.Instance.UntrackCharacterChatChannel(characterId, Type, Id);

            log.Trace($"Removed member {characterId} from chat channel {Type},{Id}.");

            // reassign owner
            // try and assign to a moderator first, otherwise a member
            if (member.HasFlag(ChatChannelMemberFlags.Owner)
                && !IsGuildChannel())
            {
                IChatChannelMember newOwner = members
                    .FirstOrDefault(m => m.Value.HasFlag(ChatChannelMemberFlags.Moderator))
                    .Value ?? members.FirstOrDefault().Value;

                // no suitable new owner, remove chat channel
                if (newOwner == null)
                    EnqueueDelete(true);
                else
                    newOwner.SetFlag(ChatChannelMemberFlags.Owner);
            }
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can kick target from <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanKick(IPlayer player, string target)
        {
            IChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return ChatResult.NotMember;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner | ChatChannelMemberFlags.Moderator))
                return ChatResult.NoPermissions;

            IChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (invokeMember.CharacterId == targetMember.CharacterId)
                return ChatResult.InvalidCharacterName;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Kick target from <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanKick(IPlayer, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Kick(IPlayer player, string target)
        {
            ICharacter targetCharacter = CharacterManager.Instance.GetCharacter(target);
            if (targetCharacter == null)
                return;

            IPlayer targetPlayer = PlayerManager.Instance.GetPlayer(targetCharacter.CharacterId);
            if (targetPlayer == null)
                return;

            // if the player is online handle through the local manager otherwise directly in the channel
            if (targetPlayer != null)
                targetPlayer.ChatManager.Leave(Id, ChatChannelLeaveReason.Kicked);
            else
                Leave(targetCharacter.CharacterId);

            Broadcast(new ServerChatAction
            {
                Channel     = new Channel
                {
                    Type   = Type,
                    ChatId = Id
                },
                Action      = ChatChannelAction.Kicked,
                NameActor   = player.Name,
                NameActedOn = target
            });
        }

        /// <summary>
        /// List all members of <see cref="IChatChannel"/>.
        /// </summary>
        public void ListMembers(IPlayer player)
        {
            IChatChannelMember member = GetMember(player.CharacterId);
            if (member == null)
                return;

            player.Session.EnqueueMessageEncrypted(new ServerChatList
            {
                Type      = Type,
                ChannelId = Id,
                Names     = members.Values
                    .Where(m => !m.PendingDelete && m.IsOnline)
                    .Select(m => CharacterManager.Instance.GetCharacter(m.CharacterId).Name)
                    .ToList(),
                Flags     = members.Values
                    .Where(m => !m.PendingDelete)
                    .Select(m => m.Flags)
                    .ToList(),
                More      = false
            });
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can set password of <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanSetPassword(IPlayer player, string password)
        {
            IChatChannelMember member = GetMember(player.CharacterId);
            if (member == null)
                return ChatResult.NotMember;

            if (!member.HasFlag(ChatChannelMemberFlags.Owner))
                return ChatResult.NoPermissions;

            if (!TextFilterManager.Instance.IsTextValid(password)
                || !TextFilterManager.Instance.IsTextValid(password, UserText.ChatCustomChannelPassword))
                return ChatResult.InvalidPasswordText;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Set password for <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanSetPassword(IPlayer, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void SetPassword(IPlayer player, string password)
        {
            IChatChannelMember member = GetMember(player.CharacterId);
            if (member == null)
                return;

            if (!member.HasFlag(ChatChannelMemberFlags.Owner))
                return;

            if (!TextFilterManager.Instance.IsTextValid(password)
                || !TextFilterManager.Instance.IsTextValid(password, UserText.ChatCustomChannelPassword))
                return;

            Password = password;

            Broadcast(new ServerChatAction
            {
                Channel   = new Channel
                {
                    Type   = Type,
                    ChatId = Id
                },
                Action    = string.IsNullOrEmpty(Password) ? ChatChannelAction.RemovePassword : ChatChannelAction.AddPassword,
                NameActor = player.Name
            });

            log.Trace($"Password updated in chat channel {Type},{Id}.");
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can make target owner of <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanPassOwner(IPlayer player, string target)
        {
            IChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return ChatResult.NotMember;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner))
                return ChatResult.NoPermissions;

            IChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (invokeMember.CharacterId == targetMember.CharacterId)
                return ChatResult.InvalidCharacterName;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Make target owner of <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanPassOwner(IPlayer, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void PassOwner(IPlayer player, string target)
        {
            IChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner))
                return;

            IChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return;

            invokeMember.RemoveFlag(ChatChannelMemberFlags.Owner);
            targetMember.SetFlag(ChatChannelMemberFlags.Owner);

            Broadcast(new ServerChatAction
            {
                Channel     = new Channel
                {
                    Type   = Type,
                    ChatId = Id
                },
                Action      = ChatChannelAction.PassOwner,
                NameActor   = player.Name,
                NameActedOn = target
            });

            log.Trace($"Member {targetMember.CharacterId} was made owner in chat channel {Type},{Id}.");
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can make target a moderator in <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanMakeModerator(IPlayer player, string target)
        {
            IChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return ChatResult.NotMember;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner))
                return ChatResult.NoPermissions;

            IChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (invokeMember.CharacterId == targetMember.CharacterId)
                return ChatResult.InvalidCharacterName;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Make target moderator in <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMakeModerator(IPlayer, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void MakeModerator(IPlayer player, string target, bool status)
        {
            IChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner))
                return;

            IChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return;

            if (status)
                targetMember.SetFlag(ChatChannelMemberFlags.Moderator);
            else
                targetMember.RemoveFlag(ChatChannelMemberFlags.Moderator);

            Broadcast(new ServerChatAction
            {
                Channel     = new Channel
                {
                    Type   = Type,
                    ChatId = Id
                },
                Action      = status ? ChatChannelAction.AddModerator : ChatChannelAction.RemoveModerator,
                NameActor   = player.Name,
                NameActedOn = target
            });

            log.Trace($"Member {targetMember.CharacterId} was made {(status ? "moderator": "player")} in chat channel {Type},{Id}.");
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can mute target in <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanMuteMember(IPlayer player, string target)
        {
            IChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return ChatResult.NotMember;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner | ChatChannelMemberFlags.Moderator))
                return ChatResult.NoPermissions;

            IChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (invokeMember.CharacterId == targetMember.CharacterId)
                return ChatResult.InvalidCharacterName;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Mute or unmute target in <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMuteMember(IPlayer, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void MuteMember(IPlayer player, string target, bool status)
        {
            IChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)    
                return;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner | ChatChannelMemberFlags.Moderator))
                return;

            IChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return;

            if (status)
                targetMember.SetFlag(ChatChannelMemberFlags.Muted);
            else
                targetMember.RemoveFlag(ChatChannelMemberFlags.Muted);

            Broadcast(new ServerChatAction
            {
                Channel     = new Channel
                {
                    Type   = Type,
                    ChatId = Id
                },
                Action      = status ? ChatChannelAction.Muted : ChatChannelAction.Unmuted,
                NameActor   = player.Name,
                NameActedOn = target
            });

            log.Trace($"Member {targetMember.CharacterId} was {(status ? "muted" : "unmuted")} in chat channel {Type},{Id}.");
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can broadcast in the <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanBroadcast(IPlayer player, string text)
        {
            IChatChannelMember member = GetMember(player.CharacterId);
            if (member == null)
                return ChatResult.NotMember;

            if (member.HasFlag(ChatChannelMemberFlags.Muted))
                return ChatResult.NoSpeaking;

            if (!TextFilterManager.Instance.IsTextValid(text)
                || !TextFilterManager.Instance.IsTextValid(text, UserText.Chat))
                return ChatResult.InvalidMessageText;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all members in the <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanBroadcast(IPlayer, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Broadcast(IWritable message, IPlayer except = null)
        {
            foreach (IChatChannelMember member in members.Values
                .Where(m => m.IsOnline && !m.PendingDelete))
            {
                IPlayer player = PlayerManager.Instance.GetPlayer(member.CharacterId);
                if (player != except)
                {
                    player?.Session?.EnqueueMessageEncrypted(message);
                }
            }
        }

        private IChatChannelMember GetMember(ulong characterId)
        {
            if (members.TryGetValue(characterId, out IChatChannelMember member)
                && !member.PendingDelete)
                return member;

            return null;
        }

        private IChatChannelMember GetMember(string name)
        {
            ulong? characterId = CharacterManager.Instance.GetCharacterIdByName(name);
            if (characterId == null)
                return null;

            return GetMember(characterId.Value);
        }

        public IEnumerator<IChatChannelMember> GetEnumerator()
        {
            return members.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
