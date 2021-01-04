using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NexusForever.Database.World.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Quest.Static;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Game
{
    public sealed class AssetManager : Singleton<AssetManager>
    {
        public static ImmutableDictionary<InventoryLocation, uint> InventoryLocationCapacities { get; private set; }

        /// <summary>
        /// Id to be assigned to the next created character.
        /// </summary>
        public ulong NextCharacterId => nextCharacterId++;

        /// <summary>
        /// Id to be assigned to the next created item.
        /// </summary>
        public ulong NextItemId => nextItemId++;

        /// <summary>
        /// Id to be assigned to the next created mail.
        /// </summary>
        public ulong NextMailId => nextMailId++;

        private ulong nextCharacterId;
        private ulong nextItemId;
        private ulong nextMailId;

        private ImmutableDictionary<uint, ImmutableList<CharacterCustomizationEntry>> characterCustomisations;

        private ImmutableDictionary<ItemSlot, ImmutableList<EquippedItem>> equippedItems;
        private ImmutableDictionary<uint, ImmutableList<ItemDisplaySourceEntryEntry>> itemDisplaySourcesEntry;

        private ImmutableDictionary</*zoneId*/uint, /*tutorialId*/uint> zoneTutorials;
        private ImmutableDictionary</*creatureId*/uint, /*targetGroupIds*/ImmutableList<uint>> creatureAssociatedTargetGroups;

        private ImmutableDictionary<AccountTier, ImmutableList<RewardPropertyPremiumModifierEntry>> rewardPropertiesByTier;

        private AssetManager()
        {
        }

        public void Initialise()
        {
            nextCharacterId = DatabaseManager.Instance.CharacterDatabase.GetNextCharacterId() + 1ul;
            nextItemId      = DatabaseManager.Instance.CharacterDatabase.GetNextItemId() + 1ul;
            nextMailId      = DatabaseManager.Instance.CharacterDatabase.GetNextMailId() + 1ul;

            CacheCharacterCustomisations();
            CacheInventoryEquipSlots();
            CacheInventoryBagCapacities();
            CacheItemDisplaySourceEntries();
            CacheTutorials();
            CacheCreatureTargetGroups();
            CacheRewardPropertiesByTier();
        }

        private void CacheCharacterCustomisations()
        {
            var entries = new Dictionary<uint, List<CharacterCustomizationEntry>>();
            foreach (CharacterCustomizationEntry entry in GameTableManager.Instance.CharacterCustomization.Entries)
            {
                uint primaryKey = (entry.Value00 << 24) | (entry.CharacterCustomizationLabelId00 << 16) | (entry.Gender << 8) | entry.RaceId;
                if (!entries.ContainsKey(primaryKey))
                    entries.Add(primaryKey, new List<CharacterCustomizationEntry>());

                entries[primaryKey].Add(entry);
            }

            characterCustomisations = entries.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
        }

        private void CacheInventoryEquipSlots()
        {
            var entries = new Dictionary<ItemSlot, List<EquippedItem>>();
            foreach (FieldInfo field in typeof(ItemSlot).GetFields())
            {
                foreach (EquippedItemAttribute attribute in field.GetCustomAttributes<EquippedItemAttribute>())
                {
                    ItemSlot slot = (ItemSlot)field.GetValue(null);
                    if (!entries.ContainsKey(slot))
                        entries.Add(slot, new List<EquippedItem>());

                    entries[slot].Add(attribute.Slot);
                }
            }

            equippedItems = entries.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
        }

        public void CacheInventoryBagCapacities()
        {
            var entries = new Dictionary<InventoryLocation, uint>();
            foreach (FieldInfo field in typeof(InventoryLocation).GetFields())
            {
                foreach (InventoryLocationAttribute attribute in field.GetCustomAttributes<InventoryLocationAttribute>())
                {
                    InventoryLocation location = (InventoryLocation)field.GetValue(null);
                    entries.Add(location, attribute.DefaultCapacity);
                }
            }

            InventoryLocationCapacities = entries.ToImmutableDictionary();
        }

        private void CacheItemDisplaySourceEntries()
        {
            var entries = new Dictionary<uint, List<ItemDisplaySourceEntryEntry>>();
            foreach (ItemDisplaySourceEntryEntry entry in GameTableManager.Instance.ItemDisplaySourceEntry.Entries)
            {
                if (!entries.ContainsKey(entry.ItemSourceId))
                    entries.Add(entry.ItemSourceId, new List<ItemDisplaySourceEntryEntry>());

                entries[entry.ItemSourceId].Add(entry);
            }

            itemDisplaySourcesEntry = entries.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
        }

        private void CacheTutorials()
        {
            var zoneEntries =  ImmutableDictionary.CreateBuilder<uint, uint>();
            foreach (TutorialModel tutorial in DatabaseManager.Instance.WorldDatabase.GetTutorialTriggers())
            {
                if (tutorial.TriggerId == 0) // Don't add Tutorials with no trigger ID
                    continue;

                if (tutorial.Type == 29 && !zoneEntries.ContainsKey(tutorial.TriggerId))
                    zoneEntries.Add(tutorial.TriggerId, tutorial.Id);
            }

            zoneTutorials = zoneEntries.ToImmutable();
        }

        private void CacheCreatureTargetGroups()
        {
            var entries = ImmutableDictionary.CreateBuilder<uint, List<uint>>();
            foreach (TargetGroupEntry entry in GameTableManager.Instance.TargetGroup.Entries)
            {
                if ((TargetGroupType)entry.Type != TargetGroupType.CreatureIdGroup)
                    continue;

                foreach (uint creatureId in entry.DataEntries)
                {
                    if (!entries.ContainsKey(creatureId))
                        entries.Add(creatureId, new List<uint>());

                    entries[creatureId].Add(entry.Id);
                }
            }

            creatureAssociatedTargetGroups = entries.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
        }

        private void CacheRewardPropertiesByTier()
        {
            // VIP was intended to be used in China from what I can see, you can force the VIP premium system in the client with the China game mode parameter
            // not supported as the system was unfinished
            IEnumerable<RewardPropertyPremiumModifierEntry> hybridEntries = GameTableManager.Instance
                .RewardPropertyPremiumModifier.Entries
                .Where(e => (PremiumSystem)e.PremiumSystemEnum == PremiumSystem.Hybrid)
                .ToList();

            // base reward properties are determined by current account tier and lower if fall through flag is set
            rewardPropertiesByTier = hybridEntries
                .Select(e => e.Tier)
                .Distinct()
                .ToImmutableDictionary(k => (AccountTier)k, k => hybridEntries
                    .Where(r => r.Tier == k)
                    .Concat(hybridEntries
                        .Where(r => r.Tier < k && ((RewardPropertyPremiumModiferFlags)r.Flags & RewardPropertyPremiumModiferFlags.FallThrough) != 0))
                    .ToImmutableList());
        }

        /// <summary>
        /// Returns an <see cref="ImmutableList{T}"/> containing all <see cref="CharacterCustomizationEntry"/>'s for the supplied race, sex, label and value.
        /// </summary>
        public ImmutableList<CharacterCustomizationEntry> GetPrimaryCharacterCustomisation(uint race, uint sex, uint label, uint value)
        {
            uint key = (value << 24) | (label << 16) | (sex << 8) | race;
            return characterCustomisations.TryGetValue(key, out ImmutableList<CharacterCustomizationEntry> entries) ? entries : null;
        }

        /// <summary>
        /// Returns an <see cref="ImmutableList{T}"/> containing all <see cref="EquippedItem"/>'s for supplied <see cref="ItemSlot"/>.
        /// </summary>
        public ImmutableList<EquippedItem> GetEquippedBagIndexes(ItemSlot slot)
        {
            return equippedItems.TryGetValue(slot, out ImmutableList<EquippedItem> entries) ? entries : null;
        }

        /// <summary>
        /// Returns an <see cref="ImmutableList{T}"/> containing all <see cref="ItemDisplaySourceEntryEntry"/>'s for the supplied itemSource.
        /// </summary>
        public ImmutableList<ItemDisplaySourceEntryEntry> GetItemDisplaySource(uint itemSource)
        {
            return itemDisplaySourcesEntry.TryGetValue(itemSource, out ImmutableList<ItemDisplaySourceEntryEntry> entries) ? entries : null;
        }

        /// <summary>
        /// Returns a Tutorial ID if it's found in the Zone Tutorials cache
        /// </summary>
        public uint GetTutorialIdForZone(uint zoneId)
        {
            return zoneTutorials.TryGetValue(zoneId, out uint tutorialId) ? tutorialId : 0;
        }

        /// <summary>
        /// Returns an <see cref="ImmutableList{T}"/> containing all TargetGroup ID's associated with the creatureId.
        /// </summary>
        public ImmutableList<uint> GetTargetGroupsForCreatureId(uint creatureId)
        {
            return creatureAssociatedTargetGroups.TryGetValue(creatureId, out ImmutableList<uint> entries) ? entries : null;
        }

        /// <summary>
        /// Returns an <see cref="ImmutableList{T}"/> containing all <see cref="RewardPropertyPremiumModifierEntry"/> for the given <see cref="AccountTier"/>.
        /// </summary>
        public ImmutableList<RewardPropertyPremiumModifierEntry> GetRewardPropertiesForTier(AccountTier tier)
        {
            return rewardPropertiesByTier.TryGetValue(tier, out ImmutableList<RewardPropertyPremiumModifierEntry> entries) ? entries : ImmutableList<RewardPropertyPremiumModifierEntry>.Empty;
        }
    }
}
