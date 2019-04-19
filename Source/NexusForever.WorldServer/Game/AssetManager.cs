using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Reflection;
using NexusForever.Database.Character.Model;
using NexusForever.Database.World.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Quest.Static;
using NexusForever.WorldServer.Game.Reputation.Static;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Game
{
    public sealed class AssetManager : Singleton<AssetManager>
    {
        public ImmutableDictionary<InventoryLocation, uint> InventoryLocationCapacities { get; private set; }

        /// <summary>
        /// Id to be assigned to the next created character.
        /// </summary>
        public ulong NextCharacterId => nextCharacterId++;

        /// <summary>
        /// Id to be assigned to the next created mail.
        /// </summary>
        public ulong NextMailId => nextMailId++;

        private ulong nextCharacterId;
        private ulong nextMailId;

        private ImmutableDictionary<(Race, Faction, CharacterCreationStart), Location> characterCreationData;
        private ImmutableDictionary<uint, ImmutableList<CharacterCustomizationEntry>> characterCustomisations;
        private ImmutableList<PropertyValue> characterBaseProperties;
        private ImmutableDictionary<Class, ImmutableList<PropertyValue>> characterClassBaseProperties;

        private ImmutableDictionary<uint, ImmutableList<ItemDisplaySourceEntryEntry>> itemDisplaySourcesEntry;
        private ImmutableDictionary<uint /*item2CategoryId*/, float /*modifier*/> itemArmorModifiers;
        private ImmutableDictionary<ItemSlot, ImmutableDictionary<Property, float>> innatePropertiesLevelScaling;
        private ImmutableDictionary<ItemSlot, ImmutableDictionary<Property, float>> innatePropertiesFlat;

        private ImmutableDictionary</*zoneId*/uint, /*tutorialId*/uint> zoneTutorials;
        private ImmutableDictionary</*creatureId*/uint, /*targetGroupIds*/ImmutableList<uint>> creatureAssociatedTargetGroups;

        private ImmutableDictionary<AccountTier, ImmutableList<RewardPropertyPremiumModifierEntry>> rewardPropertiesByTier;

        private AssetManager()
        {
        }

        public void Initialise()
        {
            nextCharacterId = DatabaseManager.Instance.CharacterDatabase.GetNextCharacterId() + 1ul;
            nextMailId      = DatabaseManager.Instance.CharacterDatabase.GetNextMailId() + 1ul;

            CacheCharacterCreate();
            CacheCharacterCustomisations();
            CacheCharacterBaseProperties();
            CacheCharacterClassBaseProperties();
            CacheInventoryBagCapacities();
            CacheItemDisplaySourceEntries();
            CacheItemArmorModifiers();
            CacheItemInnateProperties();
            CacheTutorials();
            CacheCreatureTargetGroups();
            CacheRewardPropertiesByTier();
        }

        private void CacheCharacterCreate()
        {
            var entries = ImmutableDictionary.CreateBuilder<(Race, Faction, CharacterCreationStart), Location>();
            foreach (CharacterCreateModel model in DatabaseManager.Instance.CharacterDatabase.GetCharacterCreationData())
            {
                entries.Add(((Race)model.Race, (Faction)model.Faction, (CharacterCreationStart)model.CreationStart), new Location
                (
                    GameTableManager.Instance.World.GetEntry(model.WorldId),
                    new Vector3
                    {
                        X = model.X,
                        Y = model.Y,
                        Z = model.Z
                    },
                    new Vector3
                    {
                        X = model.Rx,
                        Y = model.Ry,
                        Z = model.Rz
                    }
                ));
            }

            characterCreationData = entries.ToImmutable();
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

        private void CacheCharacterBaseProperties()
        {
            var entries = ImmutableList.CreateBuilder<PropertyValue>();
            foreach (PropertyBaseModel propertyModel in DatabaseManager.Instance.CharacterDatabase.GetProperties(0))
            {
                var newPropValue = new PropertyValue((Property)propertyModel.Property, propertyModel.Value, propertyModel.Value);
                entries.Add(newPropValue);
            }

            characterBaseProperties = entries.ToImmutable();
        }

        private void CacheCharacterClassBaseProperties()
        {
            ImmutableDictionary<Class, ImmutableList<PropertyValue>>.Builder entries = ImmutableDictionary.CreateBuilder<Class, ImmutableList<PropertyValue>>();
            var classList = GameTableManager.Instance.Class.Entries;

            List<PropertyBaseModel> classPropertyBases = DatabaseManager.Instance.CharacterDatabase.GetProperties(1);
            foreach (ClassEntry classEntry in classList)
            {
                Class @class = (Class)classEntry.Id;

                if (entries.ContainsKey(@class))
                    continue;

                ImmutableList<PropertyValue>.Builder propertyList = ImmutableList.CreateBuilder<PropertyValue>();
                foreach (PropertyBaseModel propertyModel in classPropertyBases)
                {
                    if (propertyModel.Subtype != (uint)@class)
                        continue;

                    var newPropValue = new PropertyValue((Property)propertyModel.Property, propertyModel.Value, propertyModel.Value);
                    propertyList.Add(newPropValue);
                }
                ImmutableList<PropertyValue> classProperties = propertyList.ToImmutable();

                entries.Add(@class, classProperties);
            }
            
            characterClassBaseProperties = entries.ToImmutable();
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
        
        private void CacheItemArmorModifiers()
        {
            var armorMods = ImmutableDictionary.CreateBuilder<uint, float>();
            foreach (Item2CategoryEntry entry in GameTableManager.Instance.Item2Category.Entries.Where(i => i.Item2FamilyId == 1))
                armorMods.Add(entry.Id, entry.ArmorModifier);

            itemArmorModifiers = armorMods.ToImmutable();
        }

        private void CacheItemInnateProperties()
        {
            ImmutableDictionary<ItemSlot, ImmutableDictionary<Property, float>>.Builder propFlat = ImmutableDictionary.CreateBuilder<ItemSlot, ImmutableDictionary<Property, float>>();
            ImmutableDictionary<ItemSlot, ImmutableDictionary<Property, float>>.Builder propScaling = ImmutableDictionary.CreateBuilder<ItemSlot, ImmutableDictionary<Property, float>>();

            foreach (var slot in DatabaseManager.Instance.CharacterDatabase.GetProperties(2).GroupBy(x => x.Subtype).Select(i => i.First()))
            {
                ImmutableDictionary<Property, float>.Builder subtypePropFlat = ImmutableDictionary.CreateBuilder<Property, float>();
                ImmutableDictionary<Property, float>.Builder subtypePropScaling = ImmutableDictionary.CreateBuilder<Property, float>();
                foreach (PropertyBaseModel propertyBase in DatabaseManager.Instance.CharacterDatabase.GetProperties(2).Where(i => i.Subtype == slot.Subtype))
                {
                    switch (propertyBase.ModType)
                    {
                        case 0:
                            subtypePropFlat.Add((Property)propertyBase.Property, propertyBase.Value);
                            break;
                        case 1:
                            subtypePropScaling.Add((Property)propertyBase.Property, propertyBase.Value);
                            break;
                    }
                }

                propFlat.Add((ItemSlot)slot.Subtype, subtypePropFlat.ToImmutable());
                propScaling.Add((ItemSlot)slot.Subtype, subtypePropScaling.ToImmutable());
            }

            innatePropertiesFlat = propFlat.ToImmutable();
            innatePropertiesLevelScaling = propScaling.ToImmutable();
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
        /// Returns an <see cref="ImmutableList[T]"/> containing all base <see cref="PropertyValue"/> for any character
        /// </summary>
        public ImmutableList<PropertyValue> GetCharacterBaseProperties()
        {
            return characterBaseProperties;
        }

        /// <summary>
        /// Returns an <see cref="ImmutableList[T]"/> containing all base <see cref="PropertyValue"/> for a character class
        /// </summary>
        public ImmutableList<PropertyValue> GetCharacterClassBaseProperties(Class @class)
        {
            return characterClassBaseProperties.TryGetValue(@class, out ImmutableList<PropertyValue> propertyValues) ? propertyValues : null;
        }

        /// <summary>
        /// Returns an <see cref="ImmutableList{T}"/> containing all <see cref="ItemDisplaySourceEntryEntry"/>'s for the supplied itemSource.
        /// </summary>
        public ImmutableList<ItemDisplaySourceEntryEntry> GetItemDisplaySource(uint itemSource)
        {
            return itemDisplaySourcesEntry.TryGetValue(itemSource, out ImmutableList<ItemDisplaySourceEntryEntry> entries) ? entries : null;
        }

        /// <summary>
        /// Returns a <see cref="Dictionary{TKey, TValue}"/> containing <see cref="Property"/> and associated values for given Item.
        /// </summary>
        public Dictionary<Property, float> GetInnateProperties(ItemSlot itemSlot, uint effectiveLevel, uint categoryId, float supportPowerPercentage)
        {
            Dictionary<Property, float> innateProperties = new Dictionary<Property, float>();

            var innatePropScaling = innatePropertiesLevelScaling.ContainsKey(itemSlot) ? innatePropertiesLevelScaling[itemSlot] : new Dictionary<Property, float>().ToImmutableDictionary();
            var innatePropFlat = innatePropertiesFlat.ContainsKey(itemSlot) ? innatePropertiesFlat[itemSlot] : new Dictionary<Property, float>().ToImmutableDictionary();

            // TODO: Shield reboot, max % and tick % are all the same right now. Investigate how these stats are calculated and add to method.
            foreach (KeyValuePair<Property, float> entry in innatePropFlat)
                innateProperties.TryAdd(entry.Key, entry.Value);

            foreach (KeyValuePair<Property, float> entry in innatePropScaling)
            {
                var value = entry.Value;

                if (entry.Key == Property.AssaultRating)
                {
                    if (supportPowerPercentage == 1f)
                        value = 0f;
                    else if (supportPowerPercentage == 0.5f)
                        value *= 0.3333f;
                }

                if (entry.Key == Property.SupportRating)
                {
                    if (supportPowerPercentage == -1f)
                        value = 0f;
                    else if (supportPowerPercentage == -0.5f)
                        value *= 0.3333f;
                }

                // TODO: Ensure correct values after 50 effective level. There are diminishing returns after 50 effective level to Armor.
                // At 51+ it changes to Effective Level 50 Amount + (Base Value * (EffectiveLevel - 50)).
                // i.e. Eff. Level 60 Medium Chest Armor: (50 * 25) + (8.5 * (60 - 50)) = 1335 (http://www.jabbithole.com/items/corruption-resistant-jacket-48097)
                if (entry.Key == Property.Armor)
                    if (itemArmorModifiers.TryGetValue(categoryId, out float armorMod))
                        value *= armorMod;

                if (innateProperties.ContainsKey(entry.Key))
                    innateProperties[entry.Key] = innateProperties[entry.Key] + (uint)Math.Floor(value * effectiveLevel);
                else
                    innateProperties.TryAdd(entry.Key, (uint)Math.Floor(value * effectiveLevel));
            }

            return innateProperties;
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

        /// <summary>
        /// Returns a <see cref="Location"/> describing the starting location for a given <see cref="Race"/>, <see cref="Faction"/> and Creation Type combination.
        /// </summary>
        public Location GetStartingLocation(Race race, Faction faction, CharacterCreationStart creationStart)
        {
            return characterCreationData.TryGetValue((race, faction, creationStart), out Location location) ? location : null;
        }
    }
}
