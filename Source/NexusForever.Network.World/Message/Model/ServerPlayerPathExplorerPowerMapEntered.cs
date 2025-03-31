using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Fires whenever the player enters an area where a Tracking mission can be started

    [Message(GameMessageOpcode.ServerPlayerPathExplorerPowerMapEntered)]
    public class ServerPlayerPathExplorerPowerMapEntered : IWritable
    {
        public ushort PathMissionId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId, 14);
        }
    }
}
