﻿using System.Collections;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Character;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.Message.Model.Shared;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NLog;
using NetworkGuildMember = NexusForever.Network.World.Message.Model.Shared.GuildMember;
using NetworkGuildRank = NexusForever.Network.World.Message.Model.Shared.GuildRank;

namespace NexusForever.Game.Guild
{
    public abstract partial class GuildBase : IGuildBase
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IGuildBase"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum GuildBaseSaveMask
        {
            None     = 0x0000,
            Create   = 0x0001,
            Delete   = 0x0002,
            Name     = 0x0004,
            LeaderId = 0x0008,
            Flags    = 0x0010
        }

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public ulong Id { get; }
        public GuildType Type { get; }
        public DateTime CreateTime { get; }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                saveMask |= GuildBaseSaveMask.Name;
            }
        }
        private string name;

        public ulong? LeaderId
        {
            get => leaderId;
            set
            {
                leaderId = value;
                saveMask |= GuildBaseSaveMask.LeaderId;
            }
        }
        private ulong? leaderId;

        public GuildFlag Flags
        {
            get => guildFlags;
            set
            {
                guildFlags = value;
                saveMask |= GuildBaseSaveMask.Flags;
            }
        }
        private GuildFlag guildFlags;

        private GuildBaseSaveMask saveMask;

        public uint MemberCount => (uint)members.Count;

        /// <summary>
        /// Returns if <see cref="IGuildBase"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & GuildBaseSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="IGuildBase"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & GuildBaseSaveMask.Delete) != 0;

        /// <summary>
        /// Maximum number of <see cref="IGuildMember"/>'s allowed in the guild.
        /// </summary>
        public abstract uint MaxMembers { get; }

        protected readonly SortedDictionary</*index*/byte, IGuildRank> ranks = new();
        protected readonly Dictionary</*characterId*/ulong, IGuildMember> members = new();
        protected readonly List</*characterId*/ulong> onlineMembers = new();

        /// <summary>
        /// Create a new <see cref="IGuildBase"/> from an existing database model.
        /// </summary>
        protected GuildBase(GuildModel model)
        {
            Id         = model.Id;
            Type       = (GuildType)model.Type;
            Name       = model.Name;
            Flags      = (GuildFlag)model.Flags;
            LeaderId   = model.LeaderId;
            CreateTime = model.CreateTime;

            foreach (GuildRankModel rankModel in model.GuildRank)
                ranks.Add(rankModel.Index, new GuildRank(rankModel));

            foreach (GuildMemberModel memberModel in model.GuildMember)
            {
                if (!ranks.TryGetValue(memberModel.Rank, out IGuildRank rank))
                    throw new DatabaseDataException($"Guild member {memberModel.Id} has an invalid rank {memberModel.Rank} for guild {memberModel.Guild.Id}!");
                
                var member = new GuildMember(memberModel, this, rank);
                rank.AddMember(member);
                members.Add(memberModel.CharacterId, member);
            }

            saveMask = GuildBaseSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="IGuildBase"/> using supplied parameters.
        /// </summary>
        protected GuildBase(GuildType type, string guildName, string leaderRankName, string councilRankName, string memberRankName)
        {
            Id         = GlobalGuildManager.Instance.NextGuildId;
            Type       = type;
            Name       = guildName;
            Flags      = GuildFlag.None;
            CreateTime = DateTime.Now;

            InitialiseRanks(leaderRankName, councilRankName, memberRankName);

            saveMask = GuildBaseSaveMask.Create;
        }

        protected virtual void InitialiseRanks(string leaderRankName, string councilRankName, string memberRankName)
        {
            AddRank(0, leaderRankName, GuildRankPermission.Leader, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            AddRank(1, councilRankName, GuildRankPermission.Council, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            AddRank(9, memberRankName, GuildRankPermission.MemberChat, 0, 0, 0);
        }

        /// <summary>
        /// Save this <see cref="IGuildBase"/> to a <see cref="GuildModel"/>
        /// </summary>
        public void Save(CharacterContext context)
        {
            if (saveMask != GuildBaseSaveMask.None)
            {
                if ((saveMask & GuildBaseSaveMask.Create) != 0)
                {
                    context.Add(new GuildModel
                    {
                        Id         = Id,
                        Type       = (byte)Type,
                        Name       = Name,
                        LeaderId   = LeaderId,
                        CreateTime = CreateTime
                    });
                }
                else if ((saveMask & GuildBaseSaveMask.Delete) != 0)
                {
                    var model = new GuildModel
                    {
                        Id = Id
                    };

                    EntityEntry<GuildModel> entity = context.Attach(model);

                    model.DeleteTime = DateTime.UtcNow;
                    entity.Property(p => p.DeleteTime).IsModified = true;

                    model.OriginalName = Name;
                    entity.Property(p => p.OriginalName).IsModified = true;

                    model.Name = null;
                    entity.Property(p => p.Name).IsModified = true;

                    model.OriginalLeaderId = LeaderId;
                    entity.Property(p => p.OriginalLeaderId).IsModified = true;

                    model.LeaderId = null;
                    entity.Property(p => p.LeaderId).IsModified = true;
                }
                else
                {
                    var model = new GuildModel
                    {
                        Id = Id
                    };

                    EntityEntry<GuildModel> entity = context.Attach(model);
                    if ((saveMask & GuildBaseSaveMask.Name) != 0)
                    {
                        model.Name = name;
                        entity.Property(p => p.Name).IsModified = true;
                    }

                    if ((saveMask & GuildBaseSaveMask.LeaderId) != 0)
                    {
                        model.LeaderId = LeaderId;
                        entity.Property(p => p.LeaderId).IsModified = true;
                    }

                    if ((saveMask & GuildBaseSaveMask.Flags) != 0)
                    {
                        model.Flags = (uint)Flags;
                        entity.Property(p => p.Flags).IsModified = true;
                    }
                }
            }

            Save(context, saveMask);
            saveMask = GuildBaseSaveMask.None;

            foreach (IGuildRank rank in ranks.Values.ToList())
            {
                if (rank.PendingDelete)
                    ranks.Remove(rank.Index);

                rank.Save(context);
            }

            foreach (IGuildMember member in members.Values.ToList())
            {
                if (member.PendingDelete)
                    members.Remove(member.CharacterId);

                member.Save(context);
            }
        }

        protected virtual void Save(CharacterContext context, GuildBaseSaveMask saveMask)
        {
            // deliberately empty
        }

        public virtual GuildData Build()
        {
            return new GuildData
            {
                GuildId           = Id,
                GuildName         = Name,
                Flags             = Flags,
                Type              = Type,
                Ranks             = GetGuildRanksPackets().ToList(),
                MemberCount       = (uint)members.Count,
                OnlineMemberCount = (uint)onlineMembers.Count,
                GuildInfo =
                {
                    GuildCreationDateInDays = (float)DateTime.Now.Subtract(CreateTime).TotalDays * -1f
                }
            };
        }

        /// <summary>
        /// Add a new <see cref="GuildFlag"/>.
        /// </summary>
        public void SetFlag(GuildFlag flags)
        {
            Flags |= flags;
        }

        /// <summary>
        /// Remove an existing <see cref="GuildFlag"/>.
        /// </summary>
        public void RemoveFlag(GuildFlag flags)
        {
            Flags &= ~flags;
        }

        /// <summary>
        /// Returns if supplied <see cref="GuildFlag"/> exists.
        /// </summary>
        public bool HasFlags(GuildFlag flags)
        {
            return (Flags & flags) != 0;
        }

        /// <summary>
        /// Trigger login events for <see cref="IPlayer"/> for <see cref="IGuildBase"/>.
        /// </summary>
        public void OnPlayerLogin(IPlayer player)
        {
            if (!members.TryGetValue(player.CharacterId, out IGuildMember member))
                throw new ArgumentException($"Invalid member {player.CharacterId} for guild {Id}.");

            MemberOnline(member);

            AnnounceGuildMemberChange(member);
            AnnounceGuildResult(GuildResult.MemberOnline, referenceText: player.Name);
        }

        /// <summary>
        /// Invoked when a <see cref="IGuildMember"/> comes online.
        /// </summary>
        protected virtual void MemberOnline(IGuildMember member)
        {
            onlineMembers.Add(member.CharacterId);
        }

        /// <summary>
        /// Trigger logout events for <see cref="IPlayer"/> for <see cref="IGuildBase"/>.
        /// </summary>
        public void OnPlayerLogout(IPlayer player)
        {
            if (!members.TryGetValue(player.CharacterId, out IGuildMember member))
                throw new ArgumentException($"Invalid member {player.CharacterId} for guild {Id}.");

            MemberOffline(member);

            AnnounceGuildMemberChange(member);
            AnnounceGuildResult(GuildResult.MemberOffline, referenceText: player.Name);
        }

        /// <summary>
        /// Invoked when a <see cref="IGuildMember"/> goes offline.
        /// </summary>
        protected virtual void MemberOffline(IGuildMember member)
        {
            onlineMembers.Remove(member.CharacterId);
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can join the <see cref="IGuildBase"/>.
        /// </summary>
        public virtual IGuildResultInfo CanJoinGuild(IPlayer player)
        {
            if (MemberCount >= MaxMembers)
                return new GuildResultInfo(GuildResult.CannotInviteGuildFull);

            if (GetMember(player.CharacterId) != null)
                return new GuildResultInfo(GuildResult.AlreadyAMember);

            return new GuildResultInfo(GuildResult.Success);
        }

        /// <summary>
        /// Add a new <see cref="IPlayer"/> to the <see cref="IGuildBase"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoinGuild(IPlayer)"/> should be invoked before invoking this method.
        /// If the <see cref="IGuildBase"/> has no members the <see cref="IPlayer"/> will become the leader.
        /// </remarks>
        public void JoinGuild(IPlayer player)
        {
            IGuildMember member;
            if (MemberCount == 0u)
            {
                log.Trace($"Guild{Id} has no leader, new member {player.CharacterId} will be assigned to leader.");

                LeaderId = player.CharacterId;
                member   = AddMember(player, 0);
                SendGuildResult(player.Session, GuildResult.YouCreated, Id, referenceText: Name);
            }
            else
            {
                member = AddMember(player);
                SendGuildResult(player.Session, GuildResult.YouJoined, Id, referenceText: Name);
            }

            SendGuildJoin(player, member.Build(), new GuildPlayerLimits());
            SendGuildRoster(player.Session);
            AnnounceGuildMemberChange(member);
        }

        private void SendGuildJoin(IPlayer player, NetworkGuildMember guildMember, GuildPlayerLimits playerLimits)
        {
            player.Session.EnqueueMessageEncrypted(new ServerGuildJoin
            {
                GuildData   = Build(),
                Self        = guildMember,
                SelfPrivate = playerLimits,
                Nameplate   = player.GuildManager.GuildAffiliation.Id == Id
            });
        }

        private void SendGuildRoster(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerGuildRoster
            {
                GuildRealm = RealmContext.Instance.RealmId,
                GuildId    = Id,
                GuildMembers = members.Values
                    .Select(m => m.Build())
                    .ToList(),
                Done       = true
            });
        }

        /// <summary>
        /// Returns if <see cref="IPlayer"/> can leave the <see cref="IGuildBase"/>.
        /// </summary>
        public IGuildResultInfo CanLeaveGuild(IPlayer player)
        {
            IGuildMember member = GetMember(player.CharacterId);
            if (member == null)
                return new GuildResultInfo(GuildResult.NotInThatGuild);

            return CanLeaveGuild(member);
        }

        /// <summary>
        /// Returns if <see cref="IGuildMember"/> can leave the <see cref="IGuildBase"/>.
        /// </summary>
        protected virtual IGuildResultInfo CanLeaveGuild(IGuildMember member)
        {
            if (member.Rank.Index == 0)
                return new GuildResultInfo(GuildResult.GuildmasterCannotLeaveGuild);

            return new GuildResultInfo(GuildResult.Success);
        }

        /// <summary>
        /// Remove an existing <see cref="IPlayer"/> from the <see cref="IGuildBase"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanLeaveGuild(IPlayer)"/> should be invoked before invoking this method.
        /// </remarks>
        public void LeaveGuild(IPlayer player, GuildResult reason)
        {
            if (!members.TryGetValue(player.CharacterId, out IGuildMember member))
                throw new ArgumentException($"Invalid member {player.CharacterId} for guild {Id}.");

            LeaveGuild(member, reason == GuildResult.GuildDisbanded);
            SendGuildResult(player.Session, reason, referenceText: Name);

            player.Session.EnqueueMessageEncrypted(new ServerGuildRemove
            {
                RealmId = RealmContext.Instance.RealmId,
                GuildId = Id
            });
        }

        /// <summary>
        /// Remove an existing <see cref="IGuildMember"/> from the <see cref="IGuildBase"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanLeaveGuild(IGuildMember)"/> should be invoked before invoking this method.
        /// </remarks>
        private void LeaveGuild(IGuildMember member, bool disband = false)
        {
            RemoveMember(member.CharacterId, disband);

            if (!disband)
            {
                Broadcast(new ServerGuildMemberRemove
                {
                    RealmId        = RealmContext.Instance.RealmId,
                    GuildId        = Id,
                    PlayerIdentity = new TargetPlayerIdentity
                    {
                        RealmId     = RealmContext.Instance.RealmId,
                        CharacterId = member.CharacterId
                    },
                });
            }

            GlobalGuildManager.Instance.UntrackCharacterGuild(member.CharacterId, Id);
        }

        /// <summary>
        /// Returns if the <see cref="IGuildBase"/> can be disbanded.
        /// </summary>
        protected virtual IGuildResultInfo CanDisbandGuild()
        {
            // deliberately returns success
            return new GuildResultInfo(GuildResult.Success);
        }

        /// <summary>
        /// Disband <see cref="IGuildBase"/>.
        /// </summary>
        public void DisbandGuild()
        {
            foreach (IGuildMember member in members.Values.ToList())
            {
                // if the player is online handle through the local manager otherwise directly in the guild
                IPlayer player = PlayerManager.Instance.GetPlayer(member.CharacterId);
                if (player != null)
                    player.GuildManager.LeaveGuild(Id, GuildResult.GuildDisbanded);
                else
                    LeaveGuild(member, true);
            }

            saveMask |= GuildBaseSaveMask.Delete;
            log.Trace($"Guild {Id} was disbanded.");
        }

        /// <summary>
        /// Add a new <see cref="IGuildRank"/> using the supplied parameters.
        /// </summary>
        private void AddRank(byte index, string name, GuildRankPermission permissions = GuildRankPermission.Disabled,
            ulong bankPermissions = 0ul, ulong bankMoneyWithdrawlLimits = 0ul, ulong repairLimits = 0ul)
        {
            if (index > 9)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, maximum rank index is 9!");

            if (ranks.TryGetValue(index, out IGuildRank rank))
            {
                if (!rank.PendingDelete)
                    throw new InvalidOperationException($"Rank {index} for guild {Id} already exists!");

                // rank is pending delete, reuse object
                rank.EnqueueDelete(false);
                rank.Name                     = name;
                rank.Permissions              = permissions;
                rank.BankPermissions          = bankPermissions;
                rank.BankMoneyWithdrawlLimits = bankMoneyWithdrawlLimits;
                rank.RepairLimit              = repairLimits;
            }
            else
            {
                rank = new GuildRank(Id, index, name, permissions, bankPermissions, bankMoneyWithdrawlLimits, repairLimits);
                ranks.Add(index, rank);
            }

            log.Trace($"Added rank {name}({index}) to guild {Id}.");
        }

        /// <summary>
        /// Remove an existing <see cref="IGuildRank"/> at the supplied index.
        /// </summary>
        private void RemoveRank(byte index)
        {
            if (index > 9)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, maximum rank index is 9!");

            if (!ranks.TryGetValue(index, out IGuildRank rank))
                throw new ArgumentException($"Invalid rank {rank} for guild {Id}.");

            if (rank.MemberCount > 0u)
                throw new InvalidOperationException($"Rank {index} for guild {Id} has existing members!");

            // rank is pending create, direct remove
            if (rank.PendingCreate)
                ranks.Remove(index);
            else
                rank.EnqueueDelete(true);

            log.Trace($"Removed rank {rank.Name}({rank.Index}) from guild {Id}.");
        }

        /// <summary>
        /// Returns if a <see cref="IGuildRank"/> exists with the given name.
        /// </summary>
        private bool RankExists(string name)
        {
            return ranks.Values.FirstOrDefault(r =>
                !r.PendingDelete && string.Equals(r.Name, name, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        /// <summary>
        /// Returns the <see cref="IGuildRank"/> using the given index
        /// </summary>
        private IGuildRank GetRank(byte index)
        {
            if (ranks.TryGetValue(index, out IGuildRank rank) && !rank.PendingDelete)
                return rank;
            return null;
        }

        /// <summary>
        /// Returns the <see cref="IGuildRank"/> that is below the rank at the given index.
        /// </summary>
        /// <remarks>
        /// A demoted rank has an increased index.
        /// </remarks>
        private IGuildRank GetDemotedRank(byte index)
        {
            if (index > 8)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, maximum demote rank index is 8!");

            for (byte i = (byte)(index + 1); i <= 9; i++)
                if (ranks.TryGetValue(i, out IGuildRank rank))
                    return rank;

            return null;
        }

        /// <summary>
        /// Returns the <see cref="IGuildRank"/> that is above the rank at the given index.
        /// </summary>
        /// <remarks>
        /// A promoted rank has an decreased index.
        /// </remarks>
        private IGuildRank GetPromotedRank(byte index)
        {
            if (index < 1)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, minimum promote rank index is 1!");
            if (index > 9)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, maximum demote rank index is 9!");

            for (sbyte i = (sbyte)(index - 1); i >= 0; i--)
                if (ranks.TryGetValue((byte)i, out IGuildRank rank))
                    return rank;

            return null;
        }

        protected virtual void MemberChangeRank(IGuildMember member, IGuildRank newRank)
        {
            member.Rank.RemoveMember(member);
            newRank.AddMember(member);
            member.Rank = newRank;
        }

        /// <summary>
        /// Return a collection of <see cref="Message.Model.Shared.GuildRank"/>'s.
        /// </summary>
        /// <remarks>
        /// This will always return 10 ranks, if there are less than 10 ranks in the <see cref="IGuildBase"/> empty place holders will be returned.
        /// </remarks>
        protected IEnumerable<NetworkGuildRank> GetGuildRanksPackets()
        {
            for (byte i = 0; i < 10; i++)
            {
                IGuildRank rank = GetRank(i);
                if (rank != null)
                    yield return rank.Build();
                else
                    yield return new NetworkGuildRank();
            }
        }

        /// <summary>
        /// Add a new <see cref="IGuildMember"/> with supplied <see cref="IPlayer"/>.
        /// </summary>
        private IGuildMember AddMember(IPlayer player, byte rank = 9)
        {
            if (!ranks.TryGetValue(rank, out IGuildRank guildRank))
                throw new ArgumentException($"Invalid rank {rank} for guild {Id}.");

            if (members.TryGetValue(player.CharacterId, out IGuildMember member))
            {
                if (!member.PendingDelete)
                    throw new InvalidOperationException($"Member {player.CharacterId} for guild {Id} already exists!");

                // rank is pending delete, reuse object
                member.EnqueueDelete(false);
            }
            else
            {
                // new members default to the lowest rank
                member = new GuildMember(this, player.CharacterId, guildRank);
                members.Add(player.CharacterId, member);
            }

            MemberOnline(member);
            guildRank.AddMember(member);

            log.Trace($"Added member {player.CharacterId} to guild {Id}.");
            return member;
        }

        /// <summary>
        /// Remove an existing <see cref="IGuildMember"/> with supplied character id.
        /// </summary>
        private void RemoveMember(ulong characterId, bool disband)
        {
            if (!members.TryGetValue(characterId, out IGuildMember member))
                throw new ArgumentException($"Invalid member {characterId} for guild {Id}.");

            if (!disband)
            {
                // member is pending create, direct remove
                if (member.PendingCreate)
                    members.Remove(characterId);
                else
                    member.EnqueueDelete(true);
            }

            MemberOffline(member);
            member.Rank.RemoveMember(member);

            log.Trace($"Removed member {member.CharacterId} from guild {Id}.");
        }

        /// <summary>
        /// Return <see cref="IGuildMember"/> with supplied character id.
        /// </summary>
        public IGuildMember GetMember(ulong characterId)
        {
            if (members.TryGetValue(characterId, out IGuildMember member) && !member.PendingDelete)
                return member;
            return null;
        }

        /// <summary>
        /// Return <see cref="IGuildMember"/> with supplied character name.
        /// </summary>
        public IGuildMember GetMember(string memberName)
        {
            ulong? characterId = CharacterManager.Instance.GetCharacterIdByName(memberName);
            if (characterId == null)
                return null;

            return GetMember(characterId.Value);
        }

        /// <summary>
        /// Send <see cref="ServerGuildResult"/> to <see cref="IGameSession"/> based on supplied <see cref="IGuildResultInfo"/>.
        /// </summary>
        public static void SendGuildResult(IGameSession session, IGuildResultInfo info)
        {
            SendGuildResult(session, info.Result, info.GuildId, info.ReferenceId, info.ReferenceString);
        }

        /// <summary>
        /// Send <see cref="ServerGuildResult"/> to <see cref="IGameSession"/> based on supplied parameters.
        /// </summary>
        public static void SendGuildResult(IGameSession session, GuildResult result, ulong guildId = 0ul, uint referenceId = 0u, string referenceText = "")
        {
            session.EnqueueMessageEncrypted(new ServerGuildResult
            {
                Result        = result,
                RealmId       = RealmContext.Instance.RealmId,
                GuildId       = guildId,
                ReferenceId   = referenceId,
                ReferenceText = referenceText
            });
        }

        /// <summary>
        /// Send <see cref="IWritable"/> to all online members.
        /// </summary>
        public void Broadcast(IWritable writable)
        {
            foreach (ulong characterId in onlineMembers)
            {
                IPlayer player = PlayerManager.Instance.GetPlayer(characterId);
                player?.Session?.EnqueueMessageEncrypted(writable);
            }
        }

        /// <summary>
        /// Sends <see cref="ServerGuildResult"/> to all online members based on supplied parameters.
        /// </summary>
        private void AnnounceGuildResult(GuildResult result, uint referenceId = 0, string referenceText = "")
        {
            Broadcast(new ServerGuildResult
            {
                Result        = result,
                RealmId       = RealmContext.Instance.RealmId,
                GuildId       = Id,
                ReferenceId   = referenceId,
                ReferenceText = referenceText,
            });
        }

        /// <summary>
        /// Send <see cref="ServerGuildMemberChange"/> to all online members with supplied <see cref="IGuildMember"/>.
        /// </summary>
        protected void AnnounceGuildMemberChange(IGuildMember member)
        {
            Broadcast(new ServerGuildMemberChange
            {
                RealmId           = RealmContext.Instance.RealmId,
                GuildId           = Id,
                GuildMember       = member.Build(),
                MemberCount       = (ushort)members.Count,
                OnlineMemberCount = (ushort)onlineMembers.Count
            });
        }

        /// <summary>
        /// Send <see cref="ServerGuildRankChange"/> to all online members.
        /// </summary>
        private void AnnounceGuildRankChange()
        {
            Broadcast(new ServerGuildRankChange
            {
                RealmId = RealmContext.Instance.RealmId,
                GuildId = Id,
                Ranks   = GetGuildRanksPackets().ToList()
            });
        }

        protected void SendGuildFlagUpdate()
        {
            Broadcast(new ServerGuildFlagUpdate
            {
                RealmId = RealmContext.Instance.RealmId,
                GuildId = Id,
                Value   = (uint)Flags
            });
        }

        /// <summary>
        /// Rename <see cref="IGuildBase"/> with supplied name.
        /// </summary>
        public virtual void RenameGuild(string name)
        {
            Name = name;

            Broadcast(new ServerGuildRename
            {
                TargetGuild = new TargetGuild
                {
                    RealmId = RealmContext.Instance.RealmId,
                    GuildId = Id
                },
                Name = name
            });
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
