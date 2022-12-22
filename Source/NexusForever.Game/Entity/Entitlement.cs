using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Entity
{
    public abstract class Entitlement
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
        /// Create a new <see cref="Entitlement"/> with supplied <see cref="EntitlementEntry"/> and value.
        /// </summary>
        protected Entitlement(EntitlementEntry entry, uint value, bool save)
        {
            Entry  = entry;

            // This is here to correct entitlements already existing in database.
            if (value > Entry.MaxCount)
                Amount = Entry.MaxCount;
            else
                amount = value;

            if (save)
                saveMask = SaveMask.Create;
        }
    }
}
