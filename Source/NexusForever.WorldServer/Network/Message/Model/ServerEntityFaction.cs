using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
