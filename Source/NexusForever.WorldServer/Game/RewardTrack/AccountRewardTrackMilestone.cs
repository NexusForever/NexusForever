using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.RewardTrack.Static;
using NexusForever.WorldServer.Network;
using System;

namespace NexusForever.WorldServer.Game.RewardTrack
{
    public class AccountRewardTrackMilestone : RewardTrackMilestone, ISaveAuth
    {
        public uint Id { get; }

        /// <summary>
        /// Create an <see cref="AccountRewardTrackMilestone"/> from an existing database model.
        /// </summary>
        public AccountRewardTrackMilestone(AccountRewardTrackMilestoneModel model)
        {
            Id = model.Id;
            RewardTrackId = model.RewardTrackId;
            MilestoneId = model.MilestoneId;
            Entry = GameTableManager.Instance.RewardTrackRewards.GetEntry(model.MilestoneId);
            if (Entry == null)
                throw new ArgumentException(nameof(model.MilestoneId));

            SaveMask = RewardTrackSaveMask.None;
        }
        
        /// <summary>
        /// Create a new <see cref="AccountRewardTrackMilestone"/> from a <see cref="RewardTrackRewardsEntry"/>.
        /// </summary>
        public AccountRewardTrackMilestone(WorldSession session, uint pointsRequired, RewardTrackRewardsEntry entry)
        {
            Id = session.Account.Id;
            Entry = entry;
            RewardTrackId = entry.RewardTrackId;
            MilestoneId = entry.Id;
            PointsRequired = pointsRequired;

            SaveMask = RewardTrackSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="AccountRewardTrackMilestone"/> to the database.
        /// </summary>
        public void Save(AuthContext context)
        {
            if (SaveMask == RewardTrackSaveMask.None)
                return;

            if ((SaveMask & RewardTrackSaveMask.Create) != 0)
            {
                var model = new AccountRewardTrackMilestoneModel
                {
                    Id = Id,
                    RewardTrackId = RewardTrackId,
                    MilestoneId = MilestoneId,
                    PointsRequired = PointsRequired,
                    Choice = Choice
                };
                context.Add(model);
            }
            else if ((SaveMask & RewardTrackSaveMask.Delete) != 0)
            {
                var model = new AccountRewardTrackMilestoneModel
                {
                    Id = Id,
                    RewardTrackId = RewardTrackId,
                    MilestoneId = MilestoneId
                };
                context.Entry(model).State = EntityState.Deleted;
            }
            else
            {
                var model = new AccountRewardTrackMilestoneModel
                {
                    Id = Id,
                    RewardTrackId = RewardTrackId,
                    MilestoneId = MilestoneId,
                    PointsRequired = PointsRequired
                };

                if ((SaveMask & RewardTrackSaveMask.Modify) != 0)
                {
                    EntityEntry<AccountRewardTrackMilestoneModel> entity = context.Attach(model);
                    entity.Property(p => p.Choice).IsModified = true;
                }
            }

            SaveMask = RewardTrackSaveMask.None;
        }

        /// <summary>
        /// Grants all rewards that are associated with this <see cref="AccountRewardTrackMilestone"/>.
        /// </summary>
        public void GrantAllRewards(WorldSession session)
        {
            SetChoice(0);

            for (int i = 0; i < Entry.RewardChoiceIds.Length; i++)
            {
                if (Entry.RewardChoiceIds[i] == 0)
                    continue;

                GrantItemReward(session, (RewardTrackRewardType)Entry.RewardTrackRewardTypeEnums[i], Entry.RewardChoiceIds[i], Entry.RewardChoiceCounts[i]);
            }

            GrantCurrencyReward(session);
        }
    }
}
