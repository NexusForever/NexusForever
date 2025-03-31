using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Fires whenever progress is made on a multi-step path mission.
    [Message(GameMessageOpcode.ServerPlayerPathMissionAdvanced)]
    public class ServerPlayerPathMissionAdvanced : IWritable
    {
        public ushort PathMissionId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId, 14);
        }
    }
}
