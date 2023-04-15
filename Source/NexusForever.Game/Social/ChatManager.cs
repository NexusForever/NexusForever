using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Game.Text.Filter;

namespace NexusForever.Game.Social
{
    public class ChatManager : IChatManager
    {
        private readonly IPlayer owner;
        private readonly Dictionary<ulong, IChatChannel> channels = new();

        /// <summary>
        /// Create a new <see cref="IChatManager"/>.
        /// </summary>
        public ChatManager(IPlayer player)
        {
            owner = player;
            foreach (IChatChannel channel in GlobalChatManager.Instance.GetCharacterChatChannels(ChatChannelType.Custom, owner.CharacterId))
                channels.Add(channel.Id, channel);
        }

        /// <summary>
        /// Invoked when a <see cref="IPlayer"/> comes online.
        /// </summary>
        public void OnLogin()
        {
            foreach (IChatChannel channel in channels.Values)
                channel.OnMemberLogin(owner);
        }

        /// <summary>
        /// Invoked when a <see cref="IPlayer"/> goes offline.
        /// </summary>
        public void OnLogout()
        {
            foreach (IChatChannel channel in channels.Values)
                channel.OnMemberLogout(owner);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can join the <see cref="IChatChannel"/> with supplied password.
        /// </summary>
        public ChatResult CanJoin(string name, string password)
        {
            if (channels.Count >= 20)
                return ChatResult.TooManyCustomChannels;

            if (!TextFilterManager.Instance.IsTextValid(name)
                || !TextFilterManager.Instance.IsTextValid(name, UserText.ChatCustomChannelName))
                return ChatResult.InvalidPasswordText;

            IChatChannel channel = GlobalChatManager.Instance.GetChatChannel(ChatChannelType.Custom, name);
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
        /// Add a new <see cref="IPlayer"/> to the <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoin(string, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Join(string name, string password)
        {
            IChatChannel channel = GlobalChatManager.Instance.GetChatChannel(ChatChannelType.Custom, name);
            channel ??= GlobalChatManager.Instance.CreateChatChannel(ChatChannelType.Custom, name, password);
            channel.Join(owner, password);
            
            channels.Add(channel.Id, channel);

            GlobalChatManager.Instance.TrackCharacterChatChannel(owner.CharacterId, ChatChannelType.Custom, channel.Id);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can leave <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanLeave(ulong chatId)
        {
            if (!channels.ContainsKey(chatId))
                return ChatResult.NotMember;

            return ChatResult.Ok;
        }

        /// <summary>
        /// Remove an existing <see cref="IPlayer"/> from <see cref="IChatChannel"/> with supplied <see cref="IPlayer"/>.
        /// </summary>
        public void Leave(ulong chatId, ChatChannelLeaveReason reason)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return;

            channel.Leave(owner, reason);
            channels.Remove(chatId);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can kick target from <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanKick(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanKick(owner, target);
        }

        /// <summary>
        /// Kick target from <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanKick(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Kick(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return;

            channel.Kick(owner, target);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can list all members of <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanListMembers(ulong chatId)
        {
            if (!channels.ContainsKey(chatId))
                return ChatResult.NotMember;

            return ChatResult.Ok;
        }

        /// <summary>
        /// List all members of <see cref="IChatChannel"/>.
        /// </summary>
        public void ListMembers(ulong chatId)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return;

            channel.ListMembers(owner);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can set password of <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanSetPassword(ulong chatId, string password)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanSetPassword(owner, password);
        }

        /// <summary>
        /// Set password for <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanSetPassword(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void SetPassword(ulong chatId, string password)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return;

            channel.SetPassword(owner, password);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can make target owner of <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanPassOwner(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanPassOwner(owner, target);
        }

        /// <summary>
        /// Make target owner of <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanPassOwner(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void PassOwner(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return;

            channel.PassOwner(owner, target);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can make target a moderator in <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanMakeModerator(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanMakeModerator(owner, target);
        }

        /// <summary>
        /// Make target moderator in <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMakeModerator(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void MakeModerator(ulong chatId, string target, bool status)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return;

            channel.MakeModerator(owner, target, status);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can mute target in <see cref="IChatChannel"/>.
        /// </summary>
        public ChatResult CanMute(ulong chatId, string target)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return ChatResult.DoesntExist;

            return channel.CanMuteMember(owner, target);
        }

        /// <summary>
        /// Mute or unmute target in <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMute(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        public void Mute(ulong chatId, string target, bool status)
        {
            if (!channels.TryGetValue(chatId, out IChatChannel channel))
                return;

            channel.MuteMember(owner, target, status);
        }
    }
}
