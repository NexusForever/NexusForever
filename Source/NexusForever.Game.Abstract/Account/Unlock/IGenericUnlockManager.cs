using NexusForever.Database.Auth;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Abstract.Account.Unlock
{
    public interface IGenericUnlockManager : IDatabaseAuth, IEnumerable<IGenericUnlock>
    {
        /// <summary>
        /// Create new <see cref="IGenericUnlock"/> entry from supplied id.
        /// </summary>
        void Unlock(ushort genericUnlockEntryId);

        /// <summary>
        /// Unlock all generic unlocks of supplied <see cref="GenericUnlockType"/>.
        /// </summary>
        void UnlockAll(GenericUnlockType type);

        bool IsUnlocked(GenericUnlockType type, uint objectId);
        bool IsDyeUnlocked(uint dyeColourRampId);

        /// <summary>
        /// Send <see cref="IGenericUnlock"/> with supplied id to client.
        /// </summary>
        void SendUnlock(ushort genericUnlockEntryId);

        /// <summary>
        /// Send <see cref="IGenericUnlock"/> with supplied id to client.
        /// </summary>
        void SendUnlockResult(GenericUnlockResult result);

        /// <summary>
        /// Send all <see cref="GenericUnlock"/> entries to client.
        /// </summary>
        void SendUnlockList();
    }
}