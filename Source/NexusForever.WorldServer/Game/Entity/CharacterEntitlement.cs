using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class CharacterEntitlement : Entitlement, ISaveCharacter
    {
        private readonly ulong characterId;

        /// <summary>
        /// Create a new <see cref="CharacterEntitlement"/> from an existing database model.
        /// </summary>
        public CharacterEntitlement(CharacterEntitlementModel model, EntitlementEntry entry)
            : base(entry, model.Amount, false)
        {
            characterId = model.Id;
        }

        /// <summary>
        /// Create a new <see cref="CharacterEntitlement"/> from supplied <see cref="EntitlementEntry"/> and value.
        /// </summary>
        public CharacterEntitlement(ulong characterId, EntitlementEntry entry, uint value)
            : base(entry, value, true)
        {
            this.characterId = characterId;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == SaveMask.None)
                return;

            var model = new CharacterEntitlementModel
            {
                Id            = characterId,
                EntitlementId = (byte)Type,
                Amount        = amount
            };

            if ((saveMask & SaveMask.Create) != 0)
                context.Add(model);
            else
            {
                EntityEntry<CharacterEntitlementModel> entity = context.Attach(model);
                entity.Property(p => p.Amount).IsModified = true;
            }

            saveMask = SaveMask.None;
        }
    }
}
