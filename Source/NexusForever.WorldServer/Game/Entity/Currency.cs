using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using CurrencyModel = NexusForever.WorldServer.Database.Character.Model.CharacterCurrency;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Currency : ISaveCharacter
    {
        public CurrencyTypeEntry Entry { get; set; }
        public ulong Id { get; }
        private ulong count;
        private CurrencySaveMask saveMask;
        private ulong characterId;

        public ulong CharacterId
        {
            get => characterId;
            set
            {
                characterId = value;
            }
        }

        public ulong Count
        {
            get => count;
            set
            {
                if (Entry.CapAmount > 0 && value > Entry.CapAmount)
                    throw new ArgumentOutOfRangeException();
                saveMask |= CurrencySaveMask.Count;
                count = value;
            }
        }

        /// <summary>
        /// Create a new <see cref="Currency"/> from an existing database model.
        /// </summary>
        public Currency(CurrencyModel model)
        {
            characterId = model.Id;
            Id = model.CurrencyId;
            Entry = GameTableManager.CurrencyType.GetEntry(model.CurrencyId);
            Count = model.Count;

            saveMask = CurrencySaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Currency"/> from an <see cref="Currency2Entry"/> template.
        /// </summary>
        public Currency(ulong owner, CurrencyTypeEntry entry, ulong value = 0u)
        {
            Id = entry.Id;
            characterId = owner;
            Entry = entry;
            Count = count;

            saveMask = CurrencySaveMask.Create;
        }


        public void Save(CharacterContext context)
        {
            if (saveMask == CurrencySaveMask.None)
                return;

            if ((saveMask & CurrencySaveMask.Create) != 0)
            {
                // Currency doesn't exist in database, all infomation must be saved
                context.Add(new CharacterCurrency
                {
                    Id = CharacterId,
                    CurrencyId = (byte)Entry.Id,
                    Count = Count,
                });
            }
            else
            {
                // Currency already exists in database, save only data that has been modified
                var model = new CharacterCurrency
                {
                    Id = CharacterId,
                    CurrencyId = (byte)Entry.Id,
                };

                // could probably clean this up with reflection, works for the time being
                EntityEntry<CharacterCurrency> entity = context.Attach(model);
                if ((saveMask & CurrencySaveMask.Count) != 0)
                {
                    model.Count = Count;
                    entity.Property(p => p.Count).IsModified = true;
                }
            }

            saveMask = CurrencySaveMask.None;
        }
    }
}
