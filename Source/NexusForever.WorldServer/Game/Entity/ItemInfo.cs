﻿using System;
using System.Collections.Immutable;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class ItemInfo
    {
        public uint Id => Entry.Id;
        public Item2Entry Entry { get; }
        public Item2FamilyEntry FamilyEntry { get; }
        public Item2CategoryEntry CategoryEntry { get; }
        public Item2TypeEntry TypeEntry { get; }
        public ItemSlotEntry SlotEntry { get; }
        public ItemBudgetEntry BudgetEntry { get; }
        public ItemStatEntry StatEntry { get; }
        public SecondaryItemFlags SecondaryItemFlags { get; }

        public float ItemPower { get; private set; }
        public ImmutableDictionary<Property, float> Properties { get; private set; }

        /// <summary>
        /// Create a new <see cref="ItemInfo"/> from <see cref="Item2Entry"/> entry.
        /// </summary>
        public ItemInfo(Item2Entry entry)
        {
            Entry         = entry;
            FamilyEntry   = GameTableManager.Instance.Item2Family.GetEntry(Entry.Item2FamilyId);
            CategoryEntry = GameTableManager.Instance.Item2Category.GetEntry(Entry.Item2CategoryId);
            TypeEntry     = GameTableManager.Instance.Item2Type.GetEntry(Entry.Item2TypeId);
            SlotEntry     = GameTableManager.Instance.ItemSlot.GetEntry(TypeEntry.ItemSlotId);
            BudgetEntry   = GameTableManager.Instance.ItemBudget.GetEntry(Entry.ItemBudgetId);
            StatEntry     = GameTableManager.Instance.ItemStat.GetEntry(Entry.ItemStatId);

            // the client combines the flags from the family, category and type entries into a single value
            SecondaryItemFlags = FamilyEntry.Flags | CategoryEntry.Flags | TypeEntry.Flags;

            CalculateProperties();
        }

        /// <summary>
        /// Calculate item properties.
        /// </summary>
        /// <remarks>
        /// This is based on client code which generates the property values for item tooltips, see function 0x0750880 for more information.
        /// Blame Rawaho if these calculations are wrong :)
        /// </remarks>
        private void CalculateProperties()
        {
            ItemPower = CalculateItemPower();

            var builder = new ItemInfoPropertyBuilder();
            if (BudgetEntry != null && StatEntry != null)
                CalculateExplicitProperties(builder);
            
            CalculateImplicitProperties(builder);

            Properties = builder.Properties.ToImmutable();
        }

        /// <summary>
        /// Calculate item power.
        /// </summary>
        /// <remarks>
        /// This is used in the item budget calculations.
        /// </remarks>
        private float CalculateItemPower()
        {
            float Calculate(GameFormulaEntry formulaEntry)
            {
                float relativePowerLevel = Entry.PowerLevel - formulaEntry.Dataint0;
                return (MathF.Pow(MathF.E, formulaEntry.Datafloat01 * relativePowerLevel) * formulaEntry.Datafloat0) + (MathF.Pow(relativePowerLevel, formulaEntry.Datafloat03) * formulaEntry.Datafloat02);
            }

            GameFormulaEntry formulaEntry;
            if (FamilyEntry.Id == 33u)
            {
                // runes
                formulaEntry = Entry.PowerLevel switch
                {
                    < 50u  => GameTableManager.Instance.GameFormula.GetEntry(991),
                    < 130u => GameTableManager.Instance.GameFormula.GetEntry(65),
                    _      => GameTableManager.Instance.GameFormula.GetEntry(253)
                };
            }
            else
            {
                formulaEntry = Entry.PowerLevel switch
                {
                    < 50u  => GameTableManager.Instance.GameFormula.GetEntry(1028),
                    < 130u => GameTableManager.Instance.GameFormula.GetEntry(1027),
                    _      => GameTableManager.Instance.GameFormula.GetEntry(1255)
                };
            }

            return Calculate(formulaEntry) * (SlotEntry?.ArmorModifier ?? 1f);
        }

        /// <summary>
        /// Calculate item properties with explicit item budget and stats.
        /// </summary>
        private void CalculateExplicitProperties(ItemInfoPropertyBuilder builder)
        {
            CalculatePropertyBudgets(builder);
            CalculatePropertyValues(builder);
        }

        private void CalculatePropertyBudgets(ItemInfoPropertyBuilder builder)
        {
            for (int i = 0; i < StatEntry.ItemStatTypeEnum.Length; i++)
            {
                float budget = BudgetEntry.Budgets[i];

                switch (StatEntry.ItemStatTypeEnum[i])
                {
                    case ItemStatType.None:
                        continue;
                    case ItemStatType.Standard:
                        builder.Budgets.Add((Property)StatEntry.ItemStatData[i], budget);
                        break;
                    // TODO: the data value is the RandomStatGroupId, choose a random stat?
                    case ItemStatType.RandomStatGroup:
                        break;
                    case ItemStatType.Unknown4:
                    {
                        // this one is weird...
                        // there is additional code if offset 0x60 in item template is set but from what I can see this is never set
                        // due to this I'm just hard coding the else condition which sets the type to 0, basically removing it 
                        continue;
                    }
                }
            }
        }

        private void CalculatePropertyValues(ItemInfoPropertyBuilder builder)
        {
            foreach ((Property property, float budget) in builder.Budgets)
            {
                // some of the shield properties are a special case
                // we only care about the budget being calculated and the value will be calculated in the dedicated CalculateShield function later
                if (property is Property.ShieldCapacityMax
                    or Property.ShieldMitigationMax
                    or Property.ShieldRegenPct
                    or Property.ShieldRebootTime)
                    continue;

                UnitProperty2Entry entry = GameTableManager.Instance.UnitProperty2.GetEntry((uint)property);
                if (entry != null && (entry.Flags & 0x04) != 0)
                    builder.Properties[property] = (budget * ItemPower) * entry.ValuePerPoint;
            }
        }

        /// <summary>
        /// Calculate item properties without explicit item budget and stats.
        /// </summary>
        private void CalculateImplicitProperties(ItemInfoPropertyBuilder builder)
        {
            if (Entry.ItemImbuementId != 0u)
            {
                // TODO
            }

            if (SlotEntry != null && Entry.PowerLevel != 0u)
            {
                if (IsEquippableIntoSlot(EquippedItem.Shields) && (SecondaryItemFlags & SecondaryItemFlags.Unknown0400) != 0u)
                    CalculateShield(builder);
                else
                {
                    CalculatePrimary(builder);
                    CalculateArmor(builder);
                    CalculateMaxHealth(builder);
                }
            }

            if (FamilyEntry.Id == 33u)
            {
                // TODO: runes
            }
        }

        /// <summary>
        /// Calculate item shield properties (ShieldCapacityMax, ShieldMitigationMax, ShieldRegenPct and ShieldRebootTime).
        /// </summary>
        private void CalculateShield(ItemInfoPropertyBuilder builder)
        {
            float CalculateShieldCapacityMax(float budget)
            {
                GameFormulaEntry formulaEntry = GameTableManager.Instance.GameFormula.GetEntry(548);
                GameFormulaEntry budgetEntry  = GameTableManager.Instance.GameFormula.GetEntry(1260);

                float shield   = (formulaEntry.Datafloat03 * formulaEntry.Datafloat0) + (Entry.PowerLevel * formulaEntry.Datafloat02);
                float modifier = ((budgetEntry.Datafloat01 - budgetEntry.Datafloat0) * budget) + budgetEntry.Datafloat0;
                return shield * modifier;
            }

            float CalculateShieldMitigationMax(float budget)
            {
                GameFormulaEntry formulaEntry = GameTableManager.Instance.GameFormula.GetEntry(1261);
                return ((formulaEntry.Datafloat01 - formulaEntry.Datafloat0) * budget) + formulaEntry.Datafloat0;
            }

            float CalculateShieldRegenPercentage(float budget)
            {
                GameFormulaEntry formulaEntry = GameTableManager.Instance.GameFormula.GetEntry(1260);
                return ((formulaEntry.Datafloat03 - formulaEntry.Datafloat02) * budget) + formulaEntry.Datafloat02;
            }

            float CalculateShieldRebootTime(float budget)
            {
                GameFormulaEntry formulaEntry = GameTableManager.Instance.GameFormula.GetEntry(1261);
                return (((formulaEntry.Dataint0 - formulaEntry.Dataint01) * (1f - budget)) + formulaEntry.Dataint01) + 0.0000099999997f;
            }

            builder.Properties[Property.ShieldCapacityMax] = CalculateShieldCapacityMax(
                builder.Budgets.TryGetValue(Property.ShieldCapacityMax, out float shieldCapacityMaxBudget) ? shieldCapacityMaxBudget : 0.5f);
            builder.Properties[Property.ShieldMitigationMax] = CalculateShieldMitigationMax(
                builder.Budgets.TryGetValue(Property.ShieldMitigationMax, out float shieldMitigationMaxBudget) ? shieldMitigationMaxBudget : 0.5f);
            builder.Properties[Property.ShieldRegenPct] = CalculateShieldRegenPercentage(
                builder.Budgets.TryGetValue(Property.ShieldRegenPct, out float shieldRegenPctBudget) ? shieldRegenPctBudget : 0.5f);
            builder.Properties[Property.ShieldRebootTime] = CalculateShieldRebootTime(
                builder.Budgets.TryGetValue(Property.ShieldRebootTime, out float shieldRebootTimeBudget) ? shieldRebootTimeBudget : 0.5f);
        }

        /// <summary>
        /// Calculate item primary properties (AssaultRating, SupportRating, PvPOffensiveRating and PvPDefensiveRating).
        /// </summary>
        private void CalculatePrimary(ItemInfoPropertyBuilder builder)
        {
            float CalculatePrimaryBaseValue(GameFormulaEntry formulaEntry)
            {
                float power = Entry.PowerLevel - formulaEntry.Dataint0;
                return (MathF.Pow(MathF.E, power * formulaEntry.Datafloat01) * formulaEntry.Datafloat0) + (MathF.Pow(power, formulaEntry.Datafloat03) * formulaEntry.Datafloat02);
            }

            if (FamilyEntry.Id == 26)
                return;

            GameFormulaEntry formulaEntry = GameTableManager.Instance.GameFormula.GetEntry(FamilyEntry.Id == 2u ? 1288ul : 549ul);
            float value = CalculatePrimaryBaseValue(formulaEntry);

            float support = Entry.SupportPowerPercentage;
            // TODO: research more
            /*if (dword60)
                support = MathF.Abs(support) * (((dword60_6 * (1.0f / 255.0f)) * 2.0f) - 1.0f);*/

            GameFormulaEntry formulaEntry2 = GameTableManager.Instance.GameFormula.GetEntry(1265);

            float v14 = (((1.0f - MathF.Abs(support)) * formulaEntry2.Datafloat0) + 1.0f) * (value * SlotEntry.ArmorModifier);
            float v25 = formulaEntry2.Datafloat01 * v14;

            if ((Entry.Flags & ItemFlags.PlayerVsPlayer) != 0)
            {
                builder.Properties[Property.PvPOffensiveRating] = v25 * formulaEntry2.Datafloat02;
                builder.Properties[Property.PvPDefensiveRating] = v25 * formulaEntry2.Datafloat03;
            }

            float v19 = (support + 1.0f) * 0.5f;
            float v22 = v14 - v25;

            builder.Properties[Property.AssaultRating] = (1.0f - v19) * v22;
            builder.Properties[Property.SupportRating] = v19 * v22;
        }

        /// <summary>
        /// Calculate item armor property.
        /// </summary>
        private void CalculateArmor(ItemInfoPropertyBuilder builder)
        {
            if (FamilyEntry.Id != 1u)
                return;

            GameFormulaEntry formulaEntry = Entry.PowerLevel switch
            {
                < 50u  => GameTableManager.Instance.GameFormula.GetEntry(547),
                < 120u => GameTableManager.Instance.GameFormula.GetEntry(1286),
                _      => GameTableManager.Instance.GameFormula.GetEntry(1287)
            };

            float armor = ((formulaEntry.Datafloat0 * formulaEntry.Datafloat03) + (formulaEntry.Datafloat02 * (Entry.PowerLevel - formulaEntry.Dataint0))) * (CategoryEntry.ArmorModifier * SlotEntry.ArmorModifier);
            builder.Properties[Property.Armor] = armor;
        }

        /// <summary>
        /// Calculate item max health property.
        /// </summary>
        private void CalculateMaxHealth(ItemInfoPropertyBuilder builder)
        {
            if (FamilyEntry.Id != 1u)
                return;

            GameFormulaEntry formulaEntry = GameTableManager.Instance.GameFormula.GetEntry(67);

            float health = ((formulaEntry.Datafloat03 * formulaEntry.Datafloat0) + (Entry.PowerLevel * formulaEntry.Datafloat02)) * SlotEntry.ItemLevelModifier;
            builder.Properties[Property.BaseHealth] = health;
        }

        /// <summary>
        /// Returns if item can be equipped into an item slot.
        /// </summary>
        public bool IsEquippable()
        {
            return SlotEntry != null && SlotEntry.EquippedSlotFlags != 0u;
        }

        /// <summary>
        /// Returns if item can be equipped into item slot <see cref="EquippedItem"/>.
        /// </summary>
        public bool IsEquippableIntoSlot(EquippedItem bagIndex)
        {
            return (SlotEntry?.EquippedSlotFlags & (1u << (int)bagIndex)) != 0;
        }

        /// <summary>
        /// Returns if item can be stacked with other items of the same type.
        /// </summary>
        public bool IsStackable()
        {
            // TODO: Figure out other non-stackable items, which have MaxStackCount > 1
            return !IsEquippableBag() && Entry.MaxStackCount > 1u;
        }

        /// <summary>
        /// Returns if item can be used as a bag for expanding inventory slots.
        /// </summary>
        public bool IsEquippableBag()
        {
            // client checks this flag to show bag tutorial, should be enough
            return (FamilyEntry.Flags & SecondaryItemFlags.Bag) != 0;
        }
    }
}
