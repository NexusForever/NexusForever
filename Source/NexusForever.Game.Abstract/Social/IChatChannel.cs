using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Social
{
    public interface IChatChannel : IDatabaseCharacter, IDatabaseState, IEnumerable<IChatChannelMember>
    {
        ChatChannelType Type { get; }
        ulong Id { get; }
        string Name { get; set; }
        string Password { get; set; }

        bool IsGuildChannel();

        /// <summary>
        /// Returns if supplied character is a member of the <see cref="IChatChannel"/>.
        /// </summary>
        bool IsMember(ulong characterId);

        /// <summary>
        /// Invoked when a <see cref="IPlayer"/> comes online.
        /// </summary>
        void OnMemberLogin(IPlayer player);

        /// <summary>
        /// Invoked when a <see cref="IPlayer"/> goes offline.
        /// </summary>
        void OnMemberLogout(IPlayer player);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can join the <see cref="IChatChannel"/> with supplied password.
        /// </summary>
        ChatResult CanJoin(IPlayer player, string password);

        /// <summary>
        /// Add a new <see cref="IPlayer"/> to the <see cref="IChatChannel"/> with supplied password.
        /// </summary>
        void Join(IPlayer player, string password);

        /// <summary>
        /// Add a new character to <see cref="IChatChannel"/>.
        /// </summary>
        void Join(ulong characterId);

        /// <summary>
        /// Remove an existing <see cref="IPlayer"/> from <see cref="IChatChannel"/> with supplied <see cref="ChatChannelLeaveReason"/>.
        /// </summary>
        void Leave(IPlayer player, ChatChannelLeaveReason reason);

        /// <summary>
        /// Remove an existing character from <see cref="IChatChannel"/>.
        /// </summary>
        void Leave(ulong characterId);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can kick target from <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanKick(IPlayer player, string target);

        /// <summary>
        /// Kick target from <see cref="IChatChannel"/>.
        /// </summary>
        void Kick(IPlayer player, string target);

        /// <summary>
        /// List all members of <see cref="IChatChannel"/>.
        /// </summary>
        void ListMembers(IPlayer player);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can set password of <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanSetPassword(IPlayer player, string password);

        /// <summary>
        /// Set password for <see cref="IChatChannel"/>.
        /// </summary>
        void SetPassword(IPlayer player, string password);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can make target owner of <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanPassOwner(IPlayer player, string target);

        /// <summary>
        /// Make target owner of <see cref="IChatChannel"/>.
        /// </summary>
        void PassOwner(IPlayer player, string target);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can make target a moderator in <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanMakeModerator(IPlayer player, string target);

        /// <summary>
        /// Make target moderator in <see cref="IChatChannel"/>.
        /// </summary>
        void MakeModerator(IPlayer player, string target, bool status);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can mute target in <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanMuteMember(IPlayer player, string target);

        /// <summary>
        /// Mute or unmute target in <see cref="IChatChannel"/>.
        /// </summary>
        void MuteMember(IPlayer player, string target, bool status);

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can broadcast in the <see cref="IChatChannel"/>.
        /// </summary>
        ChatResult CanBroadcast(IPlayer player, string text);

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all members in the <see cref="IChatChannel"/>.
        /// </summary>
        void Broadcast(IWritable message, IPlayer except = null);
    }
}