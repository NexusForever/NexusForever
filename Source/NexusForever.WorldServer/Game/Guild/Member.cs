using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using GuildMemberModel = NexusForever.Database.Character.Model.GuildMember;

namespace NexusForever.WorldServer.Game.Guild
{
    public class Member
    {
        public ulong GuildId { get; }
        public ulong CharacterId { get; }
        public Rank Rank { get; private set; }
        public string Note { get; private set; }
        
        private GuildBase Guild { get; }
        private bool IsDeleted() => ((saveMask & MemberSaveMask.Delete) != 0);
        private MemberSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Member"/> from a <see cref="GuildMemberModel"/> for this <see cref="GuildBase"/>
        /// </summary>
        public Member(GuildMemberModel model, Rank rank, GuildBase @base)
        {
            GuildId = model.Id;
            CharacterId = model.CharacterId;
            Rank = rank;
            Note = model.Note;

            Guild = @base;

            saveMask = MemberSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Member"/> for this <see cref="GuildBase"/>
        /// </summary>
        public Member(ulong guildId, ulong characterId, Rank rank, string note, GuildBase @base)
        {
            GuildId = guildId;
            CharacterId = characterId;
            Rank = rank;
            Note = note;

            Guild = @base;

            saveMask = MemberSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="Member"/> to a <see cref="GuildMemberModel"/>
        /// </summary>
        public void Save(CharacterContext context)
        {
            if (saveMask != MemberSaveMask.None)
            {
                if ((saveMask & MemberSaveMask.Create) != 0)
                {
                    context.Add(new GuildMemberModel
                    {
                        Id = GuildId,
                        CharacterId = CharacterId,
                        Rank = Rank.Index,
                        Note = Note
                    });
                }
                else if ((saveMask & MemberSaveMask.Delete) != 0)
                {
                    var model = new GuildMemberModel
                    {
                        Id = GuildId,
                        CharacterId = CharacterId
                    };

                    context.Entry(model).State = EntityState.Deleted;
                }
                else
                {
                    var model = new GuildMemberModel
                    {
                        Id = GuildId,
                        CharacterId = CharacterId,
                    };

                    EntityEntry<GuildMemberModel> entity = context.Attach(model);
                    if ((saveMask & MemberSaveMask.Rank) != 0)
                    {
                        model.Rank = Rank.Index;
                        entity.Property(p => p.Rank).IsModified = true;
                    }
                    if((saveMask & MemberSaveMask.Note) != 0)
                    {
                        model.Note = Note;
                        entity.Property(p => p.Note).IsModified = true;
                    }
                }

                saveMask = MemberSaveMask.None;
            }
        }

        /// <summary>
        /// Delete this <see cref="Member"/> and enqueue save
        /// </summary>
        public void Delete()
        {
            // Entity won't exist if create flag exists, so we set to None and let GC get rid of it.
            if ((saveMask & MemberSaveMask.Create) == 0)
                saveMask = MemberSaveMask.Delete;
            else
                saveMask = MemberSaveMask.None;
        }

        /// <summary>
        /// Change the <see cref="Rank"/> of this <see cref="Member"/> to a new <see cref="Rank"/>
        /// </summary>
        public void ChangeRank(Rank rank)
        {
            if (rank == null)
                throw new ArgumentNullException("Rank not found.");
            if (IsDeleted())
                throw new ArgumentException("Member has been deleted. Cannot modify rank.");

            Rank = rank;
            
            saveMask |= MemberSaveMask.Rank;
        }

        /// <summary>
        /// Set the note for this <see cref="Member"/>
        /// </summary>
        /// <param name="note"></param>
        public void SetNote(string note)
        {
            Note = note;

            saveMask |= MemberSaveMask.Note;
        }

        /// <summary>
        /// Return a <see cref="GuildMember"/> packet of this <see cref="Member"/>
        /// </summary>
        public GuildMember BuildGuildMemberPacket()
        {
            ICharacter characterInfo = CharacterManager.Instance.GetCharacterInfo(CharacterId);
            return new GuildMember
            {
                Realm = WorldServer.RealmId,
                CharacterId = CharacterId,
                Rank = Rank.Index,
                Name = characterInfo.Name,
                Sex = characterInfo.Sex,
                Class = characterInfo.Class,
                Path = characterInfo.Path,
                Level = characterInfo.Level,
                Note = Note,
                LastLogoutTimeDays = characterInfo.GetOnlineStatus() ?? 0f
            };
        }
    }
}
