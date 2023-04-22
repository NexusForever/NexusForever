﻿using System.Collections;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Static.Guild;
using NetworkGuildRank = NexusForever.Network.World.Message.Model.Shared.GuildRank;

namespace NexusForever.Game.Guild
{
    public class GuildRank : IGuildRank
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IGuildRank"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum GuildRankSaveMask
        {
            None                     = 0x0000,
            Create                   = 0x0001,
            Delete                   = 0x0002,
            //Index                    = 0x0004,
            Name                     = 0x0008,
            Permissions              = 0x0010,
            BankPermissions          = 0x0020,
            BankMoneyWithdrawlLimits = 0x0040,
            RepairLimit              = 0x0080
        }

        public ulong GuildId { get; }
        public byte Index { get; }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                saveMask |= GuildRankSaveMask.Name;
            }
        }
        private string name;

        public GuildRankPermission Permissions
        {
            get => permissions;
            set
            {
                permissions = value;
                saveMask |= GuildRankSaveMask.Permissions;
            }
        }
        private GuildRankPermission permissions;

        public ulong BankPermissions
        {
            get => bankPermissions;
            set
            {
                bankPermissions = value;
                saveMask |= GuildRankSaveMask.BankPermissions;
            }
        }
        private ulong bankPermissions;

        public ulong BankMoneyWithdrawlLimits
        {
            get => bankMoneyWithdrawlLimits;
            set
            {
                bankMoneyWithdrawlLimits = value;
                saveMask |= GuildRankSaveMask.BankMoneyWithdrawlLimits;
            }
        }
        private ulong bankMoneyWithdrawlLimits;

        public ulong RepairLimit
        {
            get => repairLimit;
            set
            {
                repairLimit = value;
                saveMask |= GuildRankSaveMask.RepairLimit;
            }
        }
        private ulong repairLimit;

        private GuildRankSaveMask saveMask;

        /// <summary>
        /// Returns if <see cref="IGuildRank"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & GuildRankSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="IGuildRank"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & GuildRankSaveMask.Delete) != 0;

        public uint MemberCount => (uint)members.Count;

        private readonly Dictionary<ulong, IGuildMember> members = new();

        /// <summary>
        /// Create a new <see cref="IGuildRank"/> from an existing database model.
        /// </summary>
        public GuildRank(GuildRankModel model)
        {
            GuildId                  = model.Id;
            Name                     = model.Name;
            Index                    = model.Index;
            permissions              = (GuildRankPermission)model.Permission;
            bankPermissions          = model.BankWithdrawalPermission;
            bankMoneyWithdrawlLimits = model.MoneyWithdrawalLimit;
            repairLimit              = model.RepairLimit;

            saveMask                 = GuildRankSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="IGuildRank"/> using the supplied parameters.
        /// </summary>
        public GuildRank(ulong guildId, byte index, string name, GuildRankPermission permissions,
            ulong bankPermissions, ulong bankMoneyWithdrawlLimits, ulong repairLimit)
        {
            GuildId                       = guildId;
            Name                          = name;
            Index                         = index;
            this.permissions              = permissions;
            this.bankPermissions          = bankPermissions;
            this.bankMoneyWithdrawlLimits = bankMoneyWithdrawlLimits;
            this.repairLimit              = repairLimit;

            saveMask                      = GuildRankSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="IGuildRank"/> to a <see cref="GuildRankModel"/>.
        /// </summary>
        public void Save(CharacterContext context)
        {
            if (saveMask == GuildRankSaveMask.None)
                return;

            var model = new GuildRankModel
            {
                Id    = GuildId,
                Index = Index
            };

            if ((saveMask & GuildRankSaveMask.Create) != 0)
            {
                model.Name                     = name;
                model.Permission               = (uint)permissions;
                model.BankWithdrawalPermission = bankPermissions;
                model.MoneyWithdrawalLimit     = bankMoneyWithdrawlLimits;
                model.RepairLimit              = repairLimit;
                context.Add(model);
            }
            else if ((saveMask & GuildRankSaveMask.Delete) != 0)
                context.Remove(model);
            else
            {
                EntityEntry<GuildRankModel> entity = context.Attach(model);
                if ((saveMask & GuildRankSaveMask.Name) != 0)
                {
                    model.Name = Name;
                    entity.Property(p => p.Name).IsModified = true;
                }
                if ((saveMask & GuildRankSaveMask.Permissions) != 0)
                {
                    model.Permission = (uint)Permissions;
                    entity.Property(p => p.Permission).IsModified = true;
                }
                if ((saveMask & GuildRankSaveMask.BankPermissions) != 0)
                {
                    model.BankWithdrawalPermission = BankPermissions;
                    entity.Property(p => p.BankWithdrawalPermission).IsModified = true;
                }
                if ((saveMask & GuildRankSaveMask.BankMoneyWithdrawlLimits) != 0)
                {
                    model.MoneyWithdrawalLimit = BankMoneyWithdrawlLimits;
                    entity.Property(p => p.MoneyWithdrawalLimit).IsModified = true;
                }
                if ((saveMask & GuildRankSaveMask.RepairLimit) != 0)
                {
                    model.RepairLimit = repairLimit;
                    entity.Property(p => p.RepairLimit).IsModified = true;
                }
            }

            saveMask = GuildRankSaveMask.None;
        }

        public NetworkGuildRank Build()
        {
            return new NetworkGuildRank
            {
                RankName                  = Name,
                PermissionMask            = Permissions,
                BankWithdrawalPermissions = BankPermissions,
                MoneyWithdrawalLimit      = BankMoneyWithdrawlLimits,
                RepairLimit               = RepairLimit
            };
        }

        /// <summary>
        /// Enqueue <see cref="IGuildRank"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= GuildRankSaveMask.Delete;
            else
                saveMask &= ~GuildRankSaveMask.Delete;
        }

        /// <summary>
        /// Add a new <see cref="GuildRankPermission"/>.
        /// </summary>
        public void AddPermission(GuildRankPermission guildRankPermission)
        {
            Permissions |= guildRankPermission;
        }

        /// <summary>
        /// Remove an existing <see cref="GuildRankPermission"/>.
        /// </summary>
        public void RemovePermission(GuildRankPermission guildRankPermission)
        {
            Permissions &= ~guildRankPermission;
        }

        /// <summary>
        /// Returns if supplied <see cref="GuildRankPermission"/> exists.
        /// </summary>
        public bool HasPermission(GuildRankPermission guildRankPermission)
        {
            return (Permissions & guildRankPermission) != 0;
        }

        /// <summary>
        /// Add a new <see cref="IGuildMember"/>.
        /// </summary>
        public void AddMember(IGuildMember member)
        {
            members.Add(member.CharacterId, member);
        }

        /// <summary>
        /// Remove an existing <see cref="IGuildMember"/>
        /// </summary>
        public void RemoveMember(IGuildMember member)
        {
            members.Remove(member.CharacterId);
        }

        public IEnumerator<IGuildMember> GetEnumerator()
        {
            return members.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
