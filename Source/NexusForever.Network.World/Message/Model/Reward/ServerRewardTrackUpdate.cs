using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    [Message(GameMessageOpcode.ServerRewardTrackUpdate)]
    public class ServerRewardTrackUpdate : IWritable
    {
        public uint RewardTrackId { get; set; }
        public uint RewardPointsEarned { get; set; }
        public NetworkBitArray ClaimedRewards { get; set; } = new NetworkBitArray(32, NetworkBitArray.BitOrder.LeastSignificantBit);
        public uint Unlocked { get; set; } // needs more research, unclear if this is a bit array or an integer
        public bool IsActive { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RewardTrackId, 14u);
            writer.Write(RewardPointsEarned);
            writer.WriteBytes(ClaimedRewards.GetBuffer());
            writer.Write(Unlocked);
            writer.Write(IsActive);
        }
    }
}
