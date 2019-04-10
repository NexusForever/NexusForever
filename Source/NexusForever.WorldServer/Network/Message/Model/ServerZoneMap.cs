using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{   
    [Message(GameMessageOpcode.ServerZoneMap)]
    public class ServerZoneMap : IWritable
    {
        public uint ZoneMapId { get; set; }
        public uint Count { get; set; }
        public NetworkBitArray ZoneMapBits { get; set; }

        public ServerZoneMap()
        {
        }

        public ServerZoneMap(uint zoneMapId, uint count)
        {
            ZoneMapId = zoneMapId;
            ZoneMapBits = new NetworkBitArray(count);
            Count = count;
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ZoneMapId);
            writer.Write(Count / 8);

            writer.WriteBytes(ZoneMapBits.GetBuffer());
        }
    }
}
