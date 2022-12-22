using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
