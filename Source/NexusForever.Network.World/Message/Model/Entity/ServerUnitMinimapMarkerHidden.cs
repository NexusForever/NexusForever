using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitMinimapMarkerHidden)]
    public class ServerUnitMinimapMarkerHidden : IWritable
    {
        public uint UnitId { get; set; }
        public bool MinimapMarkerHidden { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(MinimapMarkerHidden);
        }
    }
}
