using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathMissionUpdate)]
    public class ServerPathMissionUpdate : IWritable
    {
        public ushort PathMissionId { get; set; }
        public bool Completed { get; set; }
        public uint ObjectiveCompletionFlags { get; set; }
        public uint StateFlags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId, 15);
            writer.Write(Completed);
            writer.Write(ObjectiveCompletionFlags);
            writer.Write(StateFlags);
        }
    }
}
