using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathMissionActivate)]
    public class ServerPathMissionActivate : IWritable
    {
        public class Mission : IWritable
        {
            public uint PathMissionId { get; set; }
            public bool Completed { get; set; }
            public uint ObjectiveCompletionFlags { get; set; } // 
            public uint StateFlags { get; set; } // 
            public byte MissionStateEnum { get; set; } 
            public uint GiverUnitId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(PathMissionId, 15);
                writer.Write(Completed);
                writer.Write(ObjectiveCompletionFlags);
                writer.Write(StateFlags);
                writer.Write(MissionStateEnum, 3);
                writer.Write(GiverUnitId);
            }
        }

        public List<Mission> Missions { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Missions.Count);
            Missions.ForEach(e => e.Write(writer));
        }
    }
}
