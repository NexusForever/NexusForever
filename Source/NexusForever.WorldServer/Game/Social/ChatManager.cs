using System.Collections.Generic;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Game.TextFilter;
using NexusForever.WorldServer.Game.TextFilter.Static;

namespace NexusForever.WorldServer.Game.Social
{
    public class ChatManager
    {
        private readonly Player owner;
        private readonly Dictionary<ulong, ChatChannel> channels = new();

        /// <summary>
        /// Create a new <see cref="ChatManager"/>.
        /// </summary>
        public ChatManager(Player player)
        {
            owner = player;
            foreach (ChatChannel channel in GlobalChatManager.Instance.GetCharacterChatChannels(ChatChannelType.Custom, owner.CharacterId))
                channels.Add(channel.Id, channel);
        }

        /// <summary>
        /// Invoked when a <see cref="Player"/> comes online.
        /// </summary>
        public void OnLogin()
        {
            foreach (ChatChannel channel in channels.Values)
                channel.OnMemberLogin(owner);
        }

        /// <summary>
        /// Invoked when a <see cref="Player"/> goes offline.
        /// </summary>
        public void OnLogout()
        {
            foreach (ChatChannel channel in channels.Values)
                channel.OnMemberLogout(owner);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can join the <see cref="ChatChannel"/> with supplied password.
        /// </summary>
        public ChatResult CanJoin(string name, string password)
        {
            if (channels.Count >= 20)
                return ChatResult.TooManyCustomChannels;

            if (!TextFilterManager.Instance.IsTextValid(name)
                || !TextFilterManager.Instance.IsTextValid(name, UserText.ChatCustomChannelName))
                return ChatResult.InvalidPasswordText;

            ChatChannel channel = GlobalChatManager.Instance.GetChatChannel(ChatChannelType.Custom, name);
            if (channel == null)
            {
                // new channel
                if (!string.IsNullOrEmpty(password)
                    && (!TextFilterManager.Instance.IsTextValid(password)
                    || !TextFilterManager.Instance.IsTextValid(password, UserText.ChatCustomChannelPassword)))
                    return ChatResult.InvalidPasswordText;

                return ChatResult.Ok;
            }

            // existing channel
            return channel.CanJoin(owner, password);
        }

        /// <summary>
        /// Add a new <see cref="Player"/> to the <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoin(string, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Join(string name, string password)
        {
            ChatChannel channel = GlobalChatManager.Instance.GetChatChannel(ChatChannelType.Custom, name);
            channel ??= GlobalChatManager.Instance.CreateChatChannel(ChatChannelType.Custom, name, password);
            channel.Join(owner, password);
            
            channels.Add(channel.Id, channel);

            GlobalChatManager.Instance.TrackCharacterChatChannel(owner.CharacterId, ChatChannelType.Custom, channel.Id);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can leave <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanLeave(ulong chatId)
        {
            if (!channels.ContainsKey(chatId))
                return ChatResult.NotMember;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Remove an existing <see cref="Player"/> from <see cref="ChatChannel"/> with supplied <see cref="Player"/>.
        /// </summary>
        public void Leave(ulong chatId, ChatChannelLeaveReason reason)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return;

            channel.Leave(owner, reason);
            channels.Remove(chatId);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can kick target from <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanKick(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanKick(owner, target);
        }

        /// <summary>
        /// Kick target from <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanKick(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Kick(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return;

            channel.Kick(owner, target);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can list all members of <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanListMembers(ulong chatId)
        {
            if (!channels.ContainsKey(chatId))
                return ChatResult.NotMember;

            return ChatResult.Ok;
        }

        /// <summary>
        /// List all members of <see cref="ChatChannel"/>.
        /// </summary>
        public void ListMembers(ulong chatId)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return;

            channel.ListMembers(owner);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can set password of <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanSetPassword(ulong chatId, string password)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanSetPassword(owner, password);
        }

        /// <summary>
        /// Set password for <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanSetPassword(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void SetPassword(ulong chatId, string password)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return;

            channel.SetPassword(owner, password);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can make target owner of <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanPassOwner(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanPassOwner(owner, target);
        }

        /// <summary>
        /// Make target owner of <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanPassOwner(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void PassOwner(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return;

            channel.PassOwner(owner, target);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can make target a moderator in <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanMakeModerator(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanMakeModerator(owner, target);
        }

        /// <summary>
        /// Make target moderator in <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMakeModerator(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void MakeModerator(ulong chatId, string target, bool status)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return;

            channel.MakeModerator(owner, target, status);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can mute target in <see cref="ChatChannel"/>.
        /// </summary>
        public ChatResult CanMute(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanMuteMember(owner, target);
        }

        /// <summary>
        /// Mute or unmute target in <see cref="ChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMute(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Mute(ulong chatId, string target, bool status)
        {
            if (!channels.TryGetValue(chatId, out ChatChannel channel))
                return;

            channel.MuteMember(owner, target, status);
        }
    }
}
