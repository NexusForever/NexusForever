using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Rewards
{
    // Clears all existing RewardTrack data then loads it from this message
    // Triggers RewardTrackedLoaded event
    [Message(GameMessageOpcode.ServerRewardTracksReload)]
    public class ServerRewardTracksReload : IWritable
    {
        public List<RewardTrack> RewardsTracks { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RewardsTracks.Count);
            RewardsTracks.ForEach(rewardTrack => rewardTrack.Write(writer));
        }
    }
}
