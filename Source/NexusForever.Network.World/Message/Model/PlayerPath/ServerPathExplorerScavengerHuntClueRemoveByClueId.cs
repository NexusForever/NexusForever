using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathExplorerScavengerHuntClueRemoveByClueId)]
    public class ServerPathExplorerScavengerHuntClueRemoveByClueId : IWritable
    {
        public ushort PathExplorerScavengerClueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathExplorerScavengerClueId, 14);
        }
    }
}
