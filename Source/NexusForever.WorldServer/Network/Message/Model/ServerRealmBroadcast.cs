using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmBroadcast, MessageDirection.Server)]
    public class ServerRealmBroadcast : IWritable
    {
        public BroadcastTier Tier { get; set; }
        public string Message { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Tier, 2);
            writer.WriteStringWide(Message);
        }
    }
}
