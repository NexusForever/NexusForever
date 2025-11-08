using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
