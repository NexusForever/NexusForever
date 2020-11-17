using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerChallengeUpdate)]
    public class ServerChallengeUpdate : IWritable
    {
        public class Challenge : IWritable
        {
            public ushort ChallengeId { get; set; } // 14
            public byte Type { get; set; } // 4
            public uint ActionId { get; set; }
            public uint QualifyCount { get; set; }
            public uint QualifyTotal { get; set; }
            public uint CurrentCount { get; set; }
            public uint GoalCount { get; set; }
            public uint ExtraData { get; set; }
            public uint CurrentTier { get; set; }
            public uint LastRewardTier { get; set; }
            public uint CompletionCount { get; set; }
            public bool BUnlocked { get; set; }
            public bool BActivated { get; set; }
            public bool BInCooldown { get; set; }
            public bool BLeftArea { get; set; }
            public uint TimeActivatedDt { get; set; }
            public uint TimeTotalActive { get; set; }
            public uint TimeCooldownDt { get; set; }
            public uint TimeTotalCooldown { get; set; }
            public uint TimeAreaFailDt { get; set; }
            public uint TimeTotalAreaFail { get; set; }
            public uint[] TierCounts { get; set; } = new uint[3];
                    
            public void Write(GamePacketWriter writer)
            {
                writer.Write(ChallengeId, 14u);
                writer.Write(Type, 4u);
                writer.Write(ActionId);
                writer.Write(QualifyCount);
                writer.Write(QualifyTotal);
                writer.Write(CurrentCount);
                writer.Write(GoalCount);
                writer.Write(ExtraData);
                writer.Write(CurrentTier);
                writer.Write(LastRewardTier);
                writer.Write(CompletionCount);
                writer.Write(BUnlocked);
                writer.Write(BActivated);
                writer.Write(BInCooldown);
                writer.Write(BLeftArea);
                writer.Write(TimeActivatedDt);
                writer.Write(TimeTotalActive);
                writer.Write(TimeCooldownDt);
                writer.Write(TimeTotalCooldown);
                writer.Write(TimeAreaFailDt);
                writer.Write(TimeTotalAreaFail);

                for (int i = 0; i < 3; i++)
                    writer.Write(TierCounts[i]);
            }
        }

        public List<Challenge> Challenges { get; set; } = new List<Challenge>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Challenges.Count);
            Challenges.ForEach(c => c.Write(writer));
        }
    }
}
