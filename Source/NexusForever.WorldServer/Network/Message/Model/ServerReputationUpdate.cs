using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Reputation.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerReputationUpdate)]
    public class ServerReputationUpdate : IWritable
    {
        public Faction FactionId { get; set; }
        public float Value { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FactionId, 14u);
            writer.Write(Value);
        }
    }
}
