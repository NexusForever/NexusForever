using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPropertyModifier
    {
        Property Property { get; }
        ModType ModType { get; }

        float GetValue(uint level = 1u);
    }
}