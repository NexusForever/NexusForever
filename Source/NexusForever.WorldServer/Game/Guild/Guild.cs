using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Linq;
using GuildBaseModel = NexusForever.Database.Character.Model.Guild;
using GuildDataModel = NexusForever.Database.Character.Model.GuildData;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;

namespace NexusForever.WorldServer.Game.Guild
{
    public class Guild : GuildBase, IGuild
    {
        public uint Flags { get; private set; }
        public GuildStandard GuildStandard { get; private set; }

        public string MessageOfTheDay
        {
            get => messageOfTheDay;
            set
            {
                messageOfTheDay = value;
                guildSaveMask |= GuildSaveMask.MessageOfTheDay;
            }
        }
        private string messageOfTheDay;
        public string AdditionalInfo
        {
            get => additionalInfo;
            set
            {
                additionalInfo = value;
                guildSaveMask |= GuildSaveMask.AdditionalInfo;
            }
        }
        private string additionalInfo;

        private GuildSaveMask guildSaveMask;

        /// <summary>
        /// Create a <see cref="Guild"/> given model data
        /// </summary>
        public Guild(GuildDataModel model, GuildBaseModel baseModel) 
            : base (GuildType.Guild, baseModel)
        {
            Flags = model.Taxes;
            GuildStandard = new GuildStandard
            {
                BackgroundIcon = new GuildStandard.GuildStandardPart
                {
                    GuildStandardPartId = model.BackgroundIconPartId
                },
                ForegroundIcon = new GuildStandard.GuildStandardPart
                {
                    GuildStandardPartId = model.ForegroundIconPartId
                },
                ScanLines = new GuildStandard.GuildStandardPart
                {
                    GuildStandardPartId = model.ScanLinesPartId
                }
            };
            MessageOfTheDay = model.MessageOfTheDay;
            AdditionalInfo = model.AdditionalInfo;

            guildSaveMask = GuildSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Guild"/> given necessary parameters
        /// </summary>
        public Guild(WorldSession leaderSession, string guildName, string leaderRankName, string councilRankName, string memberRankName, GuildStandard guildStandard)
            : base(GuildType.Guild)
        {
            Name = guildName;
            LeaderId = leaderSession.Player.CharacterId;
            Flags = 0;

            // Add Default Ranks & Assign Default Permissions for Guild
            AddRank(new Rank(leaderRankName, Id, 0, GuildRankPermission.Leader, ulong.MaxValue, long.MaxValue, long.MaxValue));
            AddRank(new Rank(councilRankName, Id, 1, (GuildRankPermission.OfficerChat | GuildRankPermission.MemberChat | GuildRankPermission.Kick | GuildRankPermission.Invite | GuildRankPermission.ChangeMemberRank | GuildRankPermission.Vote), ulong.MaxValue, long.MaxValue, long.MaxValue));
            AddRank(new Rank(memberRankName, Id, 9, GuildRankPermission.MemberChat, 0, 0, 0));

            GuildStandard = guildStandard;

            Player player = leaderSession.Player;
            Member Leader = new Member(Id, player.CharacterId, GetRank(0), "", this);
            AddMember(Leader);
            OnlineMembers.Add(Leader.CharacterId);

            guildSaveMask = GuildSaveMask.Create;
            base.saveMask = GuildBaseSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="Guild"/> to a <see cref="GuildDataModel"/>. Deletion should be handled by <see cref="GuildBase"/> & Foreign Keys.
        /// </summary>
        public override void Save(CharacterContext context)
        {
            if ((base.saveMask & GuildBaseSaveMask.Delete) != 0)
            {
                base.Save(context);
                return;
            }

            base.Save(context);

            if (guildSaveMask != GuildSaveMask.None)
            {
                if ((guildSaveMask & GuildSaveMask.Create) != 0)
                {
                    context.Add(new GuildDataModel
                    {
                        Id = Id,
                        Taxes = Flags,
                        AdditionalInfo = AdditionalInfo,
                        MessageOfTheDay = MessageOfTheDay,
                        BackgroundIconPartId = GuildStandard.BackgroundIcon.GuildStandardPartId,
                        ForegroundIconPartId = GuildStandard.ForegroundIcon.GuildStandardPartId,
                        ScanLinesPartId = GuildStandard.ScanLines.GuildStandardPartId
                    });
                }
                else
                {
                    var model = new GuildDataModel
                    {
                        Id = Id
                    };

                    EntityEntry<GuildDataModel> entity = context.Attach(model);
                    if ((guildSaveMask & GuildSaveMask.MessageOfTheDay) != 0)
                    {
                        model.MessageOfTheDay = MessageOfTheDay;
                        entity.Property(p => p.MessageOfTheDay).IsModified = true;
                    }
                    if ((guildSaveMask & GuildSaveMask.Taxes) != 0)
                    {
                        model.Taxes = Flags;
                        entity.Property(p => p.Taxes).IsModified = true;
                    }
                    if ((guildSaveMask & GuildSaveMask.AdditionalInfo) != 0)
                    {
                        model.AdditionalInfo = AdditionalInfo;
                        entity.Property(p => p.AdditionalInfo).IsModified = true;
                    }
                }

                guildSaveMask = GuildSaveMask.None;
            }
        }

        /// <summary>
        /// Return a <see cref="GuildData"/> packet of this <see cref="Guild"/>
        /// </summary>
        public override GuildData BuildGuildDataPacket()
        {
            return new GuildData
            {
                GuildId = Id,
                GuildName = Name,
                Flags = Flags,
                Type = Type,
                Ranks = GetGuildRanksPackets().ToList(),
                GuildStandard = GuildStandard,
                MemberCount = (uint)members.Count,
                OnlineMemberCount = (uint)OnlineMembers.Count,
                GuildInfo =
                {
                    MessageOfTheDay = MessageOfTheDay,
                    GuildInfo = AdditionalInfo,
                    GuildCreationDateInDays = (float)DateTime.Now.Subtract(CreateTime).TotalDays * -1f
                }
            };
        }

        /// <summary>
        /// Set the <see cref="Guild"/> <see cref="Flags"/> to indicate whether or not guild taxes are enabled
        /// </summary>
        public void SetTaxes(bool taxesEnabled)
        {
            Flags = Convert.ToUInt32(taxesEnabled);
            guildSaveMask |= GuildSaveMask.Taxes;
        }
    }
}
