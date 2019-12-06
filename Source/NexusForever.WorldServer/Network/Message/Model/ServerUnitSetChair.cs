using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitSetChair)]
    public class ServerUnitSetChair : IWritable
    {
        public uint UnitId { get; set; }
        public uint UnitIdChair { get; set; }
        public bool WaitForUnit { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(UnitIdChair);
            writer.Write(WaitForUnit);
        }
    }
}
