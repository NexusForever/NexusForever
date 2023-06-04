using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NetworkItemVisual = NexusForever.Network.World.Message.Model.Shared.ItemVisual;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IItemVisual : INetworkBuildable<NetworkItemVisual>
    {
        ItemSlot Slot { get; init; }
        ushort? DisplayId { get; set; }
        ushort ColourSetId { get; set; }
        int DyeData { get; set; }
    }
}