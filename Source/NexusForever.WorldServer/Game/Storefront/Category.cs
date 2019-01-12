using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

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

        public Category(StoreCategory model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            ParentCategoryId = model.ParentId;
            Index = model.Index;
            Visible = Convert.ToBoolean(model.Visible);
        }

        public ServerStoreCategories.StoreCategory BuildNetworkPacket()
        {
            return new ServerStoreCategories.StoreCategory
            {
                CategoryId = Id,
                ParentCategoryId = ParentCategoryId,
                CategoryName = Name,
                CategoryDesc = Description,
                Index = Index,
                Visible = Visible
            };
        }
    }
}
