using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PropertyValue : IWritable
    {
        public Property Property { get; }
        public float BaseValue { get; }
        public float Value { get; set; }

        public PropertyValue(Property property, float baseValue, float value)
        {
            Property  = property;
            BaseValue = baseValue;
            Value     = value;
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Property, 8);
            writer.Write(BaseValue);
            writer.Write(Value);
        }
    }
}
