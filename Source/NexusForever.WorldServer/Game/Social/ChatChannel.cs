using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Game.TextFilter;
using NexusForever.WorldServer.Game.TextFilter.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Game.Social
{
    public class ChatChannel : IEnumerable<ChatChannelMember>, ISaveCharacter
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
        public string Name { get; }

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

        private readonly Dictionary<ulong, ChatChannelMember> members = new();

        private ChatChannelSaveMask saveMask;

        /// <summary>
        /// Returns if <see cref="ChatChannel"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & ChatChannelSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="ChatChannel"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & ChatChannelSaveMask.Delete) != 0;

        /// <summary>
        /// Enqueue <see cref="ChatChannel"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= ChatChannelSaveMask.Delete;
            else
                saveMask &= ~ChatChannelSaveMask.Delete;
        }

        /// <summary>
        /// Create a new <see cref="ChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
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
        /// Create a new <see cref="ChatChannel"/> from an existing database model.
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

            foreach (ChatChannelMember member in members.Values)
                member.Save(context);
        }

        /// <summary>
        /// Returns if supplied character is a member of the <see cref="ChatChannel"/>.
        /// </summary>
        public bool IsMember(ulong characterId)
        {
            return members.ContainsKey(characterId);
        }

        /// <summary>
        /// Invoked when a <see cref="Player"/> comes online.
        /// </summary>
        public void OnMemberLogin(Player player)
        {
            ChatChannelMember member = GetMember(player.CharacterId);
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
        /// Invoked when a <see cref="Player"/> goes offline.
        /// </summary>
        public void OnMemberLogout(Player player)
        {
            ChatChannelMember member = GetMember(player.CharacterId);
            if (member == null)
                return;

            member.IsOnline = false;
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can join the <see cref="ChatChannel"/> with supplied password.
        /// </summary>
        public ChatResult CanJoin(Player player, string password)
        {
            ChatChannelMember member = GetMember(player.CharacterId);
            if (member != null)
                return ChatResult.AlreadyMember;

            if (!string.IsNullOrEmpty(Password) && Password != password)
                return ChatResult.BadPassword;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Add a new <see cref="Player"/> to the <see cref="ChatChannel"/> with supplied password.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoin(Player, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Join(Player player, string password)
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
        /// Add a new character to <see cref="ChatChannel"/>.
        /// </summary>
        public void Join(ulong characterId)
        {
            // make member owner if they are the first character to channel
            var flags = ChatChannelMemberFlags.None;
            if (members.Count == 0)
                flags |= ChatChannelMemberFlags.Owner;

            if (members.TryGetValue(characterId, out ChatChannelMember member))
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
        /// Remove an existing <see cref="Player"/> from <see cref="ChatChannel"/> with supplied <see cref="Player"/>.
        /// </summary>
        public void Leave(Player player, ChatChannelLeaveReason reason)
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
        /// Remove an existing character from <see cref="ChatChannel"/>.
        /// </summary>
        public void Leave(ulong characterId)
        {
            if (!members.TryGetValue(characterId, out ChatChannelMember member))
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
            if (member.HasFlag(ChatChannelMemberFlags.Owner))
            {
                ChatChannelMember newOwner = members
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
        /// Returns if <see cref="Player"/> can kick target from <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanKick(Player player, string target)
        {
            ChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return ChatResult.NotMember;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner | ChatChannelMemberFlags.Moderator))
                return ChatResult.NoPermissions;

            ChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (invokeMember.CharacterId == targetMember.CharacterId)
                return ChatResult.InvalidCharacterName;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Kick target from <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanKick(Player, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Kick(Player player, string target)
        {
            ICharacter targetCharacter = CharacterManager.Instance.GetCharacterInfo(target);
            if (targetCharacter == null)
                return;

            // if the player is online handle through the local manager otherwise directly in the channel
            if (targetCharacter is Player targetPlayer)
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
        /// List all members of <see cref="ChatChannel"/>.
        /// </summary>
        public void ListMembers(Player player)
        {
            ChatChannelMember member = GetMember(player.CharacterId);
            if (member == null)
                return;

            player.Session.EnqueueMessageEncrypted(new ServerChatList
            {
                Type      = Type,
                ChannelId = Id,
                Names     = members.Values
                    .Where(m => !m.PendingDelete)
                    .Select(m => CharacterManager.Instance.GetCharacterInfo(m.CharacterId).Name)
                    .ToList(),
                Flags     = members.Values
                    .Where(m => !m.PendingDelete)
                    .Select(m => m.Flags)
                    .ToList(),
                More      = false
            });
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can set password of <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanSetPassword(Player player, string password)
        {
            ChatChannelMember member = GetMember(player.CharacterId);
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
        /// Set password for <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanSetPassword(Player, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void SetPassword(Player player, string password)
        {
            ChatChannelMember member = GetMember(player.CharacterId);
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
        /// Returns if <see cref="Player"/> can make target owner of <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanPassOwner(Player player, string target)
        {
            ChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return ChatResult.NotMember;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner))
                return ChatResult.NoPermissions;

            ChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (invokeMember.CharacterId == targetMember.CharacterId)
                return ChatResult.InvalidCharacterName;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Make target owner of <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanPassOwner(Player, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void PassOwner(Player player, string target)
        {
            ChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner))
                return;

            ChatChannelMember targetMember = GetMember(target);
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
        /// Returns if <see cref="Player"/> can make target a moderator in <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanMakeModerator(Player player, string target)
        {
            ChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return ChatResult.NotMember;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner))
                return ChatResult.NoPermissions;

            ChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (invokeMember.CharacterId == targetMember.CharacterId)
                return ChatResult.InvalidCharacterName;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Make target moderator in <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMakeModerator(Player, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void MakeModerator(Player player, string target, bool status)
        {
            ChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner))
                return;

            ChatChannelMember targetMember = GetMember(target);
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
        /// Returns if <see cref="Player"/> can mute target in <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanMuteMember(Player player, string target)
        {
            ChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)
                return ChatResult.NotMember;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner | ChatChannelMemberFlags.Moderator))
                return ChatResult.NoPermissions;

            ChatChannelMember targetMember = GetMember(target);
            if (targetMember == null)
                return ChatResult.NotMember;

            if (invokeMember.CharacterId == targetMember.CharacterId)
                return ChatResult.InvalidCharacterName;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Mute or unmute target in <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMuteMember(Player, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void MuteMember(Player player, string target, bool status)
        {
            ChatChannelMember invokeMember = GetMember(player.CharacterId);
            if (invokeMember == null)    
                return;

            if (!invokeMember.HasFlag(ChatChannelMemberFlags.Owner | ChatChannelMemberFlags.Moderator))
                return;

            ChatChannelMember targetMember = GetMember(target);
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
        /// Returns if <see cref="Player"/> can broadcast in the <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanBroadcast(Player player, string text)
        {
            ChatChannelMember member = GetMember(player.CharacterId);
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
        /// Broadcast <see cref="IWritable"/> to all members in the <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanBroadcast(Player, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Broadcast(IWritable message)
        {
            foreach (ChatChannelMember member in members.Values
                .Where(m => m.IsOnline && !m.PendingDelete))
            {
                Player player = CharacterManager.Instance.GetPlayer(member.CharacterId);
                player?.Session?.EnqueueMessageEncrypted(message);
            }
        }

        private ChatChannelMember GetMember(ulong characterId)
        {
            if (members.TryGetValue(characterId, out ChatChannelMember member)
                && !member.PendingDelete)
                return member;

            return null;
        }

        private ChatChannelMember GetMember(string name)
        {
            ulong? characterId = CharacterManager.Instance.GetCharacterIdByName(name);
            if (characterId == null)
                return null;

            return GetMember(characterId.Value);
        }

        public IEnumerator<ChatChannelMember> GetEnumerator()
        {
            return members.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
