using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Story
{
    [Message(GameMessageOpcode.ServerUnitInUse)]
    public class ServerUnitInUse : IWritable
    {
        public uint UnitId { get; set; }
        public bool InUse { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(InUse);
        }
    }
}
