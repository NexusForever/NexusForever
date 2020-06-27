using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Costume : ISaveCharacter, IEnumerable<CostumeItem>
    {
        public const byte MaxCostumeItems = 7;

        public ulong Owner { get; }
        public byte Index { get; }

        public uint Mask
        {
            get => mask;
            set
            {
                if (mask == value)
                    return;

                mask = value;
                saveMask |= CostumeSaveMask.Mask;
            }
        }

        private uint mask;

        private readonly CostumeItem[] items = new CostumeItem[MaxCostumeItems];

        private CostumeSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Costume"/> from an existing <see cref="CharacterCostumeModel"/> database model.
        /// </summary>
        public Costume(CharacterCostumeModel model)
        {
            Owner = model.Id;
            Index = model.Index;
            mask  = model.Mask;
            
            foreach (CharacterCostumeItemModel costumeItemModel in model.CostumeItem)
                items[costumeItemModel.Slot] = new CostumeItem(this, costumeItemModel);
        }

        /// <summary>
        /// Create a new <see cref="Costume"/> from packet <see cref="ClientCostumeSave"/>.
        /// </summary>
        public Costume(Player player, ClientCostumeSave costumeSave)
        {
            Owner = player.CharacterId;
            Index = (byte)costumeSave.Index;
            mask  = costumeSave.Mask;

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
                        Mask  = Mask
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
                        model.Mask = mask;
                        entity.Property(p => p.Mask).IsModified = true;
                    }
                }

                saveMask = CostumeSaveMask.None;
            }

            foreach (CostumeItem costumeItem in items)
                costumeItem.Save(context);
        }

        /// <summary>
        /// Return <see cref="CostumeItem"/> at supplied index.
        /// </summary>
        public CostumeItem GetItem(CostumeItemSlot slot)
        {
            return items[(int)slot];
        }

        /// <summary>
        /// Update <see cref="Costume"/> from <see cref="ClientCostumeSave"/>.
        /// </summary>
        public void Update(ClientCostumeSave costumeSave)
        {
            Mask = costumeSave.Mask;

            for (int i = 0; i < costumeSave.Items.Count; i++)
            {
                items[i].ItemId  = costumeSave.Items[i].ItemId;
                items[i].DyeData = CostumeItem.GenerateDyeMask(costumeSave.Items[i].Dyes);
            }
        }
        
        public IEnumerator<CostumeItem> GetEnumerator()
        {
            return items.Cast<CostumeItem>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
