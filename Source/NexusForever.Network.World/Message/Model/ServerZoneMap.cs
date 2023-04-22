using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{   
    [Message(GameMessageOpcode.ServerZoneMap)]
    public class ServerZoneMap : IWritable
    {
        public uint ZoneMapId { get; set; }
        public uint Count { get; set; }
        public NetworkBitArray ZoneMapBits { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ZoneMapId);
            writer.Write(Count / 8);

            writer.WriteBytes(ZoneMapBits.GetBuffer());
        }
    }
}
