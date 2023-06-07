using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPropertyModifier
    {
        float BaseValue { get; }
        ModType ModType { get; }
        Property Property { get; }
        float Value { get; }
    }
}