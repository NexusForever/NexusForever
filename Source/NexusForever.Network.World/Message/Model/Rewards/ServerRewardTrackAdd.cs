using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Rewards
{
    [Message(GameMessageOpcode.ServerRewardTrackAdd)]
    public class ServerRewardTrackAdd : IWritable
    {
        public RewardTrack RewardTrack { get; set; }

        public void Write(GamePacketWriter writer)
        {
            RewardTrack.Write(writer);
        }
    }
}
