using System;
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
using NLog;

namespace NexusForever.WorldServer.Game
{
    public sealed class AssetManager : Singleton<AssetManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

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

        private ImmutableDictionary</*targetGroupId*/uint, /*targetGroupIds*/ImmutableList<uint>> questObjectiveTargets;

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
            CacheQuestObjectiveTargetGroups();
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

        private void CacheQuestObjectiveTargetGroups()
        {
            List<TargetGroupType> unhandledTargetGroups = new List<TargetGroupType>();

            void AddToTargets(TargetGroupEntry entry, ref List<uint> targetIds)
            {
                switch ((TargetGroupType)entry.Type)
                {
                    case TargetGroupType.CreatureIdGroup:
                        targetIds.AddRange(entry.DataEntries.Where(d => d != 0u));
                        break;
                    case TargetGroupType.OtherTargetGroup:
                        foreach (uint targetGroupId in entry.DataEntries.Where(d => d != 0u))
                        {
                            TargetGroupEntry targetGroup = GameTableManager.Instance.TargetGroup.GetEntry(targetGroupId);
                            if (targetGroup == null)
                                throw new InvalidOperationException();

                            AddToTargets(targetGroup, ref targetIds);
                        }
                        break;
                    default:
                        if (!(unhandledTargetGroups.Contains((TargetGroupType)entry.Type)))
                            unhandledTargetGroups.Add((TargetGroupType)entry.Type);
                        break;
                }
            }

            var entries = ImmutableDictionary.CreateBuilder<uint, List<uint>>();
            foreach (QuestObjectiveEntry questObjectiveEntry in GameTableManager.Instance.QuestObjective.Entries
                .Where(o => o.TargetGroupIdRewardPane > 0u ||
                    (QuestObjectiveType)o.Type == QuestObjectiveType.ActivateTargetGroup ||
                    (QuestObjectiveType)o.Type == QuestObjectiveType.ActivateTargetGroupChecklist ||
                    (QuestObjectiveType)o.Type == QuestObjectiveType.KillTargetGroup ||
                    (QuestObjectiveType)o.Type == QuestObjectiveType.KillTargetGroups ||
                    (QuestObjectiveType)o.Type == QuestObjectiveType.TalkToTargetGroup))
            {
                uint targetGroupId = questObjectiveEntry.Data > 0 ? questObjectiveEntry.Data : questObjectiveEntry.TargetGroupIdRewardPane;
                if (targetGroupId == 0u)
                    continue;

                TargetGroupEntry targetGroup = GameTableManager.Instance.TargetGroup.GetEntry(targetGroupId);
                if (targetGroup == null)
                    continue;

                List<uint> targetIds = new List<uint>();
                AddToTargets(targetGroup, ref targetIds);
                entries.Add(questObjectiveEntry.Id, targetIds);
            }

            questObjectiveTargets = entries.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());

            string targetGroupTypes = "";
            foreach (TargetGroupType targetGroupType in unhandledTargetGroups)
                targetGroupTypes += targetGroupType.ToString() + " ";

            log.Warn($"Unhandled TargetGroup Types for Quest Objectives: {targetGroupTypes}");
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
        /// Returns an <see cref="ImmutableList{T}"/> containing all target ID's associated with the questObjectiveId.
        /// </summary>
        public ImmutableList<uint> GetQuestObjectiveTargetIds(uint questObjectiveId)
        {
            return questObjectiveTargets.TryGetValue(questObjectiveId, out ImmutableList<uint> entries) ? entries : Enumerable.Empty<uint>().ToImmutableList();
        }
    }
}
