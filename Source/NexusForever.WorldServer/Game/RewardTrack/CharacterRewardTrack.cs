using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.RewardTrack.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.RewardTrack
{
    public class CharacterRewardTrack : RewardTrack, ISaveCharacter
    {
        public ulong Id { get; }

        /// <summary>
        /// Create a new <see cref="CharacterRewardTrack"/> from an existing <see cref="CharacterRewardTrackModel"/>.
        /// </summary>
        public CharacterRewardTrack(RewardTrackEntry entry, CharacterRewardTrackModel model)
        {
            Id = model.Id;
            RewardTrackId = model.RewardTrackId;
            Points = model.Points;
            Entry = entry;

            if (model.Milestone.Count > Entry.RewardPointCosts.Where(x => x != 0).Count())
                throw new InvalidOperationException($"Database Model contains more Reward Tiers than there are in the GameTable!");

            foreach (CharacterRewardTrackMilestoneModel milestoneModel in model.Milestone)
            {
                if (!Milestones.ContainsKey(milestoneModel.PointsRequired))
                    Milestones.Add(milestoneModel.PointsRequired, new List<RewardTrackMilestone>());

                Milestones[milestoneModel.PointsRequired].Add(new CharacterRewardTrackMilestone(milestoneModel));
            }
        }

        /// <summary>
        /// Create a new <see cref="CharacterRewardTrack"/> from a <see cref="RewardTrackEntry"/> for a given <see cref="Player"/>.
        /// </summary>
        public CharacterRewardTrack(Player player, RewardTrackEntry entry)
        {
            Id = player.CharacterId;
            Entry = entry;
            RewardTrackId = entry.Id;

            BuildRewardTrackMilestones(player);

            SaveMask = RewardTrackSaveMask.Create;
        }

        /// <summary>
        /// This method constructs this <see cref="RewardTrack"/>'s <see cref="RewardTrackMilestone"/> list. This allows for both static and dynamic lists.
        /// </summary>
        public void BuildRewardTrackMilestones(Player player)
        {
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

                foreach (RewardTrackRewardsEntry eligibleReward in eligibleRewardMilestones.ToList())
                    if (eligibleReward.PrerequisiteId > 0 && !PrerequisiteManager.Instance.Meets(player, eligibleReward.PrerequisiteId))
                        eligibleRewardMilestones.Remove(eligibleReward);

                actualRewardMilestones.Add(eligibleRewardMilestones[new Random().Next(eligibleRewardMilestones.Count)]);
                eligibleIndex++;
            }

            int milestoneIndex = 0;
            foreach (RewardTrackRewardsEntry rewardsEntry in actualRewardMilestones)
            {
                if (Milestones.ContainsKey(Entry.RewardPointCosts[milestoneIndex]))
                {
                    milestoneIndex++;
                    continue;
                }

                Milestones.Add(Entry.RewardPointCosts[milestoneIndex], new List<RewardTrackMilestone>());
                Milestones[Entry.RewardPointCosts[milestoneIndex]].Add(new CharacterRewardTrackMilestone(player, Entry.RewardPointCosts[milestoneIndex], rewardsEntry));
                
                milestoneIndex++;
            }
        }

        /// <summary>
        /// Save this <see cref="CharacterRewardTrack"/> to the database.
        /// </summary>
        public void Save(CharacterContext context)
        {
            if (SaveMask != RewardTrackSaveMask.None)
            {
                if ((SaveMask & RewardTrackSaveMask.Create) != 0)
                {
                    var model = new CharacterRewardTrackModel
                    {
                        Id = Id,
                        RewardTrackId = RewardTrackId,
                        Points = Points
                    };
                    context.Add(model);
                }
                else if ((SaveMask & RewardTrackSaveMask.Delete) != 0)
                {
                    var model = new CharacterRewardTrackModel
                    {
                        Id = Id,
                        RewardTrackId = RewardTrackId
                    };
                    context.Entry(model).State = EntityState.Deleted;
                }
                else
                {
                    var model = new CharacterRewardTrackModel
                    {
                        Id = Id,
                        RewardTrackId = RewardTrackId
                    };

                    EntityEntry<CharacterRewardTrackModel> entity = context.Attach(model);

                    if ((SaveMask & RewardTrackSaveMask.Modify) != 0)
                    {
                        model.Points = Points;
                        entity.Property(p => p.Points).IsModified = true;
                    }
                }

                SaveMask = RewardTrackSaveMask.None;
            }

            foreach (CharacterRewardTrackMilestone milestone in DeletedMilestones)
                milestone.Save(context);

            DeletedMilestones.Clear();

            foreach (CharacterRewardTrackMilestone milestone in Milestones.Values.SelectMany(x => x))
                milestone.Save(context);
        }
    }
}
