using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NLog;
using CharacterReputation = NexusForever.WorldServer.Database.Character.Model.CharacterReputation;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Reputation : ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public Faction2Entry Entry { get; set; }
        public ulong Id { get; }
        public ulong CharacterId { get; set; }
        public ulong Value { get; set; }

        //public ulong Amount
        //{
        //    get => amount;
        //    set
        //    {
        //        if (Entry.CapAmount > 0 && value > Entry.CapAmount)
        //            throw new ArgumentOutOfRangeException();
        //        saveMask |= CurrencySaveMask.Amount;
        //        amount = value;
        //    }
        //}

        //private ulong amount;

        private ReputationSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Reputation"/> from an existing database model.
        /// </summary>
        public Reputation(CharacterReputation model)
        {
            CharacterId = model.Id;
            Id = model.FactionId;
            Entry = GameTableManager.Faction2.GetEntry(model.FactionId);
            Value = model.Value;

            saveMask = ReputationSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Reputation"/> from an <see cref="Faction2Entry"/> template.
        /// </summary>
        public Reputation(ulong owner, Faction2Entry entry, ulong value = 0u)
        {
            Id = entry.Id;
            CharacterId = owner;
            Entry = entry;
            Value = value;

            saveMask = ReputationSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == ReputationSaveMask.None)
                return;

            if ((saveMask & ReputationSaveMask.Create) != 0)
            {
                log.Info($"Should be saving: {CharacterId}, {Entry.Id}, {Value}");
                // Reputation doesn't exist in database, all infomation must be saved
                context.Add(new CharacterReputation
                {
                    Id = CharacterId,
                    FactionId = Entry.Id,
                    Value = Value,
                });
            }
            //else
            //{
            //    // Currency already exists in database, save only data that has been modified
            //    var model = new CharacterCurrency
            //    {
            //        Id = CharacterId,
            //        CurrencyId = (byte)Entry.Id,
            //    };

            //    // could probably clean this up with reflection, works for the time being
            //    EntityEntry<CharacterCurrency> entity = context.Attach(model);
            //    if ((saveMask & CurrencySaveMask.Amount) != 0)
            //    {
            //        model.Amount = Amount;
            //        entity.Property(p => p.Amount).IsModified = true;
            //    }
            //}

            saveMask = ReputationSaveMask.None;
        }
    }
}
