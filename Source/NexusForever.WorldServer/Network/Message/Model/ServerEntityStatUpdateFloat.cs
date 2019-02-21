using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityStatUpdateFloat)]
    public class ServerEntityStatUpdateFloat : IWritable
    {
        public uint UnitId { get; set; }
        public StatValue Stat { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            Stat.WriteUpdate(writer);
        }
    }
}
