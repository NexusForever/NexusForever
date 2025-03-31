using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // ires whenever the player successfully starts a Tracking mission.

    [Message(GameMessageOpcode.ServerPlayerPathExplorerPowerMapStarted)]
    public class ServerPlayerPathExplorerPowerMapStarted : IWritable
    {
        public ushort PathMissionId { get; set; }
        public uint UnitId { get; set; } // Unit that the player is tracking for the mission.

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId, 14);
            writer.Write(UnitId);
        }
    }
}
