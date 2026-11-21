using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reward
{
    [Message(GameMessageOpcode.ServerRewardTrackRemove)]
    public class ServerRewardTrackRemove : IWritable
    {
        public uint RewardTrackId { get; set; } // if 0 it removes all reward tracks

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RewardTrackId, 14u);
        }
    }
}
