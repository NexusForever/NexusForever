using NexusForever.Database.World.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Storefront
{
    public class Category : IBuildable<ServerStoreCategories.StoreCategory>
    {
        public uint Id { get; }
        public string Name { get; }
        public string Description { get; }
        public uint ParentCategoryId { get; }
        public uint Index { get; }
        public bool Visible { get; }

        /// <summary>
        /// Create a new <see cref="Category"/> from an existing database model.
        /// </summary>
        public Category(StoreCategoryModel model)
        {
            Id               = model.Id;
            Name             = model.Name;
            Description      = model.Description;
            ParentCategoryId = model.ParentId;
            Index            = model.Index;
            Visible          = Convert.ToBoolean(model.Visible);
        }

        public ServerStoreCategories.StoreCategory Build()
        {
            return new ServerStoreCategories.StoreCategory
            {
                CategoryId       = Id,
                ParentCategoryId = ParentCategoryId,
                Name             = Name,
                Description      = Description,
                Index            = Index,
                Visible          = Visible
            };
        }
    }
}
