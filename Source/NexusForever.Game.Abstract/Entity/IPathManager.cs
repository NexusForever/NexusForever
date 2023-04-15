using NexusForever.Database.Character;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPathManager : IDatabaseCharacter, IEnumerable<IPathEntry>
    {
        /// <summary>
        /// Checks to see if supplied <see cref="Static.Entity.Path"/> is active.
        /// </summary>
        bool IsPathActive(Static.Entity.Path pathToCheck);

        /// <summary>
        /// Attempts to activate supplied <see cref="Static.Entity.Path"/>. 
        /// </summary>
        void ActivatePath(Static.Entity.Path pathToActivate);

        /// <summary>
        /// Checks if supplied <see cref="Static.Entity.Path"/> is unlocked. 
        /// </summary>
        bool IsPathUnlocked(Static.Entity.Path pathToUnlock);

        /// <summary>
        /// Attemps to unlock supplied <see cref="Static.Entity.Path"/>.
        /// </summary>
        void UnlockPath(Static.Entity.Path pathToUnlock);

        /// <summary>
        /// Add XP to the current <see cref="Static.Entity.Path"/>.
        /// </summary>
        void AddXp(uint xp);

        void SendInitialPackets();
        void SendSetUnitPathTypePacket();
        void SendServerPathActivateResult(GenericError result = GenericError.Ok);
        void SendServerPathUnlockResult(GenericError result = GenericError.Ok);
    }
}