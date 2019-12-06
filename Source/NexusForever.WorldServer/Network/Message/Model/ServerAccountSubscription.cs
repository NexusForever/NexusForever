using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountSubscription)]
    public class ServerAccountSubscription : IWritable
    {
        public uint Unknown0 { get; set; }
        public bool SubscriptionExpires { get; set; }
        public float TimeLeft { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(SubscriptionExpires);
            writer.Write(TimeLeft);
        }
    }
}
