using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSupplySatchelUpdate)]
    public class ServerSupplySatchelUpdate : IWritable
    {
        public ushort TradeskillMaterialId { get; set; }
        public ushort StackCount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TradeskillMaterialId, 14u);
            writer.Write(StackCount);
        }
    }
}
