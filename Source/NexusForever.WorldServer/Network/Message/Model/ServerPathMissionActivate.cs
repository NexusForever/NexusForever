using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPathMissionActivate)]
    public class ServerPathMissionActivate : IWritable
    {
        public class Mission : IWritable
        {
            public uint MissionId { get; set; }
            public bool Completed { get; set; }
            public uint Userdata { get; set; } // % of mission progress
            public uint Statedata { get; set; } // 
            public byte Reason { get; set; } 
            public uint Giver { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(MissionId, 15);
                writer.Write(Completed);
                writer.Write(Userdata);
                writer.Write(Statedata);
                writer.Write(Reason, 3);
                writer.Write(Giver);
            }
        }

        public List<Mission> Missions { get; set; } = new List<Mission>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Missions.Count);
            Missions.ForEach(e => e.Write(writer));
        }
    }
}
