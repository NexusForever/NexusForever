using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerPrimalMatrixUpdate)]
    public class ServerPrimalMatrixUpdate : IWritable
    {
        // AllocationCount in each node is the saved allocation count after a save is requested
        public List<PrimalMatrixNode> PrimalMatrixNodes { get; set; }
        public uint EldanPowerAugmentation { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PrimalMatrixNodes.Count);
            PrimalMatrixNodes.ForEach(node => writer.Write(node.PrimalMatrixNodeId));
            PrimalMatrixNodes.ForEach(node => writer.Write(node.AllocationCount));
            writer.Write(EldanPowerAugmentation);
        }
    }
}
