using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathMissionUpdate)]
    public class ServerPathMissionUpdate : IWritable
    {
        public Mission Mission { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Mission.PathMissionId, 15);
            writer.Write(Mission.Completed);
            writer.Write(Mission.ObjectiveCompletionFlags);
            writer.Write(Mission.StateFlags);
        }
    }
}
