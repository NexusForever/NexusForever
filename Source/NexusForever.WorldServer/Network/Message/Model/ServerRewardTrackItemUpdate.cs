using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.RewardTrack.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRewardTrackItemUpdate)]
    public class ServerRewardTrackItemUpdate : IWritable
    {
        public ushort RewardTrackId { get; set; } // 14
        public uint PointsEarned { get; set; }
        public RewardPointFlag RewardsGranted { get; set; }
        public uint Unknown1 { get; set; }
        public bool Active { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RewardTrackId, 14u);
            writer.Write(PointsEarned);
            writer.Write(RewardsGranted, 32u);
            writer.Write(Unknown1);
            writer.Write(Active);
        }
    }
}
