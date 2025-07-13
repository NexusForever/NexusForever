using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    [Message(GameMessageOpcode.ServerUnitGroupChanged)]
    public class ServerUnitGroupChanged : IWritable
    {
        public uint UnitId { get; set; }
        public ulong GroupId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(GroupId);
        }
    }
}
