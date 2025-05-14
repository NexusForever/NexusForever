using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Fires whenever the player fails a Tracking mission.

    [Message(GameMessageOpcode.ServerPathExplorerPowerMapFailed)]
    public class ServerPathExplorerPowerMapFailed : IWritable
    {
        public ushort PathMissionId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId, 14);
        }
    }
}
