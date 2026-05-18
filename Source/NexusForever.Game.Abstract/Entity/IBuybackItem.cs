using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IBuybackItem : IUpdate, INetworkBuildable<BuybackItem>
    {
        uint UniqueId { get; }
        IItem Item { get; }
        uint Quantity { get; }
        List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> CurrencyChange { get; }
        
        bool HasExpired { get; }
    }
}