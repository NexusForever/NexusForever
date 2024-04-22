using NexusForever.Database.Auth;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Account.Costume
{
    public interface IAccountCostumeManager : IDatabaseAuth
    {
        /// <summary>
        /// Returns if costume item has been unlocked.
        /// </summary>
        bool HasItemUnlock(uint itemId);

        /// <summary>
        /// Unlock costume item with supplied <see cref="IItem"/>.
        /// </summary>
        void UnlockItem(IItem item);

        /// <summary>
        /// Unlock costume item with supplied item id.
        /// </summary>
        void UnlockItem(uint itemId);

        /// <summary>
        /// Forget costume item unlock of supplied item id.
        /// </summary>
        void ForgetItem(uint itemId);
        
        void SendInitialPackets();
    }
}