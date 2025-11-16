using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Map
{
    [Message(GameMessageOpcode.ServerGenericMapShow)]
    public class ServerGenericMapShow : IWritable
    {
        public uint GenericMapId { get; set; }
        public List<GenericMapNode> Nodes { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GenericMapId, 14u);
            writer.Write(Nodes.Count);
            Nodes.ForEach(node => node.Write(writer));
        }
    }
}
