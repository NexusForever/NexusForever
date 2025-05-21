using NexusForever.Network.Message;
using NexusForever.Game.Static.Group;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMarks)]
    public class ServerGroupMarks : IWritable
    {
        public class UnitMarker : IWritable
        {
            public GroupMarker TargetMarkerId { get; set; }
            public uint UnitId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(TargetMarkerId);
                writer.Write(UnitId);
            }
        }

        public ulong GroupId { get; set; }
        public List<UnitMarker> UnitMarkers { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            writer.Write(UnitMarkers.Count);
            foreach (var unitMarker in UnitMarkers)
            {
                unitMarker.Write(writer);
            }
        }
    }
}
