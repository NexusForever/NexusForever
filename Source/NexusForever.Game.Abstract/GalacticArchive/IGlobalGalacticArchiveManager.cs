using System.Collections.Immutable;
using NexusForever.Game.Static.GalacticArchive;

namespace NexusForever.Game.Abstract.GalacticArchive
{
    public interface IGlobalGalacticArchiveManager
    {
        /// <summary>
        /// Initialise <see cref="IGlobalGalacticArchiveManager"/> and build the unlock rule index from the game tables.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Return a mask of the slots that contain an entry for the article with supplied id.
        /// </summary>
        uint GetEntryMask(ushort articleId);

        /// <summary>
        /// Return a mask of the slots that contain an entry that is unlocked together with the article with supplied id.
        /// </summary>
        uint GetInitialEntryMask(ushort articleId);

        /// <summary>
        /// Return each article and mask of slots that contain an entry with an unlock rule of <see cref="ArchiveEntryUnlockRuleType"/> for the supplied object id.
        /// </summary>
        ImmutableArray<(ushort ArticleId, uint EntryMask)> GetUnlockedEntries(ArchiveEntryUnlockRuleType type, uint objectId);
    }
}
