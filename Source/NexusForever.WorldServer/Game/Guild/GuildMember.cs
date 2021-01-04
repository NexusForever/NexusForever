using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Guild.Static;
using NetworkGuildMember = NexusForever.WorldServer.Network.Message.Model.Shared.GuildMember;

namespace NexusForever.WorldServer.Game.Guild
{
    public class GuildMember : IBuildable<NetworkGuildMember>
    {
        public GuildBase Guild { get; }
        public ulong CharacterId { get; }

        public GuildRank Rank
        {
            get => rank;
            set
            {
                rank = value;
                saveMask |= GuildMemberSaveMask.Rank;
            }
        }
        private GuildRank rank;

        public string Note
        {
            get => note;
            set
            {
                note = value;
                saveMask |= GuildMemberSaveMask.Note;
            }
        }
        private string note;

        private GuildMemberSaveMask saveMask;

        /// <summary>
        /// Returns if <see cref="GuildMember"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & GuildMemberSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="GuildMember"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & GuildMemberSaveMask.Delete) != 0;

        /// <summary>
        /// Create a new <see cref="GuildMember"/> from an existing database model.
        /// </summary>
        public GuildMember(GuildMemberModel model, GuildBase guild, GuildRank guildRank)
        {
            Guild       = guild;
            CharacterId = model.CharacterId;
            rank        = guildRank;
            note        = model.Note;

            saveMask    = GuildMemberSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="GuildMember"/> from the supplied member information.
        /// </summary>
        public GuildMember(GuildBase guild, ulong characterId, GuildRank guildRank, string note = "")
        {
            Guild       = guild;
            CharacterId = characterId;
            rank        = guildRank;
            this.note   = note;

            saveMask    = GuildMemberSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="GuildMember"/> to a <see cref="GuildMemberModel"/>
        /// </summary>
        public void Save(CharacterContext context)
        {
            if (saveMask == GuildMemberSaveMask.None)
                return;

            var model = new GuildMemberModel
            {
                Id          = Guild.Id,
                CharacterId = CharacterId
            };

            if ((saveMask & GuildMemberSaveMask.Create) != 0)
            {
                model.Rank = rank.Index;
                model.Note = note;
                context.Add(model);
            }
            else if ((saveMask & GuildMemberSaveMask.Delete) != 0)
                context.Remove(model);
            else
            {
                EntityEntry<GuildMemberModel> entity = context.Attach(model);
                if ((saveMask & GuildMemberSaveMask.Rank) != 0)
                {
                    model.Rank = rank.Index;
                    entity.Property(p => p.Rank).IsModified = true;
                }
                if ((saveMask & GuildMemberSaveMask.Note) != 0)
                {
                    model.Note = note;
                    entity.Property(p => p.Note).IsModified = true;
                }
            }

            saveMask = GuildMemberSaveMask.None;
        }

        /// <summary>
        /// Return a <see cref="NetworkGuildMember"/> packet of this <see cref="GuildMember"/>
        /// </summary>
        public NetworkGuildMember Build()
        {
            ICharacter characterInfo = CharacterManager.Instance.GetCharacterInfo(CharacterId);
            return new NetworkGuildMember
            {
                Realm              = WorldServer.RealmId,
                CharacterId        = CharacterId,
                Rank               = rank.Index,
                Name               = characterInfo.Name,
                Sex                = characterInfo.Sex,
                Class              = characterInfo.Class,
                Path               = characterInfo.Path,
                Level              = characterInfo.Level,
                Note               = Note,
                LastLogoutTimeDays = characterInfo.GetOnlineStatus() ?? 0f
            };
        }

        /// <summary>
        /// Enqueue <see cref="GuildMember"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= GuildMemberSaveMask.Delete;
            else
                saveMask &= ~GuildMemberSaveMask.Delete;
        }
    }
}
