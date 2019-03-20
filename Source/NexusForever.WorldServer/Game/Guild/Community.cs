using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuildBaseModel = NexusForever.Database.Character.Model.Guild;
using GuildDataModel = NexusForever.Database.Character.Model.GuildData;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;

namespace NexusForever.WorldServer.Game.Guild
{
    public class Community : GuildBase, IGuild
    {
        private GuildSaveMask communitySaveMask;

        /// <summary>
        /// Create a new <see cref="Community"/> using <see cref="GuildBaseModel"/>
        /// </summary>
        public Community(GuildBaseModel baseModel) 
            : base (GuildType.Community, baseModel)
        {
            communitySaveMask = GuildSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Community"/> given necessary parameters
        /// </summary>
        public Community(WorldSession leaderSession, string guildName, string leaderRankName, string councilRankName, string memberRankName)
            : base(GuildType.Community)
        {
            Name = guildName;
            LeaderId = leaderSession.Player.CharacterId;

            // Add Default Ranks & Assign Default Permissions for Guild
            AddRank(new Rank(leaderRankName, Id, 0, GuildRankPermission.Leader, ulong.MaxValue, long.MaxValue, long.MaxValue));
            AddRank(new Rank(councilRankName, Id, 1, (GuildRankPermission.OfficerChat | GuildRankPermission.MemberChat | GuildRankPermission.Kick | GuildRankPermission.Invite | GuildRankPermission.ChangeMemberRank | GuildRankPermission.Vote), ulong.MaxValue, long.MaxValue, long.MaxValue));
            AddRank(new Rank(memberRankName, Id, 9, GuildRankPermission.MemberChat, 0, 0, 0));

            Player player = leaderSession.Player;
            Member Leader = new Member(Id, player.CharacterId, GetRank(0), "", this);
            AddMember(Leader);
            OnlineMembers.Add(Leader.CharacterId);

            communitySaveMask = GuildSaveMask.Create;
            base.saveMask = GuildBaseSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="Community"/> to a <see cref="GuildBaseModel"/>. Deletion should be handled by <see cref="GuildBase"/> & Foreign Keys.
        /// </summary>
        public override void Save(CharacterContext context)
        {
            if ((base.saveMask & GuildBaseSaveMask.Delete) != 0)
            {
                base.Save(context);
                return;
            }

            base.Save(context);

            if (communitySaveMask != GuildSaveMask.None)
            {
                if ((communitySaveMask & GuildSaveMask.Create) != 0)
                {
                    //context.Add(new GuildDataModel
                    //{
                    //    Id = Id,
                    //});
                }
                else
                {
                    var model = new GuildDataModel
                    {
                        Id = Id
                    };

                    //EntityEntry<GuildDataModel> entity = context.Attach(model);
                }

                communitySaveMask = GuildSaveMask.None;
            }
        }

        /// <summary>
        /// Return a <see cref="GuildData"/> packet of this <see cref="Community"/>
        /// </summary>
        public override GuildData BuildGuildDataPacket()
        {
            return new GuildData
            {
                GuildId = Id,
                GuildName = Name,
                Type = Type,
                Ranks = GetGuildRanksPackets().ToList(),
                MemberCount = (uint)members.Count,
                OnlineMemberCount = (uint)OnlineMembers.Count,
                GuildInfo =
                {
                    GuildCreationDateInDays = (float)DateTime.Now.Subtract(CreateTime).TotalDays * -1f
                }
            };
        }
    }
}
