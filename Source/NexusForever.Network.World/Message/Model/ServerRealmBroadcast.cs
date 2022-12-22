using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmBroadcast)]
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
