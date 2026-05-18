using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class CharacterEntitlement : Entitlement.Entitlement, ICharacterEntitlement
    {
        private readonly IPlayer player;

        /// <summary>
        /// Create a new <see cref="ICharacterEntitlement"/> from an existing database model.
        /// </summary>
        public CharacterEntitlement(IPlayer player, CharacterEntitlementModel model, EntitlementEntry entry)
            : base(entry, model.Amount)
        {
            this.player = player;
        }

        /// <summary>
        /// Create a new <see cref="ICharacterEntitlement"/> from supplied <see cref="EntitlementEntry"/> and value.
        /// </summary>
        public CharacterEntitlement(IPlayer player, EntitlementEntry entry, uint value)
            : base(entry, value)
        {
            this.player = player;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == SaveMask.None)
                return;

            var model = new CharacterEntitlementModel
            {
                Id            = player.CharacterId,
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

        public ServerEntitlement Build()
        {
            return new ServerEntitlement
            {
                Entitlement = Type,
                Count       = Amount
            };
        }
    }
}
