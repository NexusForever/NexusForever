using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;
using NetworkGuildMember = NexusForever.WorldServer.Network.Message.Model.Shared.GuildMember;
using NetworkGuildRank = NexusForever.WorldServer.Network.Message.Model.Shared.GuildRank;

namespace NexusForever.WorldServer.Game.Guild
{
    public abstract partial class GuildBase : IBuildable<GuildData>, IEnumerable<GuildMember>
    {
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
        /// Returns if <see cref="GuildBase"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & GuildBaseSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="GuildBase"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & GuildBaseSaveMask.Delete) != 0;

        /// <summary>
        /// Maximum number of <see cref="GuildMember"/>'s allowed in the guild.
        /// </summary>
        public abstract uint MaxMembers { get; }

        protected readonly SortedDictionary</*index*/byte, GuildRank> ranks = new SortedDictionary<byte, GuildRank>();
        protected readonly Dictionary</*characterId*/ulong, GuildMember> members = new Dictionary<ulong, GuildMember>();
        protected readonly List</*characterId*/ulong> onlineMembers = new List<ulong>();

        /// <summary>
        /// Create a new <see cref="GuildBase"/> from an existing database model.
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
                if (!ranks.TryGetValue(memberModel.Rank, out GuildRank rank))
                    throw new DatabaseDataException($"Guild member {memberModel.Id} has an invalid rank {memberModel.Rank} for guild {memberModel.Guild.Id}!");

                var member = new GuildMember(memberModel, this, rank);
                rank.AddMember(member);
                members.Add(memberModel.CharacterId, member);
            }

            InitialiseChatChannels();

            saveMask = GuildBaseSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="GuildBase"/> using supplied parameters.
        /// </summary>
        protected GuildBase(GuildType type, string guildName, string leaderRankName, string councilRankName, string memberRankName)
        {
            Id         = GlobalGuildManager.Instance.NextGuildId;
            Type       = type;
            Name       = guildName;
            Flags      = GuildFlag.None;
            CreateTime = DateTime.Now;

            InitialiseRanks(leaderRankName, councilRankName, memberRankName);
            InitialiseChatChannels();

            saveMask = GuildBaseSaveMask.Create;
        }

        protected virtual void InitialiseRanks(string leaderRankName, string councilRankName, string memberRankName)
        {
            AddRank(0, leaderRankName, GuildRankPermission.Leader, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            AddRank(1, councilRankName, GuildRankPermission.Council, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
            AddRank(9, memberRankName, GuildRankPermission.MemberChat, 0, 0, 0);
        }

        protected virtual void InitialiseChatChannels()
        {
            // deliberately empty
        }

        /// <summary>
        /// Save this <see cref="GuildBase"/> to a <see cref="GuildModel"/>
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

            foreach (GuildRank rank in ranks.Values.ToList())
            {
                if (rank.PendingDelete)
                    ranks.Remove(rank.Index);

                rank.Save(context);
            }

            foreach (GuildMember member in members.Values.ToList())
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
        /// Trigger login events for <see cref="Player"/> for <see cref="GuildBase"/>.
        /// </summary>
        public void OnPlayerLogin(Player player)
        {
            if (!members.TryGetValue(player.CharacterId, out GuildMember member))
                throw new ArgumentException($"Invalid member {player.CharacterId} for guild {Id}.");

            MemberOnline(member);

            AnnounceGuildMemberChange(member);
            AnnounceGuildResult(GuildResult.MemberOnline, referenceText: player.Name);
        }

        protected virtual void MemberOnline(GuildMember member)
        {
            onlineMembers.Add(member.CharacterId);
        }

        /// <summary>
        /// Trigger logout events for <see cref="Player"/> for <see cref="GuildBase"/>.
        /// </summary>
        public void OnPlayerLogout(Player player)
        {
            if (!members.TryGetValue(player.CharacterId, out GuildMember member))
                throw new ArgumentException($"Invalid member {player.CharacterId} for guild {Id}.");

            MemberOffline(member);

            AnnounceGuildMemberChange(member);
            AnnounceGuildResult(GuildResult.MemberOffline, referenceText: player.Name);
        }

        protected virtual void MemberOffline(GuildMember member)
        {
            onlineMembers.Remove(member.CharacterId);
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can join the <see cref="GuildBase"/>.
        /// </summary>
        public virtual GuildResultInfo CanJoinGuild(Player player)
        {
            if (MemberCount >= MaxMembers)
                return new GuildResultInfo(GuildResult.CannotInviteGuildFull);

            if (GetMember(player.CharacterId) != null)
                return new GuildResultInfo(GuildResult.AlreadyAMember);

            return new GuildResultInfo(GuildResult.Success);
        }

        /// <summary>
        /// Add a new <see cref="Player"/> to the <see cref="GuildBase"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanJoinGuild(Player)"/> should be invoked before invoking this method.
        /// If the <see cref="GuildBase"/> has no members the <see cref="Player"/> will become the leader.
        /// </remarks>
        public void JoinGuild(Player player)
        {
            GuildMember member;
            if (MemberCount == 0u)
            {
                log.Trace($"Guild{Id} has no leader, new member {player.CharacterId} will be assigned to leader.");

                LeaderId = player.CharacterId;
                member   = AddMember(player.CharacterId, 0);
                SendGuildResult(player.Session, GuildResult.YouCreated, Id, referenceText: Name);
            }
            else
            {
                member = AddMember(player.CharacterId);
                SendGuildResult(player.Session, GuildResult.YouJoined, Id, referenceText: Name);
            }

            SendGuildJoin(player.Session, member.Build(), new GuildPlayerLimits());
            SendGuildRoster(player.Session);
            AnnounceGuildMemberChange(member);
        }

        private void SendGuildJoin(WorldSession session, NetworkGuildMember guildMember, GuildPlayerLimits playerLimits)
        {
            session.EnqueueMessageEncrypted(new ServerGuildJoin
            {
                GuildData   = Build(),
                Self        = guildMember,
                SelfPrivate = playerLimits,
                Nameplate   = session.Player.GuildManager.GuildAffiliation.Id == Id
            });
        }

        private void SendGuildRoster(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerGuildRoster
            {
                GuildRealm = WorldServer.RealmId,
                GuildId    = Id,
                GuildMembers = members.Values
                    .Select(m => m.Build())
                    .ToList(),
                Done       = true
            });
        }

        /// <summary>
        /// Returns if <see cref="Player"/> can leave the <see cref="GuildBase"/>.
        /// </summary>
        public GuildResultInfo CanLeaveGuild(Player player)
        {
            GuildMember member = GetMember(player.CharacterId);
            if (member == null)
                return new GuildResultInfo(GuildResult.NotInThatGuild);

            return CanLeaveGuild(member);
        }

        /// <summary>
        /// Returns if <see cref="GuildMember"/> can leave the <see cref="GuildBase"/>.
        /// </summary>
        protected virtual GuildResultInfo CanLeaveGuild(GuildMember member)
        {
            if (member.Rank.Index == 0)
                return new GuildResultInfo(GuildResult.GuildmasterCannotLeaveGuild);

            return new GuildResultInfo(GuildResult.Success);
        }

        /// <summary>
        /// Remove an existing <see cref="Player"/> from the <see cref="GuildBase"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanLeaveGuild(Player)"/> should be invoked before invoking this method.
        /// </remarks>
        public void LeaveGuild(Player player, GuildResult reason)
        {
            if (!members.TryGetValue(player.CharacterId, out GuildMember member))
                throw new ArgumentException($"Invalid member {player.CharacterId} for guild {Id}.");

            LeaveGuild(member, reason == GuildResult.GuildDisbanded);
            SendGuildResult(player.Session, reason, referenceText: Name);

            player.Session.EnqueueMessageEncrypted(new ServerGuildRemove
            {
                RealmId = WorldServer.RealmId,
                GuildId = Id
            });
        }

        /// <summary>
        /// Remove an existing <see cref="GuildMember"/> from the <see cref="GuildBase"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanLeaveGuild(GuildMember)"/> should be invoked before invoking this method.
        /// </remarks>
        private void LeaveGuild(GuildMember member, bool disband = false)
        {
            RemoveMember(member.CharacterId, disband);

            if (!disband)
            {
                SendToOnlineUsers(new ServerGuildMemberRemove
                {
                    RealmId        = WorldServer.RealmId,
                    GuildId        = Id,
                    PlayerIdentity = new TargetPlayerIdentity
                    {
                        RealmId     = WorldServer.RealmId,
                        CharacterId = member.CharacterId
                    },
                });
            }

            GlobalGuildManager.Instance.UntrackCharacterGuild(member.CharacterId, Id);
        }

        /// <summary>
        /// Returns if the <see cref="GuildBase"/> can be disbanded.
        /// </summary>
        protected virtual GuildResultInfo CanDisbandGuild()
        {
            // deliberately returns success
            return new GuildResultInfo(GuildResult.Success);
        }

        /// <summary>
        /// Disband <see cref="GuildBase"/>.
        /// </summary>
        public void DisbandGuild()
        {
            foreach (GuildMember member in members.Values.ToList())
            {
                // if the player is online handle through the local manager otherwise directly in the guild
                Player player = CharacterManager.Instance.GetPlayer(member.CharacterId);
                if (player != null)
                    player.GuildManager.LeaveGuild(Id, GuildResult.GuildDisbanded);
                else
                    LeaveGuild(member, true);
            }

            saveMask |= GuildBaseSaveMask.Delete;
            log.Trace($"Guild {Id} was disbanded.");
        }

        /// <summary>
        /// Add a new <see cref="GuildRank"/> using the supplied parameters.
        /// </summary>
        private void AddRank(byte index, string name, GuildRankPermission permissions = GuildRankPermission.Disabled,
            ulong bankPermissions = 0ul, ulong bankMoneyWithdrawlLimits = 0ul, ulong repairLimits = 0ul)
        {
            if (index > 9)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, maximum rank index is 9!");

            if (ranks.TryGetValue(index, out GuildRank rank))
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
        /// Remove an existing <see cref="GuildRank"/> at the supplied index.
        /// </summary>
        private void RemoveRank(byte index)
        {
            if (index > 9)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, maximum rank index is 9!");

            if (!ranks.TryGetValue(index, out GuildRank rank))
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
        /// Returns if a <see cref="GuildRank"/> exists with the given name.
        /// </summary>
        private bool RankExists(string name)
        {
            return ranks.Values.FirstOrDefault(r =>
                !r.PendingDelete && string.Equals(r.Name, name, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        /// <summary>
        /// Returns the <see cref="GuildRank"/> using the given index
        /// </summary>
        private GuildRank GetRank(byte index)
        {
            if (ranks.TryGetValue(index, out GuildRank rank) && !rank.PendingDelete)
                return rank;
            return null;
        }

        /// <summary>
        /// Returns the <see cref="GuildRank"/> that is below the rank at the given index.
        /// </summary>
        /// <remarks>
        /// A demoted rank has an increased index.
        /// </remarks>
        private GuildRank GetDemotedRank(byte index)
        {
            if (index > 8)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, maximum demote rank index is 8!");

            for (byte i = (byte)(index + 1); i <= 9; i++)
                if (ranks.TryGetValue(i, out GuildRank rank))
                    return rank;

            return null;
        }

        /// <summary>
        /// Returns the <see cref="GuildRank"/> that is above the rank at the given index.
        /// </summary>
        /// <remarks>
        /// A promoted rank has an decreased index.
        /// </remarks>
        private GuildRank GetPromotedRank(byte index)
        {
            if (index < 1)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, minimum promote rank index is 1!");
            if (index > 9)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} out of range, maximum demote rank index is 9!");

            for (sbyte i = (sbyte)(index - 1); i >= 0; i--)
                if (ranks.TryGetValue((byte)i, out GuildRank rank))
                    return rank;

            return null;
        }

        protected virtual void MemberChangeRank(GuildMember member, GuildRank newRank)
        {
            member.Rank.RemoveMember(member);
            newRank.AddMember(member);
            member.Rank = newRank;
        }

        /// <summary>
        /// Return a collection of <see cref="Network.Message.Model.Shared.GuildRank"/>'s.
        /// </summary>
        /// <remarks>
        /// This will always return 10 ranks, if there are less than 10 ranks in the <see cref="GuildBase"/> empty place holders will be returned.
        /// </remarks>
        protected IEnumerable<NetworkGuildRank> GetGuildRanksPackets()
        {
            for (byte i = 0; i < 10; i++)
            {
                GuildRank rank = GetRank(i);
                if (rank != null)
                    yield return rank.Build();
                else
                    yield return new NetworkGuildRank();
            }
        }

        /// <summary>
        /// Add a new <see cref="GuildMember"/> with supplied character id.
        /// </summary>
        private GuildMember AddMember(ulong characterId, byte rank = 9)
        {
            if (!ranks.TryGetValue(rank, out GuildRank guildRank))
                throw new ArgumentException($"Invalid rank {rank} for guild {Id}.");

            if (members.TryGetValue(characterId, out GuildMember member))
            {
                if (!member.PendingDelete)
                    throw new InvalidOperationException($"Member {characterId} for guild {Id} already exists!");

                // rank is pending delete, reuse object
                member.EnqueueDelete(false);
            }
            else
            {
                // new members default to the lowest rank
                member = new GuildMember(this, characterId, guildRank);
                members.Add(characterId, member);
            }

            MemberOnline(member);
            guildRank.AddMember(member);

            log.Trace($"Added member {characterId} to guild {Id}.");
            return member;
        }

        /// <summary>
        /// Add an existing <see cref="GuildMember"/> with supplied character id.
        /// </summary>
        private void RemoveMember(ulong characterId, bool disband)
        {
            if (!members.TryGetValue(characterId, out GuildMember member))
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
        /// Return <see cref="GuildMember"/> with supplied character id.
        /// </summary>
        public GuildMember GetMember(ulong characterId)
        {
            if (members.TryGetValue(characterId, out GuildMember member) && !member.PendingDelete)
                return member;
            return null;
        }

        /// <summary>
        /// Return <see cref="GuildMember"/> with supplied character name.
        /// </summary>
        public GuildMember GetMember(string name)
        {
            return GetMember(CharacterManager.Instance.GetCharacterIdByName(name));
        }

        /// <summary>
        /// Send <see cref="ServerGuildResult"/> to <see cref="WorldSession"/> based on supplied <see cref="GuildResultInfo"/>.
        /// </summary>
        public static void SendGuildResult(WorldSession session, GuildResultInfo info)
        {
            SendGuildResult(session, info.Result, info.GuildId, info.ReferenceId, info.ReferenceString);
        }

        /// <summary>
        /// Send <see cref="ServerGuildResult"/> to <see cref="WorldSession"/> based on supplied parameters.
        /// </summary>
        public static void SendGuildResult(WorldSession session, GuildResult result, ulong guildId = 0ul, uint referenceId = 0u, string referenceText = "")
        {
            session.EnqueueMessageEncrypted(new ServerGuildResult
            {
                Result        = result,
                RealmId       = WorldServer.RealmId,
                GuildId       = guildId,
                ReferenceId   = referenceId,
                ReferenceText = referenceText
            });
        }

        /// <summary>
        /// Send <see cref="IWritable"/> to all online members.
        /// </summary>
        protected void SendToOnlineUsers(IWritable writable)
        {
            foreach (ulong characterId in onlineMembers)
            {
                Player player = CharacterManager.Instance.GetPlayer(characterId);
                if (player?.Session == null)
                    continue;

                player.Session.EnqueueMessageEncrypted(writable);
            }
        }

        /// <summary>
        /// Sends <see cref="ServerGuildResult"/> to all online members based on supplied parameters.
        /// </summary>
        private void AnnounceGuildResult(GuildResult result, uint referenceId = 0, string referenceText = "")
        {
            SendToOnlineUsers(new ServerGuildResult
            {
                Result        = result,
                RealmId       = WorldServer.RealmId,
                GuildId       = Id,
                ReferenceId   = referenceId,
                ReferenceText = referenceText,
            });
        }

        /// <summary>
        /// Send <see cref="ServerGuildMemberChange"/> to all online members with supplied <see cref="GuildMember"/>.
        /// </summary>
        private void AnnounceGuildMemberChange(GuildMember member)
        {
            SendToOnlineUsers(new ServerGuildMemberChange
            {
                RealmId           = WorldServer.RealmId,
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
            SendToOnlineUsers(new ServerGuildRankChange
            {
                RealmId = WorldServer.RealmId,
                GuildId = Id,
                Ranks   = GetGuildRanksPackets().ToList()
            });
        }

        public IEnumerator<GuildMember> GetEnumerator()
        {
            return members.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
