using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerScavengerHuntClueRemoveByClueId)]
    public class ServerScavengerHuntClueRemoveByClueId : IWritable
    {
        public ushort PathExplorerScavengerClueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathExplorerScavengerClueId, 14);
        }
    }
}
