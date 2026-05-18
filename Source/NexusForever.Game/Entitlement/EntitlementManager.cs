using System.Collections;
using NexusForever.Game.Abstract.Entitlement;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Entitlement
{
    public abstract class EntitlementManager<T> : IEntitlementManager<T> where T : IEntitlement
    {
        protected readonly Dictionary<EntitlementType, T> entitlements = new();

        /// <summary>
        /// Return <typeparamref name="T"/> for supplied <see cref="EntitlementType"/>.
        /// </summary>
        public T GetEntitlement(EntitlementType type)
        {
            return entitlements.TryGetValue(type, out T entitlement) ? entitlement : default;
        }

        protected virtual bool CanUpdateEntitlement(EntitlementEntry entry, int value)
        {
            if (value > entry.MaxCount)
                return false;

            EntitlementFlags entitlementFlags = (EntitlementFlags)entry.Flags;
            if (entitlementFlags.HasFlag(EntitlementFlags.Disabled))
                return false;

            EntitlementType entitlementType = (EntitlementType)entry.Id;
            if (GetEntitlement(entitlementType)?.Amount + value > entry.MaxCount)
                return false;

            return true;
        }

        /// <summary>
        /// Create or update account or character <see cref="EntitlementType"/> with supplied value.
        /// </summary>
        public void UpdateEntitlement(EntitlementType type, int value)
        {
            EntitlementEntry entry = GameTableManager.Instance.Entitlement.GetEntry((ulong)type);
            if (entry == null)
                throw new ArgumentException($"Invalid entitlement type {type}!");

            UpdateEntitlement(entry, value);
        }

        protected virtual void UpdateEntitlement(EntitlementEntry entry, int value)
        {
            if (!CanUpdateEntitlement(entry, value))
                throw new InvalidOperationException();

            EntitlementType type = (EntitlementType)entry.Id;
            if (!entitlements.TryGetValue(type, out T entitlement))
            {
                entitlement = CreateEntitlement(entry);
                entitlements[type] = entitlement;
            }

            UpdateEntitlement(entitlement, value);
        }

        private void UpdateEntitlement(T entitlement, int value)
        {
            if (value > 0 && entitlement.Amount + (uint)value > entitlement.Entry.MaxCount)
                throw new ArgumentException($"Failed to update entitlement {entitlement.Entry.Id}, incrementing by {value} exceeds max value!");

            if (value < 0 && (int)entitlement.Amount + value < 0)
                throw new ArgumentException($"Failed to update entitlement {entitlement.Entry.Id}, decrementing by {value} subceeds 0!");

            entitlement.Amount = (uint)((int)entitlement.Amount + value);

            SendEntitlement(entitlement);
        }

        protected abstract T CreateEntitlement(EntitlementEntry type);
        protected abstract void SendEntitlement(T entitlement);

        public IEnumerator<T> GetEnumerator()
        {
            return entitlements.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
