using NexusForever.Database;
using NexusForever.Game.Abstract.Entitlement;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Entitlement
{
    public abstract class Entitlement : IEntitlement
    {
        [Flags]
        protected enum SaveMask
        {
            None   = 0x00,
            Create = 0x01,
            Amount = 0x02
        }

        public EntitlementEntry Entry { get; protected set; }
        public EntitlementType Type => (EntitlementType)Entry.Id;

        public uint Amount
        {
            get => amount;
            set
            {
                if (value > Entry.MaxCount)
                    value = Entry.MaxCount;

                saveMask |= SaveMask.Amount;
                amount = value;
            }
        }

        protected uint amount;

        protected SaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="IEntitlement"/> with supplied <see cref="EntitlementEntry"/> and value.
        /// </summary>
        protected Entitlement(EntitlementEntry entry, uint value)
        {
            if (value > entry.MaxCount)
                throw new DatabaseDataException($"Invalid value {value} for entitlement {Entry.Id}, expected max is {Entry.MaxCount}!");

            Entry  = entry;
            amount = value;
        }
    }
}
