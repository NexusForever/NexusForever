using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NexusForever.Database.Character;
using GuildBaseModel = NexusForever.Database.Character.Model.Guild;
using GuildRankModel = NexusForever.Database.Character.Model.GuildRank;
using GuildMemberModel = NexusForever.Database.Character.Model.GuildMember;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;
using System.Threading.Tasks;
using NexusForever.WorldServer.Game.CharacterCache;
using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.Network;

namespace NexusForever.WorldServer.Game.Guild
{
    public abstract class GuildBase : IGuild
    {
        public ulong Id { get; }
        public GuildType Type { get; }
        public string Name { get; protected set; }
        public ulong LeaderId { get; protected set; }
        public Member Leader { get; protected set; }
        public DateTime CreateTime { get; }

        protected GuildBaseSaveMask saveMask { get; set; }

        protected SortedDictionary</*index*/byte, Rank> ranks { get; set; } = new SortedDictionary<byte, Rank>();
        private HashSet<Rank> deletedRanks { get; } = new HashSet<Rank>();
        protected Dictionary</*characterId*/ulong, Member> members { get; set; } = new Dictionary<ulong, Member>();
        private HashSet<Member> deletedMembers { get; } = new HashSet<Member>();
        public List</*id*/ulong> OnlineMembers { get; private set; } = new List<ulong>();

        private bool IsPendingDelete = false;

        /// <summary>
        /// Create a new <see cref="GuildBase"/> using <see cref="GuildBaseModel"/>
        /// </summary>
        public GuildBase(GuildType guildType, GuildBaseModel model)
        {
            Id = model.Id;
            Type = (GuildType)model.Type;
            Name = model.Name;
            LeaderId = model.LeaderId;
            CreateTime = model.CreateTime;

            foreach (GuildRankModel guildRankModel in model.GuildRank)
                ranks.Add(guildRankModel.Index, new Rank(guildRankModel));

            foreach (GuildMemberModel guildMemberModel in model.GuildMember)
                members.Add(guildMemberModel.CharacterId, new Member(guildMemberModel, ranks[guildMemberModel.Rank], this));

            Leader = members[LeaderId];

            saveMask = GuildBaseSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="GuildBase"/> of <see cref="GuildType"/>
        /// </summary>
        protected GuildBase(GuildType guildType)
        {
            Id = GlobalGuildManager.Instance.NextGuildId;
            Type = guildType;
            CreateTime = DateTime.Now;

            saveMask = GuildBaseSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="GuildBase"/> to a <see cref="GuildBaseModel"/>
        /// </summary>
        public virtual void Save(CharacterContext context)
        {
            if (saveMask != GuildBaseSaveMask.None)
            {
                if ((saveMask & GuildBaseSaveMask.Create) != 0)
                {
                    context.Add(new GuildBaseModel
                    {
                        Id = Id,
                        Type = (byte)Type,
                        Name = Name,
                        LeaderId = LeaderId,
                        CreateTime = CreateTime
                    });
                }
                else if ((saveMask & GuildBaseSaveMask.Delete) != 0)
                {
                    var model = new GuildBaseModel
                    {
                        Id = Id
                    };

                    context.Entry(model).State = EntityState.Deleted;
                }
                else
                {
                    // residence already exists in database, save only data that has been modified
                    var model = new GuildBaseModel
                    {
                        Id = Id
                    };

                    // could probably clean this up with reflection, works for the time being
                    //EntityEntry <GuildModel> entity = context.Attach(model);
                    //if ((saveMask & GuildSaveMask.Name) != 0)
                    //{
                    //    model.Name = Name;
                    //    entity.Property(p => p.Name).IsModified = true;
                    //}
                }

                saveMask = GuildBaseSaveMask.None;
            }

            // Don't save Ranks or Members if guild is being deleted, throws SQL error.
            // FK handles deleting members & ranks from DB
            if (IsPendingDelete)
                return;
            
            // Saving of deleted ranks must occur before saving of new or existing ranks so that the primary key is available
            foreach (Rank rank in deletedRanks)
                rank.Save(context);

            foreach (Rank rank in ranks.Values)
                rank.Save(context);

            foreach (Member member in deletedMembers)
                member.Save(context);

            foreach (Member member in members.Values)
                member.Save(context);

            deletedRanks.Clear();
            deletedMembers.Clear();
        }

        /// <summary>
        /// Delete this <see cref="GuildBase"/>
        /// </summary>
        public void Delete()
        {
            // Entity won't exist if create flag exists, so we set to None and let GC get rid of it.
            if ((saveMask & GuildBaseSaveMask.Create) == 0)
                saveMask = GuildBaseSaveMask.Delete;
            else
                saveMask = GuildBaseSaveMask.None;

            IsPendingDelete = true;
        }

        /// <summary>
        /// Used to trigger login events for <see cref="Player"/> associated with this <see cref="GuildBase"/>
        /// </summary>
        public void OnPlayerLogin(Player player)
        {
            // TODO: Announce to guild?
            AnnounceGuildMemberChange(player.CharacterId);
            AnnounceGuildResult(GuildResult.MemberOnline, referenceText: player.Name);

            OnlineMembers.Add(player.CharacterId);
        }

        /// <summary>
        /// Used to trigger logout events for <see cref="Player"/> associated with this <see cref="GuildBase"/>
        /// </summary>
        public void OnPlayerLogout(Player player)
        {
            OnlineMembers.Remove(player.CharacterId);

            // TODO: Announce to guild?
            AnnounceGuildMemberChange(player.CharacterId);
            AnnounceGuildResult(GuildResult.MemberOffline, referenceText: player.Name);
        }

        /// <summary>
        /// Add a <see cref="Rank"/> to this <see cref="GuildBase"/>
        /// </summary>
        public void AddRank(Rank rank)
        {
            if (ranks.ContainsKey(rank.Index))
                throw new ArgumentOutOfRangeException("There is already a rank that exists with this index.");
            if (rank.Index > 9 || rank.Index < 0)
                throw new ArgumentOutOfRangeException("Rank Index invalid.");

            ranks.Add(rank.Index, rank);
        }

        /// <summary>
        /// Remove a <see cref="Rank"/> from this <see cref="GuildBase"/> given its index
        /// </summary>
        public void RemoveRank(byte rankIndex)
        {
            if (rankIndex > 9)
                throw new ArgumentOutOfRangeException("Rank Index cannot be higher than the maximum rank count of 10.");
            if (!ranks.ContainsKey(rankIndex))
                throw new ArgumentNullException("Rank does not exist by that rank index");

            RemoveRank(ranks[rankIndex]);
        }

        /// <summary>
        /// Remove the <see cref="Rank"/> from this <see cref="GuildBase"/> and enqueues deletion
        /// </summary>
        private void RemoveRank(Rank rank)
        {
            rank.Delete();
            deletedRanks.Add(rank);
            ranks.Remove(rank.Index);
        }

        /// <summary>
        /// Returns whether or not a rank exists with the given name
        /// </summary>
        public bool RankExists(string name)
        {
            return ranks.Values.FirstOrDefault(i => i.Name == name) != null;
        }

        /// <summary>
        /// Returns the <see cref="Rank"/> using the given index
        /// </summary>
        public Rank GetRank(byte index)
        {
            ranks.TryGetValue(index, out Rank rank);
            return rank;
        }

        /// <summary>
        /// Returns the <see cref="Rank"/> that is below the rank at the given index
        /// </summary>
        public Rank GetDemotedRank(byte index)
        {
            Rank newRank = null;
            for(byte i = (byte)(index + 1); i < 10; i++)
            {
                if (newRank != null)
                    break;

                if (ranks.ContainsKey(i))
                    newRank = ranks[i];
            }

            return newRank;
        }

        /// <summary>
        /// Returns the <see cref="Rank"/> that is above the rank at the given index
        /// </summary>
        public Rank GetPromotedRank(byte index)
        {
            Rank newRank = null;
            for (sbyte i = (sbyte)(index - 1); i > -1; i--)
            {
                if (newRank != null)
                    break;

                if (ranks.ContainsKey((byte)i))
                    newRank = ranks[(byte)i];
            }

            return newRank;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> containing built packets for all <see cref="Rank"/> in this <see cref="GuildBase"/>
        /// </summary>
        public IEnumerable<GuildRank> GetGuildRanksPackets()
        {
            for (byte i = 0; i < 10; i++)
            {
                Rank rank = GetRank(i);
                if (rank != null)
                    yield return rank.BuildGuildRankPacket();
                else
                    yield return new GuildRank();
            }
        }

        /// <summary>
        /// Adds the <see cref="Member"/> to the memberlist for this <see cref="GuildBase"/>
        /// </summary>
        public void AddMember(Member member)
        {
            if (members.ContainsKey(member.CharacterId))
                throw new ArgumentOutOfRangeException("That character already exists in the guild.");

            members.Add(member.CharacterId, member);
        }

        /// <summary>
        /// Removes the <see cref="Member"/> from the memberlist for this <see cref="GuildBase"/> based on their character ID
        /// </summary>
        public void RemoveMember(ulong characterId, out WorldSession memberSession)
        {
            if (!members.ContainsKey(characterId))
                throw new ArgumentNullException("That character does not exist in the guild.");

            // Make sure the session is returned if it exists before removing from OnlineMembers
            memberSession = NetworkManager<WorldSession>.Instance.GetSession(i => i.Player?.CharacterId == characterId);
            if (memberSession != null)
                OnlineMembers.Remove(characterId);

            RemoveMember(members[characterId]);
        }

        /// <summary>
        /// Removes the selected <see cref="Member"/> from this <see cref="GuildBase"/> and enqueues deletion
        /// </summary>
        private void RemoveMember(Member member)
        {
            member.Delete();
            deletedMembers.Add(member);
            members.Remove(member.CharacterId);
        }

        /// <summary>
        /// Returns the number of <see cref="Member"/> in this guild
        /// </summary>
        public uint GetMemberCount()
        {
            return (uint)members.Values.Count;
        }

        /// <summary>
        /// Returns the <see cref="Member"/> matching a character name
        /// </summary>
        public Member GetMember(string characterName)
        {
            return members.Values.FirstOrDefault(i => CharacterManager.Instance.GetCharacterInfo(i.CharacterId).Name == characterName);
        }

        /// <summary>
        /// Returns the <see cref="Member"/> matching a character ID
        /// </summary>
        public Member GetMember(ulong characterId)
        {
            members.TryGetValue(characterId, out Member member);
            return member;
        }

        /// <summary>
        /// Returns all <see cref="Member"/> that are of a given <see cref="Rank"/> index
        /// </summary>
        public IEnumerable<Member> GetMembersOfRank(byte index)
        {
            return members.Values.Where(i => i.Rank.Index == index);
        }

        /// <summary>
        /// Returns all <see cref="GuildMember"/> packets from the memberlist for this <see cref="GuildBase"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GuildMember> GetGuildMembersPackets()
        {
            foreach(Member member in members.Values)
                yield return member.BuildGuildMemberPacket();
        }

        /// <summary>
        /// Returns all <see cref="WorldSession"/> of online members who have a given <see cref="GuildRankPermission"/>
        /// </summary>
        public IEnumerable<WorldSession> GetOnlineMembersWithPermission(GuildRankPermission permission)
        {
            List<WorldSession> worldSessions = new List<WorldSession>();
            IEnumerable<ulong> matchingMembers = members.Keys.Intersect(OnlineMembers);

            foreach(ulong member in matchingMembers)
            {
                if ((GetMember(member).Rank.GuildPermission & permission) != 0)
                    worldSessions.Add(NetworkManager<WorldSession>.Instance.GetSession(i => i.Player?.CharacterId == member));
            }

            return worldSessions.AsEnumerable();
        }

        /// <summary>
        /// Emits an <see cref="IWritable"/> packet to all online members
        /// </summary>
        /// <param name="writable"></param>
        public void SendToOnlineUsers(IWritable writable)
        {
            foreach (ulong characterId in OnlineMembers)
            {
                WorldSession targetSession = NetworkManager<WorldSession>.Instance.GetSession(i => i.Player?.CharacterId == characterId);
                if (targetSession == null)
                    continue;

                targetSession.EnqueueMessageEncrypted(writable);
            }
                
        }

        /// <summary>
        /// Emits an <see cref="IWritable"/> packet to all online guild members who have a <see cref="GuildRankPermission"/>
        /// </summary>
        public void SendToOnlineUsers(IWritable writable, GuildRankPermission requiredPermission = GuildRankPermission.None)
        {
            foreach (ulong characterId in OnlineMembers)
            {
                WorldSession targetSession = NetworkManager<WorldSession>.Instance.GetSession(i => i.Player?.CharacterId == characterId);
                if (targetSession == null)
                    continue;

                if (members.TryGetValue(targetSession.Player.CharacterId, out Member member))
                {
                    if ((member.Rank.GuildPermission & requiredPermission) != 0)
                        targetSession.EnqueueMessageEncrypted(writable);
                }
            }
                
        }

        /// <summary>
        /// Sends a packet notifying all members of a <see cref="GuildResult"/>
        /// </summary>
        public void AnnounceGuildResult(GuildResult guildResult, uint referenceId = 0, string referenceText = "")
        {
            ServerGuildResult serverGuildResult = new ServerGuildResult
            {
                Result = guildResult
            };
            serverGuildResult.RealmId = WorldServer.RealmId;
            serverGuildResult.GuildId = Id;
            serverGuildResult.ReferenceId = referenceId;
            serverGuildResult.ReferenceText = referenceText;

            SendToOnlineUsers(serverGuildResult);
        }

        /// <summary>
        /// Send a packet to all online member containing data for another member based on a character ID
        /// </summary>
        /// <param name="characterId"></param>
        public void AnnounceGuildMemberChange(ulong characterId)
        {
            ServerGuildMemberChange serverGuildMemberChange = new ServerGuildMemberChange
            {
                RealmId = WorldServer.RealmId,
                GuildId = Id,
                GuildMember = members[characterId].BuildGuildMemberPacket(),
                MemberCount = (ushort)members.Count,
                OnlineMemberCount = (ushort)OnlineMembers.Count
            };

            SendToOnlineUsers(serverGuildMemberChange);
        }

        /// <summary>
        /// Return a <see cref="GuildData"/> packet of this <see cref="GuildBase"/>
        /// </summary>
        public abstract GuildData BuildGuildDataPacket();
    }
}
