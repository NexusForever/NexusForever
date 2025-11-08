using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitPropertyUnknownUpdate)]
    public class ServerUnitPropertyUnknownUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public uint PropertyType { get; set; }
        public float BaseValue { get; set; }
        public float Value { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(PropertyType);
            writer.Write(BaseValue);
            writer.Write(Value);
        }
    }
}
