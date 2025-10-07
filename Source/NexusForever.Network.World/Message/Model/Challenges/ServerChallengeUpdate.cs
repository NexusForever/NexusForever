using NexusForever.Game.Static.Challenges;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Challenges
{
    [Message(GameMessageOpcode.ServerChallengeUpdate)]
    public class ServerChallengeUpdate : IWritable
    {
        public class Challenge : IWritable
        {
            public uint ChallengeId { get; set; }
            public ChallengeType Type { get; set; }
            public uint TargetGroupId { get; set; } // See tbl targetGroup
            public uint QualifyCount { get; set; }
            public uint QualityTotal { get; set; }
            public uint CurrentCount { get; set; }
            public uint GoalCount { get; set; }
            public uint ObjectiveCompletion { get; set; } // Sometimes a number, sometimes bit flags. Depends on challege.challengeFlags in tbl
            public uint CurrentTier { get; set; }
            public uint LastRewardTier { get; set; }
            public uint CompletionCount { get; set; }
            public bool Unlocked { get; set; }
            public bool Activated { get; set; }
            public bool OnCooldown { get; set; }
            public bool LeftArea { get; set; }
            public uint TimeActivatedDt { get; set; }
            public uint TimeTotalActive { get; set; } // varies, most commonly 5 minutes
            public uint TimeCooldownDt { get; set; }
            public uint TimeTotalCooldown { get; set; } // typically 30 minutes
            public uint TimeAreaFailDt { get; set; }
            public uint TimeTotalAreaFail { get; set; } // typically 10 seconds
            public uint[] TierGoalCount { get; set; } = new uint[3];

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ChallengeId, 14u);
                writer.Write(Type, 4u);
                writer.Write(TargetGroupId);
                writer.Write(QualifyCount);
                writer.Write(QualityTotal);
                writer.Write(CurrentCount);
                writer.Write(GoalCount);
                writer.Write(ObjectiveCompletion);
                writer.Write(CurrentTier);
                writer.Write(LastRewardTier);
                writer.Write(CompletionCount);
                writer.Write(Unlocked);
                writer.Write(Activated);
                writer.Write(OnCooldown);
                writer.Write(LeftArea);
                writer.Write(TimeActivatedDt);
                writer.Write(TimeTotalActive);
                writer.Write(TimeCooldownDt);
                writer.Write(TimeTotalCooldown);
                writer.Write(TimeAreaFailDt);
                writer.Write(TimeTotalAreaFail);
                foreach (var tier in TierGoalCount)
                {
                    writer.Write(tier);
                }
            }
        }

        public List<Challenge> ActiveChallenges { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ActiveChallenges.Count);
            foreach (var challenge in ActiveChallenges)
            {
                challenge.Write(writer);
            }
        }
    }
}
