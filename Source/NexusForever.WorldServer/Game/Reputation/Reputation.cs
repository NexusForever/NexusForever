using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Reputation.Static;

namespace NexusForever.WorldServer.Game.Reputation
{
    public class Reputation : ISaveCharacterExtended
    {
        [Flags]
        private enum SaveMask
        {
            None   = 0x00,
            Create = 0x01,
            Amount = 0x02
        }

        public Faction Id => Entry.FactionId;
        public FactionNode Entry { get; }

        public float Amount
        {
            get => amount;
            set
            {
                amount = value;
                saveMask |= SaveMask.Amount;
            }
        }

        private float amount;

        private SaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Reputation"/> from an existing database model.
        /// </summary>
        public Reputation(FactionNode entry, CharacterReputation model)
        {
            Entry    = entry;
            amount   = model.Amount;

            saveMask = SaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Reputation"/> from the supplied <see cref="FactionNode"/> and amount.
        /// </summary>
        public Reputation(FactionNode entry, float amount)
        {
            Entry       = entry;
            this.amount = amount;

            saveMask    = SaveMask.Create;
        }

        public void Save(ulong characterId, CharacterContext context)
        {
            if (saveMask == SaveMask.None)
                return;

            var model = new CharacterReputation
            {
                Id        = characterId,
                FactionId = (uint)Id,
                Amount    = Amount
            };

            if ((saveMask & SaveMask.Create) != 0)
                context.Add(model);
            else
            {
                EntityEntry<CharacterReputation> entity = context.Attach(model);
                if ((saveMask & SaveMask.Amount) != 0)
                {
                    model.Amount = Amount;
                    entity.Property(p => p.Amount).IsModified = true;
                }
            }

            saveMask = SaveMask.None;
        }
    }
}
