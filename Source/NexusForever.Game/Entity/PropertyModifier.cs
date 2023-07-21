using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class PropertyModifier : IPropertyModifier
    {
        public Property Property { get; }
        public ModType ModType { get; }
        public float Value { get; }

        /// <summary>
        /// Create a new <see cref="IPropertyModifier"/> from supplied parameters.
        /// </summary>
        public PropertyModifier(Property property, ModType modType, float value)
        {
            Property  = property;
            ModType   = modType;
            Value     = value;
        }

        public float GetValue(uint level = 1u)
        {
            if (ModType == ModType.LevelScale)
                return Value * level;

            return Value;
        }
    }
}
