using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerRefreshHealthCeiling)]
    public class ServerRefreshHealthCeiling : IWritable
    {
        public uint UnitId { get; set; }
        public uint HealthCeiling { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(HealthCeiling);
        }
    }
}
