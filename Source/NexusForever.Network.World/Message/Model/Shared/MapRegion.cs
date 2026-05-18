using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class MapRegion : IWritable
    {
        public uint WorldSocketId { get; set; }
        public uint WorldLocation2Id { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(WorldSocketId, 15u);
            writer.Write(WorldLocation2Id, 17u);
        }
    }
}
