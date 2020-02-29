using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Currency : ISaveCharacter
    {
        public CurrencyTypeEntry Entry { get; set; }
        public CurrencyType Id => (CurrencyType)Entry.Id;
        public ulong CharacterId { get; set; }

        public ulong Amount
        {
            get => amount;
            set
            {
                if (Entry.CapAmount > 0 && value > Entry.CapAmount)
                    throw new ArgumentOutOfRangeException();
                saveMask |= CurrencySaveMask.Amount;
                amount = value;
            }
        }

        private ulong amount;

        private CurrencySaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Currency"/> from an existing database model.
        /// </summary>
        public Currency(CharacterCurrencyModel model)
        {
            CharacterId = model.Id;
            Entry       = GameTableManager.Instance.CurrencyType.GetEntry(model.CurrencyId);
            Amount      = model.Amount;

            saveMask    = CurrencySaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Currency"/> from an <see cref="CurrencyTypeEntry"/> template.
        /// </summary>
        public Currency(ulong owner, CurrencyTypeEntry entry, ulong value = 0u)
        {
            CharacterId = owner;
            Entry       = entry;

            saveMask    = CurrencySaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == CurrencySaveMask.None)
                return;

            if ((saveMask & CurrencySaveMask.Create) != 0)
            {
                // Currency doesn't exist in database, all information must be saved
                context.Add(new CharacterCurrencyModel
                {
                    Id         = CharacterId,
                    CurrencyId = (byte)Entry.Id,
                    Amount     = Amount
                });
            }
            else
            {
                // Currency already exists in database, save only data that has been modified
                var model = new CharacterCurrencyModel
                {
                    Id         = CharacterId,
                    CurrencyId = (byte)Entry.Id,
                };

                // could probably clean this up with reflection, works for the time being
                EntityEntry<CharacterCurrencyModel> entity = context.Attach(model);
                if ((saveMask & CurrencySaveMask.Amount) != 0)
                {
                    model.Amount = Amount;
                    entity.Property(p => p.Amount).IsModified = true;
                }
            }

            saveMask = CurrencySaveMask.None;
        }
    }
}
