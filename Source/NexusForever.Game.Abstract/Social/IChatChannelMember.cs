using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Game.Static.Social;

namespace NexusForever.Game.Abstract.Social
{
    public interface IChatChannelMember : IDatabaseCharacter, IDatabaseState
    {
        ulong Id { get; set; }
        ulong CharacterId { get; set; }
        ChatChannelMemberFlags Flags { get; set; }
        bool IsOnline { get; set; }

        /// <summary>
        /// Returns if supplied <see cref="ChatChannelMemberFlags"/> exists.
        /// </summary>
        bool HasFlag(ChatChannelMemberFlags flags);

        /// <summary>
        /// Add a new <see cref="ChatChannelMemberFlags"/>.
        /// </summary>
        void SetFlag(ChatChannelMemberFlags flags);

        /// <summary>
        /// Remove an existing <see cref="ChatChannelMemberFlags"/>.
        /// </summary>
        void RemoveFlag(ChatChannelMemberFlags flags);
    }
}