using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PropertyModifier
    {
        public uint Priority { get; private set; }
        public float BaseValue { get; private set; }
        public float Value { get; private set; }
        public uint StackCount { get; private set; }

        public PropertyModifier(uint priority, float baseValue, float value)
        {
            Priority = priority;
            BaseValue = baseValue;
            Value = value;
        }

        public PropertyModifier(uint priority, float baseValue, float value, uint stackCount)
        {
            Priority = priority;
            BaseValue = baseValue;
            Value = value;
            StackCount = stackCount;
        }
    }
}
