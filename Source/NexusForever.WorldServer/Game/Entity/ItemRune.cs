using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.Entity
{
    public class ItemRune : ISaveCharacter
    {
        public ulong Guid { get; }
        public uint Index { get; }

        public ItemInfo RuneInfo { get; }

        public RuneType RuneType
        {
            get => runeType;
            set
            {
                if (value == RuneType.Random)
                    runeType = RuneSlotHelper.Instance.GetRandomRuneType(new List<RuneType> { runeType });
                else
                    runeType = value;
                saveMask |= ItemRuneSaveMask.RuneType;
            }
        }
        private RuneType runeType;

        public uint? RuneItem
        {
            get => runeItem;
            set
            {
                runeItem = value;
                saveMask |= ItemRuneSaveMask.RuneItem;
            }
        }
        private uint? runeItem;

        private ItemRuneSaveMask saveMask;

        public ItemRune(ItemRuneModel model)
        {
            Guid     = model.Id;
            Index    = model.Index;
            runeType = (RuneType)model.RuneType;
            runeItem = model.RuneItemId;

            if (model.RuneItemId.HasValue)
                RuneInfo = ItemManager.Instance.GetItemInfo(model.RuneItemId.Value);

            saveMask = ItemRuneSaveMask.None;
        }

        public ItemRune(ulong guid, uint index, RuneType runeType, uint? runeItem)
        {
            Guid     = guid;
            Index    = index;
            RuneType = runeType;
            RuneItem = runeItem;

            if (runeItem.HasValue)
                RuneInfo = ItemManager.Instance.GetItemInfo(runeItem.Value);

            saveMask |= ItemRuneSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == ItemRuneSaveMask.None)
                return;

            if ((saveMask & ItemRuneSaveMask.Create) != 0)
            {
                context.Add(new ItemRuneModel
                {
                    Id         = Guid,
                    Index      = Index,
                    RuneType   = (byte)RuneType,
                    RuneItemId = RuneItem
                });
            }
            else
            {
                var model = new ItemRuneModel
                {
                    Id    = Guid,
                    Index = Index
                };

                EntityEntry<ItemRuneModel> entity = context.Attach(model);
                if ((saveMask & ItemRuneSaveMask.RuneType) != 0)
                {
                    model.RuneType = (byte)RuneType;
                    entity.Property(p => p.RuneType).IsModified = true;
                }
                if ((saveMask & ItemRuneSaveMask.RuneItem) != 0)
                {
                    model.RuneItemId = RuneItem;
                    entity.Property(p => p.RuneItemId).IsModified = true;
                }
            }

            saveMask = ItemRuneSaveMask.None;
        }
    }
}
