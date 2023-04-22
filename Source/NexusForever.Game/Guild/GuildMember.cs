using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Character;
using NetworkGuildMember = NexusForever.Network.World.Message.Model.Shared.GuildMember;

namespace NexusForever.Game.Guild
{
    public class GuildMember : IGuildMember
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IGuildMember"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum GuildMemberSaveMask
        {
            None                     = 0x0000,
            Create                   = 0x0001,
            Delete                   = 0x0002,
            Rank                     = 0x0004,
            Note                     = 0x0008,
            CommunityPlotReservation = 0x0010
        }

        public IGuildBase Guild { get; }
        public ulong CharacterId { get; }

        public IGuildRank Rank
        {
            get => rank;
            set
            {
                rank = value;
                saveMask |= GuildMemberSaveMask.Rank;
            }
        }
        private IGuildRank rank;

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

        public int CommunityPlotReservation
        {
            get => communityPlotReservation;
            set
            {
                communityPlotReservation = value;
                saveMask |= GuildMemberSaveMask.CommunityPlotReservation;
            }
        }
        private int communityPlotReservation;

        private GuildMemberSaveMask saveMask;

        /// <summary>
        /// Returns if <see cref="IGuildMember"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & GuildMemberSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="IGuildMember"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & GuildMemberSaveMask.Delete) != 0;

        /// <summary>
        /// Create a new <see cref="IGuildMember"/> from an existing database model.
        /// </summary>
        public GuildMember(GuildMemberModel model, IGuildBase guild, IGuildRank guildRank)
        {
            Guild                    = guild;
            CharacterId              = model.CharacterId;
            rank                     = guildRank;
            note                     = model.Note;
            communityPlotReservation = model.CommunityPlotReservation;

            saveMask = GuildMemberSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="IGuildMember"/> from the supplied member information.
        /// </summary>
        public GuildMember(IGuildBase guild, ulong characterId, IGuildRank guildRank, string note = "")
        {
            Guild                    = guild;
            CharacterId              = characterId;
            rank                     = guildRank;
            this.note                = note;
            communityPlotReservation = -1;

            saveMask = GuildMemberSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="IGuildMember"/> to a <see cref="GuildMemberModel"/>
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
                if ((saveMask & GuildMemberSaveMask.CommunityPlotReservation) != 0)
                {
                    model.CommunityPlotReservation = communityPlotReservation;
                    entity.Property(p => p.CommunityPlotReservation).IsModified = true;
                }
            }

            saveMask = GuildMemberSaveMask.None;
        }

        /// <summary>
        /// Return a <see cref="NetworkGuildMember"/> packet of this <see cref="IGuildMember"/>
        /// </summary>
        public NetworkGuildMember Build()
        {
            ICharacter characterInfo = CharacterManager.Instance.GetCharacter(CharacterId);
            return new NetworkGuildMember
            {
                Realm                    = RealmContext.Instance.RealmId,
                CharacterId              = CharacterId,
                Rank                     = rank.Index,
                Name                     = characterInfo.Name,
                Sex                      = characterInfo.Sex,
                Class                    = characterInfo.Class,
                Path                     = characterInfo.Path,
                Level                    = characterInfo.Level,
                Note                     = Note,
                LastLogoutTimeDays       = characterInfo.GetOnlineStatus() ?? 0f,
                CommunityPlotReservation = communityPlotReservation
            };
        }

        /// <summary>
        /// Enqueue <see cref="IGuildMember"/> to be deleted from the database.
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
