using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    // Can only update rewards already added via LockedList or LockedAdd
    [Message(GameMessageOpcode.ServerRewardRotationLockedUpdate)]
    public class ServerRewardRotationLockedUpdate : IWritable
    {
        public LockedReward Reward { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Reward.Write(writer);
        }
    }
}
