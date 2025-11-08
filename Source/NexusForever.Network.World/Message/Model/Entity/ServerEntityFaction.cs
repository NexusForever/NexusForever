using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Changes the override faction for the unit. BaseFaction can only be set when the unit is created.
    [Message(GameMessageOpcode.ServerEntityFaction)]
    public class ServerEntityFaction : IWritable
    {
        public uint UnitId { get; set; }
        public Faction Faction2Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Faction2Id, 14u);
        }
    }
}
