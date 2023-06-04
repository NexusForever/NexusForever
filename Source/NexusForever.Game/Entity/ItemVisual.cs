using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NetworkItemVisual = NexusForever.Network.World.Message.Model.Shared.ItemVisual;

namespace NexusForever.Game.Entity
{
    public class ItemVisual : IItemVisual
    {
        public required ItemSlot Slot { get; init; }
        public required ushort? DisplayId { get; set; }
        public ushort ColourSetId { get; set; }
        public int DyeData { get; set; }
        
        public NetworkItemVisual Build()
        {
            return new()
            {
                Slot        = Slot,
                DisplayId   = DisplayId ?? 0,
                ColourSetId = ColourSetId,
                DyeData     = DyeData
            };
        }
    }
}
