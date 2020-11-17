using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRewardTrackUpdate)]
    public class ServerRewardTrackUpdate : IWritable
    {
        public ServerRewardTrack RewardTrack { get; set; }

        public void Write(GamePacketWriter writer)
        {
            RewardTrack.Write(writer);
        }
    }
}
