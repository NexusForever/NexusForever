using NexusForever.Game.Static.Reward;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    public class LockedReward : IWritable
    {
        public RewardRotationContentType ContentType { get; set; }
        public uint RewardRotationContentId { get; set; }
        public uint RewardId { get; set; } // RewardRotationItemId, RewardRotationEssenceId, RewardRotationModifierId
        public RewardRotationRewardType RewardType { get; set; }
        public GrantedRewardFlags RewardedFlags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ContentType, 3u);
            writer.Write(RewardRotationContentId);
            writer.Write(RewardId);
            writer.Write(RewardType, 8u);
            writer.Write(RewardedFlags);
        }
    }
}
