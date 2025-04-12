using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerBuildStatusUpdate)]
    public class ServerPathSettlerBuildStatusUpdate : IWritable
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
