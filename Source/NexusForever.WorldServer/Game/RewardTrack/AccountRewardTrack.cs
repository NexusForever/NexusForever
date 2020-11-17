using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.RewardTrack.Static;
using NexusForever.WorldServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.RewardTrack
{
    public class AccountRewardTrack : RewardTrack, ISaveAuth
    {
        public uint Id { get; }

        /// <summary>
        /// Create a new <see cref="AccountRewardTrack"/> from an existing database model.
        /// </summary>
        public AccountRewardTrack(RewardTrackEntry entry, AccountRewardTrackModel model)
        {
            Id = model.Id;
            RewardTrackId = model.RewardTrackId;
            Points = model.Points;
            Entry = entry;

            if (model.Milestone.Count < Entry.RewardPointCosts.Where(x => x != 0).Count())
                throw new InvalidOperationException($"Database Model contains more Reward Tiers than there are in the GameTable!");

            foreach (AccountRewardTrackMilestoneModel milestoneModel in model.Milestone)
            {
                if (!Milestones.ContainsKey(milestoneModel.PointsRequired))
                    Milestones.Add(milestoneModel.PointsRequired, new List<RewardTrackMilestone>());

                Milestones[milestoneModel.PointsRequired].Add(new AccountRewardTrackMilestone(milestoneModel));
            }
        }

        /// <summary>
        /// Create a new <see cref="AccountRewardTrack"/> from a <see cref="RewardTrackEntry"/>.
        /// </summary>
        public AccountRewardTrack(WorldSession session, RewardTrackEntry entry)
        {
            Id = session.Account.Id;
            Entry = entry;
            RewardTrackId = entry.Id;

            // All Account Rewards only have 1 option at each Tier and can only be completed once; they never reset. As a result, we only "pick" the first entry at each tier when building Reward Track Milestones for an Account Reward Track.
            IEnumerable<RewardTrackRewardsEntry> allRewardMilestones = GameTableManager.Instance.RewardTrackRewards.Entries.Where(i => i.RewardTrackId == RewardTrackId);

            List<RewardTrackRewardsEntry> actualRewardMilestones = new List<RewardTrackRewardsEntry>();
            int eligibleIndex = 0;
            for (int i = 0; i < Entry.RewardPointCosts.Length; i++)
            {
                if (Entry.RewardPointCosts[eligibleIndex] == 0)
                {
                    eligibleIndex++;
                    continue;
                }

                int allowedRewardPointFlags = 1 << eligibleIndex;
                List<RewardTrackRewardsEntry> eligibleRewardMilestones = allRewardMilestones
                    .Where(m => (m.RewardPointFlags & allowedRewardPointFlags) != 0)
                    .ToList();

                if (eligibleRewardMilestones.Count == 0)
                {
                    eligibleIndex++;
                    continue;
                }

                actualRewardMilestones.Add(eligibleRewardMilestones[new Random().Next(eligibleRewardMilestones.Count)]);
                eligibleIndex++;
            }

            uint index = 0;
            foreach (RewardTrackRewardsEntry rewardsEntry in actualRewardMilestones)
            {
                if (!Milestones.ContainsKey(Entry.RewardPointCosts[index]))
                    Milestones.Add(Entry.RewardPointCosts[index], new List<RewardTrackMilestone>());

                Milestones[Entry.RewardPointCosts[index]].Add(new AccountRewardTrackMilestone(session, Entry.RewardPointCosts[index], rewardsEntry));

                index++;
            }

            SaveMask = RewardTrackSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="AccountRewardTrack"/> to the database.
        /// </summary>
        public void Save(AuthContext context)
        {
            if (SaveMask != RewardTrackSaveMask.None)
            {
                if ((SaveMask & RewardTrackSaveMask.Create) != 0)
                {
                    var model = new AccountRewardTrackModel
                    {
                        Id = Id,
                        RewardTrackId = RewardTrackId,
                        Points = Points
                    };
                    context.Add(model);
                }
                else if ((SaveMask & RewardTrackSaveMask.Delete) != 0)
                {
                    var model = new AccountRewardTrackModel
                    {
                        Id = Id,
                        RewardTrackId = RewardTrackId
                    };
                    context.Entry(model).State = EntityState.Deleted;
                }
                else
                {
                    var model = new AccountRewardTrackModel
                    {
                        Id = Id,
                        RewardTrackId = RewardTrackId
                    };
                    
                    EntityEntry<AccountRewardTrackModel> entity = context.Attach(model);

                    if ((SaveMask & RewardTrackSaveMask.Modify) != 0)
                    {
                        model.Points = Points;
                        entity.Property(p => p.Points).IsModified = true;
                    }
                }

                SaveMask = RewardTrackSaveMask.None;
            }

            foreach (AccountRewardTrackMilestone milestone in DeletedMilestones)
                milestone.Save(context);

            DeletedMilestones.Clear();

            foreach (AccountRewardTrackMilestone milestone in Milestones.Values.SelectMany(x => x))
                milestone.Save(context);
        }

        /// <summary>
        /// Adds Points to this <see cref="AccountRewardTrack"/> and grant any rewards to the user as appropriate. Should only be called by a <see cref="RewardTrackManager"/> handler.
        /// </summary>
        public override void AddPoints(WorldSession session, ulong amount)
        {
            ulong previousPoints = Points;
            base.AddPoints(session, amount);

            // Account Rewards check automatically.
            foreach (uint pointsNeeded in Entry.RewardPointCosts.Where(i => i > previousPoints && i <= Points))
                if (Milestones.TryGetValue(pointsNeeded, out List<RewardTrackMilestone> milestoneList))
                {
                    foreach (RewardTrackMilestone milestone in milestoneList)
                    {
                        if (!(milestone is AccountRewardTrackMilestone accountMilestone))
                            throw new InvalidOperationException("Selected RewardTrackMilestone is not of the right type!");

                        accountMilestone.GrantAllRewards(session);
                    }
                }

            SaveMask |= RewardTrackSaveMask.Modify;
        }
    }
}
