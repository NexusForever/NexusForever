using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRewardTracksLoaded)]
    public class ServerRewardTracksLoaded : IWritable
    {
        public List<ServerRewardTrack> RewardTracks { get; set; } = new List<ServerRewardTrack>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RewardTracks.Count, 32u);
            RewardTracks.ForEach(r => r.Write(writer));
        }
    }
}
