using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class CostumeItem : ISaveCharacter
    {
        public const byte MaxCostumeItemDyes = 3;

        /// <summary>
        /// Return dye ramp mask generated from supplied dyes.
        /// </summary>
        public static int GenerateDyeMask(uint[] dyes)
        {
            int[] ramps = new int[MaxCostumeItemDyes];
            for (var i = 0; i < dyes.Length; i++)
            {
                if (dyes[i] == 0)
                    continue;

                DyeColorRampEntry entry = GameTableManager.Instance.DyeColorRamp.GetEntry(dyes[i]);
                ramps[i] = (int)entry.RampIndex;
            }

            return (int)((ramps[2] & 0x3FF | 0xFFFFF800) << 20) | ((ramps[1] & 0x3FF) << 10) | (ramps[0] & 0x3FF);
        }

        public CostumeItemSlot Slot { get; }
        public Item2Entry Entry { get; private set; }

        public uint ItemId
        {
            get => itemId;
            set
            {
                if (itemId == value)
                    return;

                Entry  = GameTableManager.Instance.Item.GetEntry(value);
                itemId = value;

                saveMask |= CostumeItemSaveMask.ItemId;
            }
        }

        private uint itemId;

        public int DyeData
        {
            get => dyeData;
            set
            {
                if (dyeData == value)
                    return;

                dyeData = value;
                saveMask |= CostumeItemSaveMask.DyeData;
            }
        }

        private int dyeData;

        private readonly Costume costume;

        private CostumeItemSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="CostumeItem"/> from an existing <see cref="CharacterCostumeItemModel"/> database model.
        /// </summary>
        public CostumeItem(Costume costume, CharacterCostumeItemModel model)
        {
            this.costume = costume;
            Slot         = (CostumeItemSlot)model.Slot;
            Entry        = GameTableManager.Instance.Item.GetEntry(model.ItemId);
            itemId       = model.ItemId;
            dyeData      = model.DyeData;
        }

        /// <summary>
        /// Create a new <see cref="CostumeItem"/> from packet <see cref="ClientCostumeSave"/>.
        /// </summary>
        public CostumeItem(Costume costume, ClientCostumeSave.CostumeItem item, CostumeItemSlot slot)
        {
            this.costume = costume;
            Slot         = slot;
            Entry        = GameTableManager.Instance.Item.GetEntry(item.ItemId);
            itemId       = item.ItemId;
            dyeData      = GenerateDyeMask(item.Dyes);
            saveMask     = CostumeItemSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == CostumeItemSaveMask.None)
                return;

            if ((saveMask & CostumeItemSaveMask.Create) != 0)
            {
                // costume item doesn't exist in database, all infomation must be saved
                context.Add(new CharacterCostumeItemModel
                {
                    Id      = costume.Owner,
                    Index   = costume.Index,
                    Slot    = (byte)Slot,
                    ItemId  = itemId,
                    DyeData = dyeData
                });
            }
            else
            {
                // costume item already exists in database, save only data that has been modified
                var model = new CharacterCostumeItemModel
                {
                    Id    = costume.Owner,
                    Index = costume.Index,
                    Slot  = (byte)Slot
                };

                EntityEntry<CharacterCostumeItemModel> entity = context.Attach(model);
                if ((saveMask & CostumeItemSaveMask.ItemId) != 0)
                {
                    model.ItemId = itemId;
                    entity.Property(p => p.ItemId).IsModified = true;
                }
                if ((saveMask & CostumeItemSaveMask.DyeData) != 0)
                {
                    model.DyeData = dyeData;
                    entity.Property(p => p.DyeData).IsModified = true;
                }
            }

            saveMask = CostumeItemSaveMask.None;
        }
    }
}
