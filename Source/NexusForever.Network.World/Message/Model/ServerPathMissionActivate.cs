using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathMissionActivate)]
    public class ServerPathMissionActivate : IWritable
    {
        public class Mission : IWritable
        {
            public enum PathMissionState
            {
                NoMission = 0x0,
                Unlocked = 0x2,
                Started = 0x3,
                Complete = 0x4,
            };

            public uint PathMissionId { get; set; }
            public bool Completed { get; set; }
            public uint ObjectiveCompletionFlags { get; set; } // 
            public uint StateFlags { get; set; } // 
            public PathMissionState State { get; set; } 
            public uint GiverUnitId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(PathMissionId, 15);
                writer.Write(Completed);
                writer.Write(ObjectiveCompletionFlags);
                writer.Write(StateFlags);
                writer.Write(State, 3);
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
