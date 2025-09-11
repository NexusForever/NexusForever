using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Reputation
{
    // For temporarily overriding a player's reputation value for a specific faction.
    // Does not affect the underlying reputation value sent in ServerReputationUpdate.
    [Message(GameMessageOpcode.ServerReputationOverrideAdd)]
    public class ServerReputationOverrideAdd : IWritable
    {
        public uint UnitId { get; set; } // Must be player's UnitId otherwise message is ignored
        public Faction FactionId { get; set; }
        public float Value { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(FactionId, 14u);
            writer.Write(Value);
        }
    }
}
