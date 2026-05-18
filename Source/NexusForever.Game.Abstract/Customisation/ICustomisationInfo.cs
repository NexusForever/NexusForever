using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Customisation
{
    public interface ICustomisationInfo
    {
        Race Race { get; }
        Sex Sex { get; }
        uint[] Label { get; }
        uint[] Value { get; }
        ItemSlot Slot { get; }
        uint DisplayId { get; }
    }
}
