using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NetworkItem = NexusForever.Network.World.Message.Model.Shared.Item;

namespace NexusForever.Game.Entity
{
    public class Item : IItem
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IItem"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum ItemSaveMask
        {
            None               = 0x0000,
            Create             = 0x0001,
            Delete             = 0x0002,
            CharacterId        = 0x0004,
            Location           = 0x0008,
            BagIndex           = 0x0010,
            StackCount         = 0x0020,
            Charges            = 0x0040,
            Durability         = 0x0080,
            ExpirationTimeLeft = 0x0100
        }

        /// <summary>
        /// Return the display id for <see cref="Item2Entry"/>.
        /// </summary>
        public static ushort GetDisplayId(Item2Entry entry)
        {
            if (entry == null)
                return 0;

            if (entry.ItemSourceId == 0u)
                return (ushort)entry.ItemDisplayId;

            List<ItemDisplaySourceEntryEntry> entries = AssetManager.Instance.GetItemDisplaySource(entry.ItemSourceId)
                .Where(e => e.Item2TypeId == entry.Item2TypeId)
                .ToList();

            if (entries.Count == 1)
                return (ushort)entries[0].ItemDisplayId;
            else if (entries.Count > 1)
            {
                if (entry.ItemDisplayId > 0)
                    return (ushort)entry.ItemDisplayId; // This is what the preview window shows for "Frozen Wrangler Mitts" (Item2Id: 28366).

                ItemDisplaySourceEntryEntry fallbackVisual = entries.FirstOrDefault(e => entry.PowerLevel >= e.ItemMinLevel && entry.PowerLevel <= e.ItemMaxLevel);
                if (fallbackVisual != null)
                    return (ushort)fallbackVisual.ItemDisplayId;
            }

            // TODO: research this...
            throw new NotImplementedException();
        }

        public uint Id => Info?.Id ?? SpellEntry.Id;
        public IItemInfo Info { get; }
        public Spell4BaseEntry SpellEntry { get; }
        public ulong Guid { get; }

        public ulong? CharacterId
        {
            get => characterId;
            set
            {
                characterId = value;
                saveMask |= ItemSaveMask.CharacterId;
            }
        }

        private ulong? characterId;

        public InventoryLocation Location
        {
            get => location;
            set
            {
                location = value;
                saveMask |= ItemSaveMask.Location;
            }
        }

        private InventoryLocation location;

        public InventoryLocation PreviousLocation { get; set; }

        public uint BagIndex
        {
            get => bagIndex;
            set
            {
                bagIndex = value;
                saveMask |= ItemSaveMask.BagIndex;
            }
        }

        private uint bagIndex;

        public uint PreviousBagIndex { get; set; }

        public uint StackCount
        {
            get => stackCount;
            set
            {
                if (value > Info.Entry.MaxStackCount)
                    throw new ArgumentOutOfRangeException();

                stackCount = value;
                saveMask |= ItemSaveMask.StackCount;
            }
        }

        private uint stackCount;

        public uint Charges
        {
            get => charges;
            set
            {
                if (value > Info.Entry.MaxCharges)
                    throw new ArgumentOutOfRangeException();

                charges = value;
                saveMask |= ItemSaveMask.Charges;
            }
        }

        private uint charges;

        public float Durability
        {
            get => durability;
            set
            {
                if (Durability > 1.0f)
                    throw new ArgumentOutOfRangeException();

                durability = value;
                saveMask |= ItemSaveMask.Durability;
            }
        }

        private float durability;

        public uint ExpirationTimeLeft
        {
            get => expirationTimeLeft;
            set
            {
                expirationTimeLeft = value;
                saveMask |= ItemSaveMask.ExpirationTimeLeft;
            }
        }

        private uint expirationTimeLeft;

        /// <summary>
        /// Returns if <see cref="IItem"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & ItemSaveMask.Create) != 0;

        public bool PendingDelete => (saveMask & ItemSaveMask.Delete) != 0;

        private ItemSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="IItem"/> from an existing database model.
        /// </summary>
        public Item(ItemModel model)
        {
            Guid             = model.Id;
            characterId      = model.OwnerId;
            location         = InventoryLocation.None;
            PreviousLocation = InventoryLocation.None;
            bagIndex         = 0u;
            PreviousBagIndex = 0u;
            stackCount       = model.StackCount;
            charges          = model.Charges;
            durability       = model.Durability;

            if ((InventoryLocation)model.Location != InventoryLocation.Ability)
                Info = ItemManager.Instance.GetItemInfo(model.ItemId);
            else
                SpellEntry = GameTableManager.Instance.Spell4Base.GetEntry(model.ItemId);

            saveMask = ItemSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="IItem"/> from an <see cref="IItemInfo"/> template.
        /// </summary>
        public Item(ulong? owner, IItemInfo info, uint count = 1u, uint initialCharges = 0)
        {
            Guid             = ItemManager.Instance.NextItemId;
            characterId      = owner;
            location         = InventoryLocation.None;
            PreviousLocation = InventoryLocation.None;
            bagIndex         = 0u;
            stackCount       = count;
            charges          = initialCharges;
            durability       = 1.0f;
            Info             = info;

            saveMask         = ItemSaveMask.Create;
        }

        /// <summary>
        /// Create a new <see cref="IItem"/> from a <see cref="Spell4BaseEntry"/> template.
        /// </summary>
        public Item(ulong owner, Spell4BaseEntry entry, uint count = 1u)
        {
            Guid             = ItemManager.Instance.NextItemId;
            characterId      = owner;
            location         = InventoryLocation.None;
            PreviousLocation = InventoryLocation.None;
            bagIndex         = 0u;
            PreviousBagIndex = 0u;
            stackCount       = count;
            charges          = 0u;
            durability       = 0.0f;
            SpellEntry       = entry;

            saveMask         = ItemSaveMask.Create;
        }

        /// <summary>
        /// Enqueue <see cref="IItem"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool state)
        {
            saveMask = ItemSaveMask.Delete;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == ItemSaveMask.None)
                return;

            if ((saveMask & ItemSaveMask.Create) != 0)
            {
                // item doesn't exist in database, all information must be saved
                context.Add(new ItemModel
                {
                    Id                 = Guid,
                    OwnerId            = CharacterId,
                    ItemId             = Id,
                    Location           = (ushort)Location,
                    BagIndex           = BagIndex,
                    StackCount         = StackCount,
                    Charges            = Charges,
                    Durability         = Durability,
                    ExpirationTimeLeft = ExpirationTimeLeft
                });
            }
            else if ((saveMask & ItemSaveMask.Delete) != 0)
            {
                var model = new ItemModel
                {
                    Id = Guid,
                };

                context.Entry(model).State = EntityState.Deleted;
            }
            else
            {
                // item already exists in database, save only data that has been modified
                var model = new ItemModel
                {
                    Id = Guid
                };

                // could probably clean this up with reflection, works for the time being
                EntityEntry<ItemModel> entity = context.Attach(model);
                if ((saveMask & ItemSaveMask.CharacterId) != 0)
                {
                    model.OwnerId = CharacterId;
                    entity.Property(p => p.OwnerId).IsModified = true;
                }
                if ((saveMask & ItemSaveMask.Location) != 0)
                {
                    model.Location = (ushort)Location;
                    entity.Property(p => p.Location).IsModified = true;
                }
                if ((saveMask & ItemSaveMask.BagIndex) != 0)
                {
                    model.BagIndex = BagIndex;
                    entity.Property(p => p.BagIndex).IsModified = true;
                }
                if ((saveMask & ItemSaveMask.StackCount) != 0)
                {
                    model.StackCount = StackCount;
                    entity.Property(p => p.StackCount).IsModified = true;
                }
                if ((saveMask & ItemSaveMask.Charges) != 0)
                {
                    model.Charges = Charges;
                    entity.Property(p => p.Charges).IsModified = true;
                }
                if ((saveMask & ItemSaveMask.Durability) != 0)
                {
                    model.Durability = Durability;
                    entity.Property(p => p.Durability).IsModified = true;
                }
                if ((saveMask & ItemSaveMask.ExpirationTimeLeft) != 0)
                {
                    model.ExpirationTimeLeft = ExpirationTimeLeft;
                    entity.Property(p => p.ExpirationTimeLeft).IsModified = true;
                }
            }

            saveMask = ItemSaveMask.None;
        }

        /// <summary>
        /// Return a network representation of the current <see cref="IItem"/> for use in a item related packet.
        /// </summary>
        public NetworkItem Build()
        {
            var networkItem = new NetworkItem
            {
                Guid         = Guid,
                ItemId       = Id,
                LocationData = new ItemLocation
                {
                    Location = Location,
                    BagIndex = BagIndex
                },
                StackCount = StackCount,
                Charges    = Charges,
                Durability = Durability,
                Unknown58  = new NetworkItem.UnknownStructure[2]
                {
                    new NetworkItem.UnknownStructure(),
                    new NetworkItem.UnknownStructure()
                }
            };

            return networkItem;
        }

        /// <summary>
        /// Returns the <see cref="CurrencyType"/> this <see cref="IItem"/> sells for at a vendor.
        /// </summary>
        public CurrencyType GetVendorSellCurrency(byte index)
        {
            if (Info.Entry.CurrencyTypeIdSellToVendor[index] != 0u)
                return (CurrencyType)Info.Entry.CurrencyTypeIdSellToVendor[index];

            return CurrencyType.None;
        }

        /// <summary>
        /// Returns the amount of <see cref="CurrencyType"/> this <see cref="IItem"/> sells for at a vendor.
        /// </summary>
        public uint GetVendorSellAmount(byte index)
        {
            if (Info.Entry.CurrencyTypeIdSellToVendor[index] != 0u)
                return Info.Entry.CurrencyAmountSellToVendor[index];

            // most items that sell for credits have their sell amount calculated and not stored in the tbl
            return CalculateVendorSellAmount();
        }

        private uint CalculateVendorSellAmount()
        {
            // TODO: Rawaho was lazy and didn't finish this
            // GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(559);
            // uint cost = Entry.PowerLevel * entry.Dataint01;
            return 0u;
        }
    }
}
