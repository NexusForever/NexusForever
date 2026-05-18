using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ISpellPropertyModifier
    {
        List<IPropertyModifier> Alterations { get; }
        uint Priority { get; }
        Property Property { get; }
        uint StackCount { get; }
    }
}