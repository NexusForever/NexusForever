using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Fires whenever the player fails a Tracking mission.

    [Message(GameMessageOpcode.ServerPlayerPathExplorerPowerMapFailed)]
    public class ServerPlayerPathExplorerPowerMapFailed : IWritable
    {
        public ushort PathMissionId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId, 14);
        }
    }
}
