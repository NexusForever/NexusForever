using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NetworkPropertyValue = NexusForever.Network.World.Message.Model.Shared.PropertyValue;

namespace NexusForever.Game.Entity
{
    public class PropertyValue : IPropertyValue
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

        public NetworkPropertyValue Build()
        {
            return new NetworkPropertyValue
            {
                Property  = Property,
                BaseValue = BaseValue,
                Value     = Value
            };
        }
    }
}
