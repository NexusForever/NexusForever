using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemModdableData)]
    public class ServerItemModdableData : IWritable
    {
        public ulong ItemGuid { get; set; }
        public ItemThresholds ThresholdData { get; set; } = new();
        public RuneSlots GlyphData { get; set; } = new();
        public CraftStats CircuitData { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            ThresholdData.Write(writer);
            GlyphData.Write(writer);
            CircuitData.Write(writer);
        }
    }
}
