using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitMinimapMarkerChanged)]
    public class ServerUnitMinimapMarkerChanged : IWritable
    {
        public uint UnitId { get; set; }
        public ushort MinimapMarkerId { get; set; } // see minimapMarker tbl for values

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(MinimapMarkerId);
        }
    }
}
