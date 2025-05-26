using NexusForever.Game.Static.Challenges;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerChallengeUpdate)]
    public class ServerChallengeUpdate : IWritable
    {
        public class ActiveChallenge : IWritable
        {
            public uint ChallengeId { get; set; }
            public ChallengeType Type { get; set; }
            public uint Field_8 { get; set; }
            public uint Field_C { get; set; }
            public uint Field_10 { get; set; }
            public uint CurrentCount { get; set; }
            public uint TotalCount { get; set; }
            public uint ObjectiveCompletion { get; set; } // Sometimes a number, sometimes bit flags. Depends on challege.challengeFlags in tbl
            public uint MaxAchievedChallengeTier { get; set; }
            public uint LastRewardTierCompleted { get; set; }
            public uint CompletionCount { get; set; }
            public bool Unlocked { get; set; }
            public bool Active { get; set; }
            public bool OnCooldown { get; set; }
            public bool HasLeftArea { get; set; }
            public uint TimeRemaining { get; set; }
            public uint DurationMs { get; set; }
            public uint CooldownRemaining { get; set; }
            public uint CooldownDurationMs { get; set; }
            public uint LeftAreaTimeRemaining { get; set; }
            public uint LeftAreaTimeoutMs { get; set; }
            public uint[] ChallengeTierGoalCount { get; set; } = new uint[3];

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ChallengeId, 14u);
                writer.Write(Type, 4u);
                writer.Write(Field_8);
                writer.Write(Field_C);
                writer.Write(Field_10);
                writer.Write(CurrentCount);
                writer.Write(TotalCount);
                writer.Write(ObjectiveCompletion);
                writer.Write(MaxAchievedChallengeTier);
                writer.Write(LastRewardTierCompleted);
                writer.Write(CompletionCount);
                writer.Write(Unlocked);
                writer.Write(Active);
                writer.Write(OnCooldown);
                writer.Write(HasLeftArea);
                writer.Write(TimeRemaining);
                writer.Write(DurationMs);
                writer.Write(CooldownRemaining);
                writer.Write(CooldownDurationMs);
                writer.Write(LeftAreaTimeRemaining);
                writer.Write(LeftAreaTimeoutMs);
                foreach (var tier in ChallengeTierGoalCount)
                {
                    writer.Write(tier);
                }
            }
        }

        List<ActiveChallenge> ActiveChallenges { get; set; } = [];

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
