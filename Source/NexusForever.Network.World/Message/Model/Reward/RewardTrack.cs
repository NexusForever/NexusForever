using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    public class RewardTrack : IWritable
    {
        public uint RewardTrackId { get; set; }
        public uint RewardPointsEarned { get; set; }
        public NetworkBitArray ClaimedRewards { get; set; } = new NetworkBitArray(32, NetworkBitArray.BitOrder.LeastSignificantBit);
        public uint Unlocked { get; set; } // needs more research, unclear if this is a bit array or an integer
        public List<uint> RewardTrackRewardsIds { get; set; } = [];
        public bool IsActive { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RewardTrackId, 14u);
            writer.Write(RewardPointsEarned);
            writer.WriteBytes(ClaimedRewards.GetBuffer());
            writer.Write(RewardTrackRewardsIds.Count);
            writer.Write(Unlocked, 32u);
            RewardTrackRewardsIds.ForEach(rewardId => writer.Write(rewardId));
            writer.Write(IsActive);
        }
    }
}
