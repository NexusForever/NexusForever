using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using GuildRankModel = NexusForever.Database.Character.Model.GuildRank;

namespace NexusForever.WorldServer.Game.Guild
{
    public class Rank
    {
        public ulong GuildId { get; }
        public byte Index
        {
            get => index;
            set
            {
                if (index != value)
                {
                    index = value;

                }       
            }
        }
        private byte index;
        public string Name { get; private set; }
        public GuildRankPermission GuildPermission { get; private set; }
        public ulong BankWithdrawalPermissions { get; set; }
        public ulong MoneyWithdrawalLimit { get; private set; }
        public ulong RepairLimit { get; private set; }

        public bool PendingDelete => saveMask == RankSaveMask.Delete;

        private RankSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Rank"/> using <see cref="GuildRankModel"/>
        /// </summary>
        public Rank(GuildRankModel model)
        {
            GuildId = model.Id;
            Name = model.Name;
            Index = model.Index;
            GuildPermission = (GuildRankPermission)model.Permission;
            BankWithdrawalPermissions = model.BankWithdrawalPermission;
            MoneyWithdrawalLimit = model.MoneyWithdrawalLimit;
            RepairLimit = model.RepairLimit;

            saveMask = RankSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Rank"/> given necessary parameters
        /// </summary>
        public Rank(string name, ulong guildId, byte index, GuildRankPermission guildRankPermission, ulong bankWithdrawalPermissions, ulong moneyWithdrawalLimit, ulong repairLimit)
        {
            GuildId = guildId;
            Name = name;
            Index = index;
            GuildPermission = guildRankPermission;
            BankWithdrawalPermissions = bankWithdrawalPermissions;
            MoneyWithdrawalLimit = moneyWithdrawalLimit;
            RepairLimit = repairLimit;

            saveMask = RankSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="Rank"/> to a <see cref="GuildRankModel"/>
        /// </summary>
        public void Save(CharacterContext context)
        {
            if (saveMask != RankSaveMask.None)
            {
                if ((saveMask & RankSaveMask.Create) != 0)
                {
                    context.Add(new GuildRankModel
                    {
                        Id = GuildId,
                        Index = Index,
                        Name = Name,
                        Permission = (int)GuildPermission,
                        BankWithdrawalPermission = BankWithdrawalPermissions,
                        MoneyWithdrawalLimit = MoneyWithdrawalLimit,
                        RepairLimit = RepairLimit
                    });
                }
                else if ((saveMask & RankSaveMask.Delete) != 0)
                {
                    var model = new GuildRankModel
                    {
                        Id = GuildId,
                        Index = Index
                    };

                    context.Entry(model).State = EntityState.Deleted;
                }
                else
                {
                    var model = new GuildRankModel
                    {
                        Id = GuildId,
                        Index = Index
                    };

                    EntityEntry<GuildRankModel> entity = context.Attach(model);
                    if ((saveMask & RankSaveMask.Rename) != 0)
                    {
                        model.Name = Name;
                        entity.Property(p => p.Name).IsModified = true;
                    }
                    if ((saveMask & RankSaveMask.Permissions) != 0)
                    {
                        model.Permission = (int)GuildPermission;
                        entity.Property(p => p.Permission).IsModified = true;
                    }
                }

                saveMask = RankSaveMask.None;
            }
        }

        /// <summary>
        /// Rename this <see cref="Rank"/> and enqueue save
        /// </summary>
        public void Rename(string name)
        {
            Name = name;
            saveMask |= RankSaveMask.Rename;
        }

        /// <summary>
        /// Add a <see cref="GuildRankPermission"/> to this <see cref="Rank"/> and enqueue save
        /// </summary>
        public void AddPermission(GuildRankPermission guildRankPermission)
        {
            GuildPermission |= guildRankPermission;
            saveMask |= RankSaveMask.Permissions;
        }

        /// <summary>
        /// Remove a <see cref="GuildRankPermission"/> from this <see cref="Rank"/> and enqueue save
        /// </summary>
        public void RemovePermission(GuildRankPermission guildRankPermission)
        {
            if((GuildPermission & guildRankPermission) == 0)
            {
                GuildPermission |= guildRankPermission;
                saveMask |= RankSaveMask.Permissions;
            }
        }

        /// <summary>
        /// Set this <see cref="Rank"/> <see cref="GuildRankPermission"/> and enqueue save
        /// </summary>
        public void SetPermission(GuildRankPermission guildRankPermission)
        {
            GuildPermission = guildRankPermission;

            saveMask |= RankSaveMask.Permissions;
        }

        /// <summary>
        /// Delete this <see cref="Rank"/> and enqueue save
        /// </summary>
        public void Delete()
        {
            // Entity won't exist if create flag exists, so we set to None and let GC get rid of it.
            if ((saveMask & RankSaveMask.Create) == 0)
                saveMask = RankSaveMask.Delete;
            else
                saveMask = RankSaveMask.None;
        }

        /// <summary>
        /// Return a <see cref="GuildRank"/> packet of this <see cref="Rank"/>
        /// </summary>
        public GuildRank BuildGuildRankPacket()
        {
            return new GuildRank
            {
                RankName = Name,
                PermissionMask = GuildPermission,
                BankWithdrawalPermissions = BankWithdrawalPermissions,
                MoneyWithdrawalLimit = MoneyWithdrawalLimit,
                RepairLimit = RepairLimit
            };
        }

    }
}
