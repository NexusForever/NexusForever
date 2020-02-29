using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class AccountEntitlement : Entitlement, ISaveAuth
    {
        private readonly uint accountId;

        /// <summary>
        /// Create a new <see cref="AccountEntitlement"/> from an existing database model.
        /// </summary>
        public AccountEntitlement(AccountEntitlementModel model, EntitlementEntry entry)
            : base(entry, model.Amount, false)
        {
            accountId = model.Id;
        }

        /// <summary>
        /// Create a new <see cref="AccountEntitlement"/> from supplied <see cref="EntitlementEntry"/> and value.
        /// </summary>
        public AccountEntitlement(uint accountId, EntitlementEntry entry, uint value)
            : base(entry, value, true)
        {
            this.accountId = accountId;
        }

        public void Save(AuthContext context)
        {
            if (saveMask == SaveMask.None)
                return;

            var model = new AccountEntitlementModel
            {
                Id            = accountId,
                EntitlementId = (byte)Type,
                Amount        = amount
            };

            if ((saveMask & SaveMask.Create) != 0)
                context.Add(model);
            else
            {
                EntityEntry<AccountEntitlementModel> entity = context.Attach(model);
                entity.Property(p => p.Amount).IsModified = true;
            }

            saveMask = SaveMask.None;
        }
    }
}
