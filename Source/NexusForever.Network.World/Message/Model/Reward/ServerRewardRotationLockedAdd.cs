using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    [Message(GameMessageOpcode.ServerRewardRotationLockedAdd)]
    public class ServerRewardRotationLockedAdd : IWritable
    {
        public LockedReward Reward { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Reward.Write(writer);
        }
    }
}
