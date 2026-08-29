using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reputation
{
    [Message(GameMessageOpcode.ServerReputationOverrideRemove)]
    public class ServerReputationOverrideRemove : IWritable
    {
        public uint UnitId { get; set; } // Must be player's UnitId otherwise message is ignored
        public Faction FactionId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(FactionId, 14u);
        }
    }
}
