using NexusForever.Game.Static.PlayerPath;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    public class Mission : IWritable
    {
        public uint PathMissionId { get; set; }
        public bool Completed { get; set; }
        public uint ObjectiveCompletionFlags { get; set; }
        public uint StateFlags { get; set; }
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
}
