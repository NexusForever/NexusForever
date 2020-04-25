using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;

namespace NexusForever.WorldServer.Game.Account
{
    public class AccountItemCooldown : ISaveAuth
    {
        public uint Id { get; }
        public uint CooldownGroupId { get; }
        public DateTime? TimeUsed { get; set; }
        public uint Duration { get; set; }

        private AccountItemCooldownSaveMask saveMask;

        /// <summary>
        /// Create an <see cref="AccountItemCooldown"/> from a database model.
        /// </summary>
        public AccountItemCooldown(AccountItemCooldownModel model)
        {
            Id              = model.Id;
            CooldownGroupId = model.CooldownGroupId;
            TimeUsed        = model.Timestamp;
            Duration        = model.Duration;

            saveMask = AccountItemCooldownSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="AccountItemCooldown"/> for a given ID.
        /// </summary>
        public AccountItemCooldown(WorldSession session, uint cooldownGroupId, uint duration = 0)
        {
            Id = session.Account.Id;
            CooldownGroupId = cooldownGroupId;
            Duration = duration;
            if (Duration > 0)
                TimeUsed = DateTime.UtcNow;

            saveMask = AccountItemCooldownSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="AccountItemCooldown"/> to the database.
        /// </summary>
        public void Save(AuthContext context)
        {
            if (saveMask == AccountItemCooldownSaveMask.None)
                return;

            if ((saveMask & AccountItemCooldownSaveMask.Create) != 0)
            {
                var model = new AccountItemCooldownModel
                {
                    Id = Id,
                    CooldownGroupId = CooldownGroupId,
                    Timestamp = TimeUsed,
                    Duration = Duration
                };

                context.Add(model);
            } 
            else
            {
                var model = new AccountItemCooldownModel
                {
                    Id = Id,
                    CooldownGroupId = CooldownGroupId
                };

                EntityEntry<AccountItemCooldownModel> entity = context.Attach(model);

                if ((saveMask & AccountItemCooldownSaveMask.Modify) != 0)
                {
                    model.Timestamp = TimeUsed;
                    entity.Property(p => p.Timestamp).IsModified = true;

                    model.Duration = Duration;
                    entity.Property(p => p.Duration).IsModified = true;
                }
            }

            saveMask = AccountItemCooldownSaveMask.None;
        }

        /// <summary>
        /// Triggers this <see cref="AccountItemCooldown"/> to set with a given duration (in seconds).
        /// </summary>>
        public void TriggerWithDuration(uint duration)
        {
            Duration = duration;
            TimeUsed = DateTime.UtcNow;

            saveMask |= AccountItemCooldownSaveMask.Modify;
        }

        /// <summary>
        /// Returns the remaining time in seconds before this <see cref="AccountItemCooldown"/> resets.
        /// </summary>
        public uint GetRemainingDuration()
        {
            if (TimeUsed == null)
                return 0;

            return (uint)DateTime.UtcNow.Subtract((DateTime)TimeUsed).TotalSeconds > Duration ? 0 : Duration - (uint)DateTime.UtcNow.Subtract((DateTime)TimeUsed).TotalSeconds;
        }

        /// <summary>
        /// Returns a <see cref="ServerAccountItemCooldownSet"/> to be sent to the client.
        /// </summary>
        public ServerAccountItemCooldownSet BuildNetworkModel()
        {
            return new ServerAccountItemCooldownSet
            {
                AccountItemCooldownGroup = CooldownGroupId,
                CooldownInSeconds = GetRemainingDuration()
            };
        }
    }
}
