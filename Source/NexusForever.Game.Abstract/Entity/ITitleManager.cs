using NexusForever.Database.Character;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ITitleManager : IDatabaseCharacter, IUpdate, IEnumerable<ITitle>
    {
        ushort ActiveTitleId { get; set; }

        /// <summary>
        /// Add new <see cref="ITitle"/> with supplied title id.
        /// </summary>
        /// <remarks>
        /// If suppress if true, update won't be sent to client.
        /// </remarks>
        void AddTitle(ushort titleId, bool suppress = false);

        /// <summary>
        /// Revoke <see cref="ITitle"/> with supplied title id.
        /// </summary>
        /// <remarks>
        /// If suppress if true, update won't be sent to client.
        /// </remarks>
        void RevokeTitle(ushort titleId, bool suppress = false);

        /// <summary>
        /// Send all owned titles to client.
        /// </summary>
        void SendTitles();

        /// <summary>
        /// Add all available titles.
        /// </summary>
        /// <remarks>
        /// This is only used debug/command purposes.
        /// </remarks>
        void AddAllTitles();
        
        /// <summary>
        /// Remove all available titles.
        /// </summary>
        /// <remarks>
        /// This is only used debug/command purposes.
        /// </remarks>
        void RevokeAllTitles();

        /// <summary>
        /// Returns if title with supplied id is owned.
        /// </summary>
        bool HasTitle(ushort id);
    }
}