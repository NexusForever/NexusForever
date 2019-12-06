using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.RewardTrack.Static;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class ServerRewardTrack : IWritable
    {
        public ushort RewardTrackId { get; set; } // 14
        public uint PointsEarned { get; set; }
        public RewardPointFlag RewardsGranted { get; set; }
        public uint Unknown1 { get; set; }
        public List<uint> Rewards { get; set; }
        public bool Active { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RewardTrackId, 14u);
            writer.Write(PointsEarned);
            writer.Write(RewardsGranted, 32u);
            writer.Write(Rewards.Count, 32u);
            writer.Write(Unknown1);
            Rewards.ForEach(r => writer.Write(r));
            writer.Write(Active);
        }
    }
}
