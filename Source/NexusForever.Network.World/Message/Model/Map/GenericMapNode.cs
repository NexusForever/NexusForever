using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Map
{
    public class GenericMapNode : IWritable
    {
        public uint GenericMapModeId { get; set; }
        public bool Enabled { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GenericMapModeId, 14u);
            writer.Write(Enabled);
        }
    }
}
