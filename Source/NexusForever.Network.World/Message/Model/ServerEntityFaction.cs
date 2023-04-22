using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityFaction)]
    public class ServerEntityFaction : IWritable
    {
        public uint UnitId { get; set; }
        public ushort Faction { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Faction, 14u);
        }
    }
}
