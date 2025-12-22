using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ClientPrimalMatrixSave)]
    public class ClientPrimalMatrixSave : IReadable
    {
        // Allocation counts send for each node are the unsaved amounts to apply
        public List<PrimalMatrixNode> Nodes { get; private set; } = [];

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt();
            for (uint i = 0; i < count; i++)
            {
                PrimalMatrixNode node = new PrimalMatrixNode();
                node.PrimalMatrixNodeId = reader.ReadUInt();
                Nodes.Add(node);
            }

            for (int i = 0;i < count; i++)
            {
                Nodes[i].AllocationCount = reader.ReadByte();
            }
        }
    }
}
