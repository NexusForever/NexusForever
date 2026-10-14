using NexusForever.Database.Character;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.GalacticArchive;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IArchiveArticle : IDatabaseCharacter, INetworkBuildable<ServerGalacticArchiveUpdate>
    {
        ushort Id { get; }
        uint UnlockedFlags { get; }
        uint ViewedFlags { get; }

        /// <summary>
        /// Returns if the article itself is unlocked.
        /// </summary>
        bool IsUnlocked { get; }

        /// <summary>
        /// Returns if every entry in the supplied slot mask is unlocked.
        /// </summary>
        bool HasEntries(uint entryMask);

        /// <summary>
        /// Unlock the article and every entry in the supplied slot mask.
        /// </summary>
        /// <remarks>
        /// Returns false if the article and every supplied entry were already unlocked.
        /// </remarks>
        bool Unlock(uint entryMask);

        /// <summary>
        /// Mark the article and every unlocked entry as viewed.
        /// </summary>
        void View();
    }
}
