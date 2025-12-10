using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    [Message(GameMessageOpcode.ClientRewardTrackClaimRewardPoint)]
    public class ClientRewardTrackClaimRewardPoint : IReadable
    {
        public ushort RewardTrackRewardsId { get; private set; }
        public uint ChoiceIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            RewardTrackRewardsId = reader.ReadUShort(14u);
            ChoiceIndex = reader.ReadUInt();
        }
    }
}
