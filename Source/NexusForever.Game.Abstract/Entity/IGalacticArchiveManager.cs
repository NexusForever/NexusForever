using NexusForever.Database.Character;
using NexusForever.Game.Static.GalacticArchive;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IGalacticArchiveManager : IDatabaseCharacter
    {
        /// <summary>
        /// Return <see cref="IArchiveArticle"/> with supplied id.
        /// </summary>
        IArchiveArticle GetArticle(ushort id);

        /// <summary>
        /// Unlock article with supplied id at the request of the client.
        /// </summary>
        void UnlockArticle(ushort id);

        /// <summary>
        /// Mark article with supplied id and every unlocked entry in it as viewed at the request of the client.
        /// </summary>
        void ViewArticle(ushort id);

        /// <summary>
        /// Unlock every entry with an unlock rule of <see cref="ArchiveEntryUnlockRuleType"/> for the supplied object id.
        /// </summary>
        /// <remarks>
        /// Must only be invoked once the object is complete, for a quest this is when the quest is handed in.
        /// </remarks>
        void UpdateUnlockRule(ArchiveEntryUnlockRuleType type, uint objectId);

        void SendInitialPackets();
    }
}
