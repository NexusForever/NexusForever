using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerPrimalMatrixUpdate)]
    public class ServerPrimalMatrixUpdate : IWritable
    {
        public class PrimalMatrixNode
        {
            public uint PrimalMatrixNodeId { get; set; }
            public byte Value { get; set; }
        }

        public List<PrimalMatrixNode> PrimalMatrixNodes { get; set; }
        public uint EldanPowerAugmentation { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PrimalMatrixNodes.Count);
            PrimalMatrixNodes.ForEach(node => writer.Write(node.PrimalMatrixNodeId));
            PrimalMatrixNodes.ForEach(node => writer.Write(node.Value));
            writer.Write(EldanPowerAugmentation);
        }
    }
}
