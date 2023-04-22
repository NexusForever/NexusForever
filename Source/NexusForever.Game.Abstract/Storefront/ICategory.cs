using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Storefront
{
    public interface ICategory : INetworkBuildable<ServerStoreCategories.StoreCategory>
    {
        uint Id { get; }
        string Name { get; }
        string Description { get; }
        uint ParentCategoryId { get; }
        uint Index { get; }
        bool Visible { get; }
    }
}