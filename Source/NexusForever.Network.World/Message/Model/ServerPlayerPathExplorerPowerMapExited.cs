using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Fires whenever the player leaves a location where a power map mission can be started.

    [Message(GameMessageOpcode.ServerPlayerPathExplorerPowerMapExited)]
    public class ServerPlayerPathExplorerPowerMapExited : IWritable
    {
        public ushort PathMissionId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId, 14);
        }
    }
}
