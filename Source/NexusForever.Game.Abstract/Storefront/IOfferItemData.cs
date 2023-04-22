using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Storefront
{
    public interface IOfferItemData : INetworkBuildable<ServerStoreOffers.OfferGroup.Offer.OfferItemData>
    {
        uint OfferId { get; }
        ushort ItemId { get; }
        uint Type { get; }
        uint Amount { get; }
        AccountItemEntry Entry { get; }
    }
}