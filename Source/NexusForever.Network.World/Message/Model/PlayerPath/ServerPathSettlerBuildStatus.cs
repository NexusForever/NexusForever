using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathSettlerBuildStatus)]
    public class ServerPathSettlerBuildStatus : IWritable
    {
        public ushort PathSettlerHubId { get; set; }
        public SettlerImprovementGroupStatus Status { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSettlerHubId, 14);
            Status.Write(writer);
        }
    }
}
