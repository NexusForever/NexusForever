using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Guild
{
    public partial class Guild : GuildBase
    {
        public override uint MaxMembers => 40u;

        public GuildStandard Standard { get; }

        public string MessageOfTheDay
        {
            get => messageOfTheDay;
            set
            {
                messageOfTheDay = value;
                saveMask |= GuildSaveMask.MessageOfTheDay;
            }
        }
        private string messageOfTheDay;

        public string AdditionalInfo
        {
            get => additionalInfo;
            set
            {
                additionalInfo = value;
                saveMask |= GuildSaveMask.AdditionalInfo;
            }
        }
        private string additionalInfo;

        private GuildSaveMask saveMask;

        private ChatChannel memberChannel;
        private ChatChannel officerChannel;

        /// <summary>
        /// Create a new <see cref="Guild"/> from an existing database model.
        /// </summary>
        public Guild(GuildModel model) 
            : base(model)
        {
            Standard        = new GuildStandard(model.GuildData);
            messageOfTheDay = model.GuildData.MessageOfTheDay;
            additionalInfo  = model.GuildData.AdditionalInfo;
        }

        /// <summary>
        /// Create a new <see cref="Guild"/> using the supplied parameters.
        /// </summary>
        public Guild(string name, string leaderRankName, string councilRankName, string memberRankName, GuildStandard standard)
            : base(GuildType.Guild, name, leaderRankName, councilRankName, memberRankName)
        {
            Standard        = standard;
            messageOfTheDay = "";
            additionalInfo  = "";
        }

        protected override void InitialiseChatChannels()
        {
            memberChannel  = SocialManager.Instance.RegisterChatChannel(ChatChannelType.Guild, Id);
            officerChannel = SocialManager.Instance.RegisterChatChannel(ChatChannelType.GuildOfficer, Id);
        }

        protected override void Save(CharacterContext context, GuildBaseSaveMask baseSaveMask)
        {
            if ((baseSaveMask & GuildBaseSaveMask.Create) != 0)
            {
                context.Add(new GuildDataModel
                {
                    Id                   = Id,
                    AdditionalInfo       = AdditionalInfo,
                    MessageOfTheDay      = MessageOfTheDay,
                    BackgroundIconPartId = (ushort)Standard.BackgroundIcon.GuildStandardPartEntry.Id,
                    ForegroundIconPartId = (ushort)Standard.ForegroundIcon.GuildStandardPartEntry.Id,
                    ScanLinesPartId      = (ushort)Standard.ScanLines.GuildStandardPartEntry.Id
                });
            }

            if (saveMask != GuildSaveMask.None)
            {
                var model = new GuildDataModel
                {
                    Id = Id
                };

                EntityEntry<GuildDataModel> entity = context.Attach(model);
                if ((saveMask & GuildSaveMask.MessageOfTheDay) != 0)
                {
                    model.MessageOfTheDay = MessageOfTheDay;
                    entity.Property(p => p.MessageOfTheDay).IsModified = true;
                }

                if ((saveMask & GuildSaveMask.AdditionalInfo) != 0)
                {
                    model.AdditionalInfo = AdditionalInfo;
                    entity.Property(p => p.AdditionalInfo).IsModified = true;
                }

                saveMask = GuildSaveMask.None;
            }
        }

        public override GuildData Build()
        {
            return new GuildData
            {
                GuildId           = Id,
                GuildName         = Name,
                Flags             = Flags,
                Type              = Type,
                Ranks             = GetGuildRanksPackets().ToList(),
                GuildStandard     = Standard.Build(),
                MemberCount       = (uint)members.Count,
                OnlineMemberCount = (uint)onlineMembers.Count,
                GuildInfo =
                {
                    MessageOfTheDay         = MessageOfTheDay,
                    GuildInfo               = AdditionalInfo,
                    GuildCreationDateInDays = (float)DateTime.Now.Subtract(CreateTime).TotalDays * -1f
                }
            };
        }

        protected override void MemberOnline(GuildMember member)
        {
            if (member.Rank.HasPermission(GuildRankPermission.MemberChat))
                memberChannel.AddMember(member.CharacterId);
            if (member.Rank.HasPermission(GuildRankPermission.OfficerChat))
                officerChannel.AddMember(member.CharacterId);

            base.MemberOnline(member);
        }

        protected override void MemberOffline(GuildMember member)
        {
            if (member.Rank.HasPermission(GuildRankPermission.MemberChat))
                memberChannel.RemoveMember(member.CharacterId);
            if (member.Rank.HasPermission(GuildRankPermission.OfficerChat))
                officerChannel.RemoveMember(member.CharacterId);

            base.MemberOffline(member);
        }
    }
}
