using System;
using NexusForever.Database.World.Model;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class Category
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

        public ServerStoreCategories.StoreCategory BuildNetworkPacket()
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
