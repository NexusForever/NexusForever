using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerReputationUpdate, MessageDirection.Server)]
    public class ServerReputationUpdate : IWritable
    {
        public ushort FactionId { get; set; }
        public float Value { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FactionId, 14);
            writer.Write(Value);
        }
    }
}
