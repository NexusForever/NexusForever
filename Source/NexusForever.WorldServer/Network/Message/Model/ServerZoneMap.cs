using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
