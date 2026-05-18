using System.Collections.Immutable;
using NexusForever.Game.Static.Storefront;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Storefront
{
    public interface IOfferGroup : INetworkBuildable<ServerStoreOffers.OfferGroup>
    {
        uint Id { get; }
        DisplayFlag DisplayFlags { get; }
        string Name { get; }
        string Description { get; }
        ushort DisplayInfoOverride { get; }
        bool Visible { get; }
        ImmutableList<IOfferGroupCategory> Categories { get; }

        /// <summary>
        /// Returns an <see cref="IOfferItem"/> matching the supplied offer ID, if it exists.
        /// </summary>
        IOfferItem GetOfferItem(uint offerId);
    }
}