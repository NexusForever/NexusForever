using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RewardTrack.Static;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.RewardTrack
{
    public class CharacterRewardTrackMilestone : RewardTrackMilestone, ISaveCharacter
    {
        public ulong Id { get; }

        /// <summary>
        /// Create a <see cref="CharacterRewardTrackMilestone"/> from an existing database model.
        /// </summary>
        public CharacterRewardTrackMilestone(CharacterRewardTrackMilestoneModel model)
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
        /// Create a new <see cref="CharacterRewardTrackMilestone"/>.
        /// </summary>
        public CharacterRewardTrackMilestone(Player player, uint pointsRequired, RewardTrackRewardsEntry entry, int choice = -1)
        {
            Id = player.CharacterId;
            Entry = entry;
            RewardTrackId = entry.RewardTrackId;
            MilestoneId = entry.Id;
            PointsRequired = pointsRequired;
            Choice = choice;

            if (Choice > 0)
                ChooseReward(player.Session, choice);

            SaveMask = RewardTrackSaveMask.Create;
        }

        /// <summary>
        /// Save this <see cref="CharacterRewardTrackMilestone"/> to the database.
        /// </summary>
        public void Save(CharacterContext context)
        {
            if (SaveMask == RewardTrackSaveMask.None)
                return;

            if ((SaveMask & RewardTrackSaveMask.Create) != 0)
            {
                var model = new CharacterRewardTrackMilestoneModel
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
                var model = new CharacterRewardTrackMilestoneModel
                {
                    Id = Id,
                    RewardTrackId = RewardTrackId,
                    MilestoneId = MilestoneId
                };
                context.Entry(model).State = EntityState.Deleted;
            }
            else
            {
                var model = new CharacterRewardTrackMilestoneModel
                {
                    Id = Id,
                    RewardTrackId = RewardTrackId,
                    MilestoneId = MilestoneId,
                    PointsRequired = PointsRequired
                };

                if ((SaveMask & RewardTrackSaveMask.Modify) != 0)
                {
                    EntityEntry<CharacterRewardTrackMilestoneModel> entity = context.Attach(model);
                    entity.Property(p => p.Choice).IsModified = true;
                }
            }

            SaveMask = RewardTrackSaveMask.None;
        }
    }
}
