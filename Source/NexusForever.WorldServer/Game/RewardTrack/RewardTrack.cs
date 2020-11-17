using NexusForever.Database.Auth;
using NexusForever.Database.Character;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.RewardTrack.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.RewardTrack
{
    public class RewardTrack : IEnumerable<RewardTrackMilestone>
    {
        public uint RewardTrackId { get; protected set; }
        public ulong Points { get; protected set; }

        public RewardTrackEntry Entry { get; protected set; }
        public uint MaximumPoints => Entry.RewardPointCosts.LastOrDefault(i => i != 0);
        public bool IsRepeatable() => (RewardTrackType)Entry.RewardTrackTypeEnum != RewardTrackType.Loyalty;
        public bool IsAccountTrack() => Entry.RewardTrackIdParent == 1;
        public bool IsComplete() => Milestones.Values.SelectMany(x => x).FirstOrDefault(i => i.Choice == -1) == null;

        protected Dictionary</* pointsRequired */ uint, List<RewardTrackMilestone>> Milestones { get; set; } 
            = new Dictionary</* pointsRequires */ uint, List<RewardTrackMilestone>>();
        protected HashSet<RewardTrackMilestone> DeletedMilestones { get; set; } 
            = new HashSet<RewardTrackMilestone>();

        protected RewardTrackSaveMask SaveMask;

        public RewardTrack()
        {
        }

        /// <summary>
        /// Used to add Points to this <see cref="RewardTrack"/>.
        /// </summary>
        public virtual void AddPoints(WorldSession session, ulong amount)
        {
            if (Points + amount > uint.MaxValue)
                Points = uint.MaxValue;
            else
                Points += amount;

            SaveMask |= RewardTrackSaveMask.Modify;

            // Send updates
            SendRewardTrackUpdate(session);
        }

        /// <summary>
        /// Attempts to choose a Reward based on <see cref="RewardTrackMilestone"/> ID and a given choice. Should only occur when the Client has the opportunity to pick.
        /// </summary>
        public void ChooseReward(WorldSession session, uint milestoneId, int choice)
        {
            RewardTrackMilestone milestone = Milestones.Values.SelectMany(x => x).FirstOrDefault(v => v.MilestoneId == milestoneId);
            if (milestone == null)
                throw new InvalidOperationException($"RewardTrackMilestone {milestoneId} not found but should exist.");

            if (Points < milestone.PointsRequired)
                throw new InvalidOperationException($"Not enough Points! Player should not be able to choose a reward.");

            if (!milestone.ChooseReward(session, choice))
                return;

            SendRewardTrackItemUpdate(session);

            if (IsComplete() && IsRepeatable() && session.Player != null && this is CharacterRewardTrack characterRewardTrack)
            {
                AttemptReset();
                characterRewardTrack.BuildRewardTrackMilestones(session.Player);
                SendRewardTrackUpdate(session);
            }
        }

        /// <summary>
        /// Attempts to Reset this <see cref="RewardTrack"/> so that it may be completed again.
        /// </summary>
        public void AttemptReset()
        {
            foreach ((uint index, List<RewardTrackMilestone> milestones) in Milestones)
            {
                foreach (RewardTrackMilestone milestone in milestones)
                {
                    milestone.EnqueueDelete();
                    DeletedMilestones.Add(milestone);
                }
            }

            Milestones.Clear();

            Points -= MaximumPoints;
            SaveMask |= RewardTrackSaveMask.Modify;
        }

        /// <summary>
        /// Returns a packed <see cref="ServerRewardTrack"/> network message that describes this <see cref="RewardTrack"/>.
        /// </summary>
        public ServerRewardTrack BuildNetworkMessage()
        {
            RewardPointFlag rewardsGranted = RewardPointFlag.None;
            foreach (RewardTrackMilestone milestone in Milestones.Values.SelectMany(x => x.Where(y => y.Choice > -1)))
                rewardsGranted |= (RewardPointFlag)milestone.Entry.RewardPointFlags;

            var rewardTracks = new ServerRewardTrack
            {
                RewardTrackId = (ushort)RewardTrackId,
                PointsEarned = (uint)Points,
                Active = Entry.RewardTrackIdParent == 0 || (Points != MaximumPoints && Entry.Flags == 1),
                Rewards = Milestones.OrderBy(x => x.Key)
                    .SelectMany(x => x.Value)
                    .Select(y => y.Entry.Id)
                    .ToList(),
                RewardsGranted = rewardsGranted
            };

            return rewardTracks;
        }

        /// <summary>
        /// Sends a <see cref="ServerRewardTrackUpdate"/> to the <see cref="Session"/> using data from this <see cref="RewardTrack"/>.
        /// </summary>
        private void SendRewardTrackUpdate(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerRewardTrackUpdate
            {
                RewardTrack = BuildNetworkMessage()
            });
        }

        /// <summary>
        /// Sends a <see cref="ServerRewardTrackItemUpdate"/> to the <see cref="Session"/> using data from this <see cref="RewardTrack"/>.
        /// </summary>
        private void SendRewardTrackItemUpdate(WorldSession session)
        {
            RewardPointFlag rewardsGranted = RewardPointFlag.None;
            foreach (RewardTrackMilestone milestone in Milestones.Values.SelectMany(x => x.Where(y => y.Choice > -1)))
                rewardsGranted |= (RewardPointFlag)milestone.Entry.RewardPointFlags;

            session.EnqueueMessageEncrypted(new ServerRewardTrackItemUpdate
            {
                RewardTrackId = (ushort)RewardTrackId,
                PointsEarned = (uint)Points,
                Active = Entry.RewardTrackIdParent == 0 || (Points != MaximumPoints && Entry.Flags == 1),
                RewardsGranted = rewardsGranted
            });
        }

        public IEnumerator<RewardTrackMilestone> GetEnumerator()
        {
            return Milestones.Values.SelectMany(x => x).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
