using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    //Fires when the unit that is being tracked in a Tracking mission reaches its destination before the player.

    [Message(GameMessageOpcode.ServerPlayerPathExplorerPowerMapWaiting)]
    public class ServerPlayerPathExplorerPowerMapWaiting : IWritable
    {
        public ushort PathMissionId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId, 14);
        }
    }
}
