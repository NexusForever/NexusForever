using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Sent to trigger the SetterHubReward event on the client.
    [Message(GameMessageOpcode.ServerPathSettlerHubReward)]
    public class ServerPathSettlerHubReward : IWritable
    {
        public uint PathSettlerHubId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSettlerHubId, 14);
        }
    }
}
