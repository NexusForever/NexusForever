using System;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
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
            amount = value;

            if (save)
                saveMask = SaveMask.Create;
        }
    }
}
