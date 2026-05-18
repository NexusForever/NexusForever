using System.Collections;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Costume;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Message.Model.Costume;
using NetworkCostume = NexusForever.Network.World.Message.Model.Costume.Costume;

namespace NexusForever.Game.Entity
{
    public class Costume : ICostume
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="ICostume"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum CostumeSaveMask
        {
            None   = 0x00,
            Create = 0x01,
            Mask   = 0x02
        }

        public const byte MaxCostumeItems = 7;

        public ulong Owner { get; }
        public byte Index { get; }

        public uint VisibilityMask
        {
            get => visibilityMask;
            set
            {
                if (visibilityMask == value)
                    return;

                visibilityMask = value;
                saveMask |= CostumeSaveMask.Mask;
            }
        }

        private uint visibilityMask;

        private readonly ICostumeItem[] items = new CostumeItem[MaxCostumeItems];

        private CostumeSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="ICostume"/> from an existing <see cref="CharacterCostumeModel"/> database model.
        /// </summary>
        public Costume(CharacterCostumeModel model)
        {
            Owner = model.Id;
            Index = model.Index;
            visibilityMask  = model.VisibilityMask;

            foreach (CharacterCostumeItemModel costumeItemModel in model.CostumeItem)
                items[costumeItemModel.Slot] = new CostumeItem(this, costumeItemModel);
        }

        /// <summary>
        /// Create a new <see cref="ICostume"/> from packet <see cref="ClientCostumeSave"/>.
        /// </summary>
        public Costume(IPlayer player, ClientCostumeSave costumeSave)
        {
            Owner = player.CharacterId;
            Index = (byte)costumeSave.Index;
            visibilityMask  = costumeSave.VisibilityMask;

            for (byte i = 0; i < costumeSave.Items.Count; i++)
                items[i] = new CostumeItem(this, costumeSave.Items[i], (CostumeItemSlot)i);

            saveMask = CostumeSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != CostumeSaveMask.None)
            {
                if ((saveMask & CostumeSaveMask.Create) != 0)
                {
                    // costume doesn't exist in database, all information must be saved
                    var model = new CharacterCostumeModel
                    {
                        Id    = Owner,
                        Index = Index,
                        VisibilityMask  = VisibilityMask
                    };

                    context.Add(model);
                }
                else
                {
                    // costume already exists in database, save only data that has been modified
                    var model = new CharacterCostumeModel
                    {
                        Id    = Owner,
                        Index = Index
                    };

                    EntityEntry<CharacterCostumeModel> entity = context.Attach(model);
                    if ((saveMask & CostumeSaveMask.Mask) != 0)
                    {
                        model.VisibilityMask = visibilityMask;
                        entity.Property(p => p.VisibilityMask).IsModified = true;
                    }
                }

                saveMask = CostumeSaveMask.None;
            }

            foreach (ICostumeItem costumeItem in items)
                costumeItem.Save(context);
        }

        /// <summary>
        /// Return <see cref="ICostumeItem"/> at supplied index.
        /// </summary>
        public ICostumeItem GetItem(CostumeItemSlot slot)
        {
            return items[(int)slot];
        }

        /// <summary>
        /// Get <see cref="ICostumeItem"/> for supplied <see cref="ItemSlot"/>.
        /// </summary>
        public ICostumeItem GetItem(ItemSlot slot)
        {
            return items.SingleOrDefault(i => i.ItemSlot == slot);
        }

        /// <summary>
        /// Get <see cref="IItemVisual"/> for <see cref="ICostumeItem"/> for supplied <see cref="ItemSlot"/>.
        /// </summary>
        public IItemVisual GetItemVisual(ItemSlot slot)
        {
            ICostumeItem item = GetItem(slot);
            return item != null ? GetItemVisual(item) : null;
        }

        private IItemVisual GetItemVisual(ICostumeItem item)
        {
            IItemVisual visual = item.GetItemVisual();

            // remove visual if slot is hidden
            if ((VisibilityMask & 1 << (int)item.Slot) == 0)
                visual.DisplayId = 0;

            return visual;
        }

        /// <summary>
        /// Return a collection of <see cref="IItemVisual"/> for <see cref="ICostume"/>.
        /// </summary>
        public IEnumerable<IItemVisual> GetItemVisuals()
        {
            return items.Select(GetItemVisual);
        }

        /// <summary>
        /// Update <see cref="ICostume"/> from <see cref="ClientCostumeSave"/>.
        /// </summary>
        public void Update(ClientCostumeSave costumeSave)
        {
            VisibilityMask = costumeSave.VisibilityMask;

            for (int i = 0; i < costumeSave.Items.Count; i++)
            {
                items[i].Item2Id  = costumeSave.Items[i].Item2Id;
                items[i].DyeData = CostumeItem.GenerateDyeData(costumeSave.Items[i].DyeColorRampIds);
            }
        }

        public NetworkCostume Build()
        {
            var networkCostume = new NetworkCostume
            {
                Index = Index,
                VisibilityMask = VisibilityMask
            };

            foreach (ICostumeItem costumeItem in items)
            {
                networkCostume.Item2Ids[(byte)costumeItem.Slot] = costumeItem.Item2Id ?? 0;
                networkCostume.DyeData[(byte)costumeItem.Slot] = costumeItem.DyeData;
            }

            return networkCostume;
        }

        public IEnumerator<ICostumeItem> GetEnumerator()
        {
            return items.Cast<ICostumeItem>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
