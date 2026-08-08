using NexusForever.Game.Static.Reward;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    [Message(GameMessageOpcode.ServerRewardRotations)]
    public class ServerRewardRotations : IWritable
    {
        public class RewardRotation : IWritable
        {
            public uint RotationId { get; set; } // Appears to be an ID used to denote a reward offered for some rotation period
                                             // Values seen vary been 0 and ~5600
                                             // Seen multiple times over time with different values in the RewardInfo
                                             // Not certain how this should be created/managed 
            public uint RewardRotationContentId { get; set; }
            public float ExpirationTime { get; set; } // in days from now
            public RewardRotationRewardType RewardType { get; set; }
            public uint RewardId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(RotationId);
                writer.Write(RewardRotationContentId, 14u);
                writer.Write(ExpirationTime);
                writer.Write(RewardType, 8u);
                writer.Write(RewardId);
            }
        }

        public List<RewardRotation> RewardRotations { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RewardRotations.Count);
            RewardRotations.ForEach(reward => reward.Write(writer));
        }
    }
}
