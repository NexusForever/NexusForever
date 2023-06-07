using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class SpellPropertyModifier : ISpellPropertyModifier
    {
        public Property Property { get; private set; }
        public uint Priority { get; private set; }
        public List<IPropertyModifier> Alterations { get; private set; } = new();
        public uint StackCount { get; private set; } // TODO: Should we have StackCount on this? I presume we just want to have spell effects stack up individually, not tracked in each SpellPropertyModifier

        public SpellPropertyModifier(Property property, uint priority, float value2, float value3, float value4, uint stackCount = 1)
        {
            Property = property;
            Priority = priority;
            StackCount = stackCount;

            CalculateModAndValue(value2, value3, value4);
        }

        /// <summary>
        /// Calculates the modifier type and value.
        /// </summary>
        /// <remarks>
        /// Based on digging through a lot of spell effects, it appears that each dataBits was a specific adjustment. It does not appear as though a Percentage and FlatValue applied at the same time. BUt, a Level Scaling would apply alongside either.
        /// </remarks>
        private void CalculateModAndValue(float value2, float value3, float value4)
        {
            // Value4 is used by very few spells to adjust property value further based on level. Most are deprecated, too.
            if (value4 != 0f)
                Alterations.Add(new PropertyModifier(Property, ModType.LevelScale, 0f, value4));

            // Value3 is a flat value
            if (value3 != 0f)
                Alterations.Add(new PropertyModifier(Property, ModType.FlatValue, 0f, value3));
            // Value2 is a Percentage
            else if (value2 != 0f)
                Alterations.Add(new PropertyModifier(Property, ModType.Percentage, 0f, value2));
        }
    }
}
