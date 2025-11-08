using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerRefreshHealthFloor)]
    public class ServerRefreshHealthFloor : IWritable
    {
        public uint UnitId { get; set; }
        public uint HealthFloor { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(HealthFloor);
        }
    }
}
