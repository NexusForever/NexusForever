using System.Collections;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Network;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Game.Entity
{
    public class GenericUnlockManager : ISaveAuth, IEnumerable<GenericUnlock>
    {
        private readonly WorldSession session;
        private readonly Dictionary<uint, GenericUnlock> unlocks = new();

        /// <summary>
        /// Create a new <see cref="GenericUnlockManager"/> from <see cref="AccountModel"/> database model.
        /// </summary>
        public GenericUnlockManager(WorldSession session, AccountModel model)
        {
            this.session = session;

            foreach (AccountGenericUnlockModel unlockModel in model.AccountGenericUnlock)
                unlocks.Add(unlockModel.Entry, new GenericUnlock(unlockModel));
        }

        public void Save(AuthContext context)
        {
            foreach (GenericUnlock genericUnlock in unlocks.Values)
                genericUnlock.Save(context);
        }

        /// <summary>
        /// Create new <see cref="GenericUnlock"/> entry from supplied id.
        /// </summary>
        public void Unlock(ushort genericUnlockEntryId)
        {
            GenericUnlockEntryEntry entry = GameTableManager.Instance.GenericUnlockEntry.GetEntry(genericUnlockEntryId);
            if (entry == null)
            {
                SendUnlockResult(GenericUnlockResult.Invalid);
                return;
            }

            if (unlocks.ContainsKey(genericUnlockEntryId))
            {
                SendUnlockResult(GenericUnlockResult.AlreadyUnlocked);
                return;
            }

            unlocks.Add(genericUnlockEntryId, new GenericUnlock(session.Account, entry));
            
            SendUnlock(genericUnlockEntryId);
            SendUnlockResult(GenericUnlockResult.Unlocked);
        }

        /// <summary>
        /// Unlock all generic unlocks of supplied <see cref="GenericUnlockType"/>.
        /// </summary>
        public void UnlockAll(GenericUnlockType type)
        {
            foreach (GenericUnlockEntryEntry entry in GameTableManager.Instance.GenericUnlockEntry.Entries
                .Where(e => e.GenericUnlockTypeEnum == (uint)type))
            {
                if (unlocks.ContainsKey(entry.Id))
                    continue;

                unlocks.Add(entry.Id, new GenericUnlock(session.Account, entry));
                SendUnlock((ushort)entry.Id);
            }
        }

        public bool IsUnlocked(GenericUnlockType type, uint objectId)
        {
            return unlocks.Values.Any(e => e.Type == type && e.Entry.UnlockObject == objectId);
        }

        public bool IsDyeUnlocked(uint dyeColourRampId)
        {
            return unlocks.Values
                .Where(u => u.Type == GenericUnlockType.Dye)
                .Any(e => e.Entry.UnlockObject == dyeColourRampId);
        }

        /// <summary>
        /// Send <see cref="ServerGenericUnlock"/> with supplied id.
        /// </summary>
        public void SendUnlock(ushort genericUnlockEntryId)
        {
            session.EnqueueMessageEncrypted(new ServerGenericUnlock
            {
                GenericUnlockEntryId = genericUnlockEntryId
            });
        }

        /// <summary>
        /// Send <see cref="ServerGenericUnlockResult"/> with supplied <see cref="GenericUnlockResult"/>.
        /// </summary>
        public void SendUnlockResult(GenericUnlockResult result)
        {
            session.EnqueueMessageEncrypted(new ServerGenericUnlockResult
            {
                Result = result
            });
        }

        /// <summary>
        /// Send <see cref="ServerGenericUnlockResult"/> with all current <see cref="GenericUnlock"/> entries.
        /// </summary>
        public void SendUnlockList()
        {
            session.EnqueueMessageEncrypted(new ServerGenericUnlockList
            {
                Unlocks = unlocks.Keys.ToList()
            });
        }

        public IEnumerator<GenericUnlock> GetEnumerator()
        {
            return unlocks.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
