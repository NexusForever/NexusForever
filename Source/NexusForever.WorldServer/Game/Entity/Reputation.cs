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
        public uint Amount
        {
            get => amount;
            set
            {
                amount = value;
                saveMask |= ReputationSaveMask.Amount;
            }
        }
        public uint amount;

        private ReputationSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Reputation"/> from an existing database model.
        /// </summary>
        public Reputation(CharacterReputation model)
        {
            CharacterId = model.Id;
            Id = model.FactionId;
            Entry = GameTableManager.Faction2.GetEntry(model.FactionId);
            Amount = model.Value;

            saveMask = ReputationSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Reputation"/> from an <see cref="Faction2Entry"/> template.
        /// </summary>
        public Reputation(ulong owner, Faction2Entry entry, uint value = 0u)
        {
            Id = entry.Id;
            CharacterId = owner;
            Entry = entry;
            Amount = value;

            saveMask = ReputationSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == ReputationSaveMask.None)
                return;

            if ((saveMask & ReputationSaveMask.Create) != 0)
            {
                // Reputation doesn't exist in database, all infomation must be saved
                context.Add(new CharacterReputation
                {
                    Id = CharacterId,
                    FactionId = Entry.Id,
                    Value = Amount,
                });
            }
            else
            {
                // Currency already exists in database, save only data that has been modified
                var model = new CharacterReputation
                {
                    Id = CharacterId,
                    FactionId = Entry.Id,
                };

                // could probably clean this up with reflection, works for the time being
                EntityEntry<CharacterReputation> entity = context.Attach(model);
                if ((saveMask & ReputationSaveMask.Amount) != 0)
                {
                    model.Value = Amount;
                    entity.Property(p => p.Value).IsModified = true;
                }
            }

            saveMask = ReputationSaveMask.None;
        }
    }
}
