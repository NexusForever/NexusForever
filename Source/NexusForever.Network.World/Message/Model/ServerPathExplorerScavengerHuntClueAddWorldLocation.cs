using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathExplorerScavengerHuntClueAddWorldLocation)]
    public class ServerPathExplorerScavengerHuntClueAddWorldLocation : IWritable
    {
        public uint WorldLocation2Id { get; set; }
        public ushort PathExplorerScavengerClueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(WorldLocation2Id, 17);
            writer.Write(PathExplorerScavengerClueId, 14);
        }
    }
}
