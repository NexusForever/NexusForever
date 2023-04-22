using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Social;

namespace NexusForever.Game.Abstract.Social
{
    public interface IChatManager
    {
        /// <summary>
        /// Invoked when a <see cref="IPlayer"/> comes online.
        /// </summary>
        void OnLogin();

        /// <summary>
        /// Invoked when a <see cref="IPlayer"/> goes offline.
        /// </summary>
        void OnLogout();

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can join the <see cref="IChatChannel"/> with supplied password.
        /// </summary>
        ChatResult CanJoin(string name, string password);

        /// <summary>
        /// Add a new <see cref="IPlayer"/> to the <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoin(string, string)"/> should be invoked before invoking this method.
        /// </remarks>
        void Join(string name, string password);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can leave <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanLeave(ulong chatId);

        /// <summary>
        /// Remove an existing <see cref="IPlayer"/> from <see cref="IChatChannel"/> with supplied <see cref="IPlayer"/>.
        /// </summary>
        void Leave(ulong chatId, ChatChannelLeaveReason reason);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can kick target from <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanKick(ulong chatId, string target);

        /// <summary>
        /// Kick target from <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanKick(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        void Kick(ulong chatId, string target);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can list all members of <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanListMembers(ulong chatId);

        /// <summary>
        /// List all members of <see cref="IChatChannel"/>.
        /// </summary>
        void ListMembers(ulong chatId);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can set password of <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanSetPassword(ulong chatId, string password);

        /// <summary>
        /// Set password for <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanSetPassword(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        void SetPassword(ulong chatId, string password);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can make target owner of <see cref="IChatChannel"/>.
        /// </summary>
        void PassOwner(ulong chatId, string target);

        /// <summary>
        /// Make target owner of <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanPassOwner(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        ChatResult CanPassOwner(ulong chatId, string target);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can make target a moderator in <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanMakeModerator(ulong chatId, string target);

        /// <summary>
        /// Make target moderator in <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMakeModerator(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        void MakeModerator(ulong chatId, string target, bool status);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can mute target in <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanMute(ulong chatId, string target);

        /// <summary>
        /// Mute or unmute target in <see cref="IChatChannel"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanMute(ulong, string)"/> should be invoked before invoking this method.
        /// </remarks>
        void Mute(ulong chatId, string target, bool status);
    }
}