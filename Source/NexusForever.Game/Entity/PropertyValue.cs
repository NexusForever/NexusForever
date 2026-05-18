using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NetworkPropertyValue = NexusForever.Network.World.Message.Model.Shared.PropertyValue;

namespace NexusForever.Game.Entity
{
    public class PropertyValue : IPropertyValue
    {
        public Property Property { get; }
        public float BaseValue { get; set; }
        public float Value { get; set; }

        /// <summary>
        /// Create a new <see cref="IPropertyValue"/> with supplied <see cref="Property"/> and base value.
        /// </summary>
        public PropertyValue(Property property, float baseValue)
        {
            Property  = property;
            BaseValue = baseValue;
            Value     = BaseValue;
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
