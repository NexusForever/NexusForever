using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Map.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
