using NexusForever.Game.Static.Map;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPendingWorldRemoval)]
    public class ServerPendingWorldRemoval : IWritable
    {
        public WorldRemovalReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Reason, 3u);
        }
    }
}
