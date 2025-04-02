using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathExplorerScavengerHuntClueAddUnitList)]
    public class ServerPathExplorerScavengerHuntClueAddUnitList : IWritable
    {
        public ushort PathExplorerScavengerClueId { get; set; }
        public uint[] UnitIdList { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathExplorerScavengerClueId, 14);
            for (uint i = 0u; i < UnitIdList.Length; i++)
                writer.Write(UnitIdList[i]);
        }
    }
}
