using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMarkUnit)]
    public class ServerGroupMarkUnit : IWritable
    {
        public ulong GroupId { get; set; }

        public GroupMarker Marker { get; set; }

        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(Marker, 32u);
            writer.Write(UnitId);
        }
    }
}
