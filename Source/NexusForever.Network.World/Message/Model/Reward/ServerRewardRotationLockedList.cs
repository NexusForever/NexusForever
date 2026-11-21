using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    [Message(GameMessageOpcode.ServerRewardRotationLockedList)]
    public class ServerRewardRotationLockedList : IWritable
    {
        public List<LockedReward> Rewards { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Rewards.Count);
            Rewards.ForEach(reward => reward.Write(writer));
        }
    }
}
