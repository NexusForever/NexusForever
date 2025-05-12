using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathExplorerScavengerHuntClueRemoveByClueIdAndUnitId)]
    public class ServerPathExplorerScavengerHuntClueRemoveByClueIdAndUnitId : IWritable
    {
        public ushort PathExplorerScavengerClueId { get; set; }
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathExplorerScavengerClueId, 14);
            writer.Write(UnitId);
        }
    }
}
