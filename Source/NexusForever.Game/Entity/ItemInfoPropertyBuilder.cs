using System.Collections.Immutable;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class ItemInfoPropertyBuilder
    {
        public ImmutableDictionary<Property, float>.Builder Budgets { get; } = ImmutableDictionary.CreateBuilder<Property, float>();
        public ImmutableDictionary<Property, float>.Builder Properties { get; } = ImmutableDictionary.CreateBuilder<Property, float>();
    }
}
