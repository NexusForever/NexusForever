using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class PropertyModifier : IPropertyModifier
    {
        public Property Property { get; protected set; }
        public ModType ModType { get; protected set; }
        public float BaseValue { get; protected set; }
        public float Value { get; protected set; }

        public PropertyModifier(Property property, ModType modType, float baseValue, float value)
        {
            Property = property;
            ModType = modType;
            BaseValue = baseValue;
            Value = value;
        }
    }
}
