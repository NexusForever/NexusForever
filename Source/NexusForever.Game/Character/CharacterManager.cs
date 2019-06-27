using System.Collections.Immutable;
using System.Numerics;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Character
{
    public sealed class CharacterManager : Singleton<CharacterManager>, ICharacterManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Id to be assigned to the next created character.
        /// </summary>
        public ulong NextCharacterId => nextCharacterId++;

        private ulong nextCharacterId;

        private ImmutableDictionary<(Race, Faction, CharacterCreationStart), ILocation> characterCreationData;
        private ImmutableDictionary<uint, ImmutableList<CharacterCustomizationEntry>> characterCustomisations;

        private readonly Dictionary<ulong, ICharacter> characters = new();
        private readonly Dictionary<string, ulong> characterNameToId = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Called to Initialise the <see cref="CharacterManager"/> at server start
        /// </summary>
        public void Initialise()
        {
            nextCharacterId = DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetNextCharacterId() + 1ul;

            CacheCharacterCreate();
            CacheCharacterCustomisations();

            BuildCharacterInfoFromDb();
        }

        private void CacheCharacterCreate()
        {
            var entries = ImmutableDictionary.CreateBuilder<(Race, Faction, CharacterCreationStart), ILocation>();
            foreach (CharacterCreateModel model in DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetCharacterCreationData())
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
                uint primaryKey;
                if (entry.CharacterCustomizationLabelId00 == 0 && entry.CharacterCustomizationLabelId01 > 0)
                    primaryKey = (entry.Value01 << 24) | (entry.CharacterCustomizationLabelId01 << 16) | (entry.Gender << 8) | entry.RaceId;
                else
                    primaryKey = (entry.Value00 << 24) | (entry.CharacterCustomizationLabelId00 << 16) | (entry.Gender << 8) | entry.RaceId;

                if (!entries.ContainsKey(primaryKey))
                    entries.Add(primaryKey, new List<CharacterCustomizationEntry>());

                entries[primaryKey].Add(entry);
            }

            characterCustomisations = entries.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
        }

        /// <summary>
        /// Adds <see cref="CharacterModel"/> data from the database to the cache.
        /// </summary>
        private void BuildCharacterInfoFromDb()
        {
            List<CharacterModel> allCharactersInDb = DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetAllCharacters();
            foreach (CharacterModel character in allCharactersInDb)
                AddCharacter(character);

            log.Info($"Stored {characters.Count} characters in Character Cache");
        }

        public void AddCharacter(CharacterModel character)
        {
            AddCharacter(character.Id, new Character(character));
        }

        /// <summary>
        /// Add <see cref="ICharacter"/> to the cache with associated ID.
        /// </summary>
        private void AddCharacter(ulong characterId, ICharacter character)
        {
            characters.TryAdd(characterId, character);
            characterNameToId.Add(character.Name, characterId);
        }

        /// <summary>
        /// Used to delete a <see cref="ICharacter"/> from the cache when the <see cref="CharacterModel"/> is deleted.
        /// </summary>
        public void DeleteCharacter(ulong id, string name)
        {
            if (!characters.ContainsKey(id))
                throw new ArgumentNullException(nameof(id));

            characters.Remove(id, out ICharacter character);
            if (character == null)
                throw new ArgumentNullException();

            characterNameToId.Remove(name);

            log.Trace($"Removed character {character.Name} (ID: {id}) from the cache due to player delete.");
        }

        /// <summary>
        /// Returns a <see cref="bool"/> whether there is an <see cref="ICharacter"/> that exists with the name passed in.
        /// </summary>
        public bool IsCharacter(string name)
        {
            return characterNameToId.TryGetValue(name, out ulong value);
        }

        /// <summary>
        /// Returns the character ID of a player with the name passed in.
        /// </summary>
        public ulong? GetCharacterIdByName(string name)
        {
            return characterNameToId.TryGetValue(name, out ulong characterId) ? characterId : 0;
        }

        /// <summary>
        /// Returns an <see cref="ICharacter"/> instance that matches the passed character ID, should one exist.
        /// </summary>
        public ICharacter GetCharacter(ulong characterId)
        {
            return characters.TryGetValue(characterId, out ICharacter characterInfo) ? characterInfo : null;
        }

        /// <summary>
        /// Returns an <see cref="ICharacter"/> instance that matches the passed character name, should one exist.
        /// </summary>
        public ICharacter GetCharacter(string name)
        {
            return characterNameToId.TryGetValue(name, out ulong characterId) ? GetCharacter(characterId) : null;
        }

        /// <summary>
        /// Returns a <see cref="ILocation"/> describing the starting location for a given <see cref="Race"/>, <see cref="Faction"/> and Creation Type combination.
        /// </summary>
        public ILocation GetStartingLocation(Race race, Faction faction, CharacterCreationStart creationStart)
        {
            return characterCreationData.TryGetValue((race, faction, creationStart), out ILocation location) ? location : null;
        }

        /// <summary>
        /// Returns matching <see cref="CharacterCustomizationEntry"/> given input parameters
        /// </summary>
        public IEnumerable<CharacterCustomizationEntry> GetCharacterCustomisation(Dictionary<uint, uint> customisations, uint race, uint sex, uint primaryLabel, uint primaryValue)
        {
            ImmutableList<CharacterCustomizationEntry> entries = GetPrimaryCharacterCustomisation(race, sex, primaryLabel, primaryValue);
            if (entries == null)
                return Enumerable.Empty<CharacterCustomizationEntry>();

            List<CharacterCustomizationEntry> customizationEntries = new List<CharacterCustomizationEntry>();

            // Customisation has multiple results, filter with a non-zero secondary KvP.
            List<CharacterCustomizationEntry> primaryEntries = entries.Where(e => e.CharacterCustomizationLabelId01 != 0).ToList();
            if (primaryEntries.Count > 0)
            {
                // This will check all entries where there is a primary AND secondary KvP.
                foreach (CharacterCustomizationEntry customizationEntry in primaryEntries)
                {
                    // Missing primary KvP in table, skipping.
                    if (customizationEntry.CharacterCustomizationLabelId00 == 0)
                        continue;

                    // Secondary KvP not found in customisation list, skipping.
                    if (!customisations.ContainsKey(customizationEntry.CharacterCustomizationLabelId01))
                        continue;

                    // Returning match found for primary KvP and secondary KvP
                    if (customisations[customizationEntry.CharacterCustomizationLabelId01] == customizationEntry.Value01)
                        customizationEntries.Add(customizationEntry);
                }

                // Return the matching value when the primary KvP matching the table's secondary KvP
                CharacterCustomizationEntry entry = entries.FirstOrDefault(e => e.CharacterCustomizationLabelId01 == primaryLabel && e.Value01 == primaryValue);
                if (entry != null)
                    customizationEntries.Add(entry);
            }
            
            if (customizationEntries.Count == 0)
            {
                // Return the matching value when the primary KvP matches the table's primary KvP, and no secondary KvP is present.
                CharacterCustomizationEntry entry = entries.FirstOrDefault(e => e.CharacterCustomizationLabelId00 == primaryLabel && e.Value00 == primaryValue);
                if (entry != null)
                    customizationEntries.Add(entry);
                else
                {
                    entry = entries.Single(e => e.CharacterCustomizationLabelId01 == 0 && e.Value01 == 0);
                    if (entry != null)
                        customizationEntries.Add(entry);
                }
            }

            // Ensure we only return 1 entry per ItemSlot.
            return customizationEntries.GroupBy(i => i.ItemSlotId).Select(i => i.First());
        }

        /// <summary>
        /// Returns an <see cref="ImmutableList{T}"/> containing all <see cref="CharacterCustomizationEntry"/>'s for the supplied race, sex, label and value.
        /// </summary>
        public ImmutableList<CharacterCustomizationEntry> GetPrimaryCharacterCustomisation(uint race, uint sex, uint label, uint value)
        {
            uint key = (value << 24) | (label << 16) | (sex << 8) | race;
            return characterCustomisations.TryGetValue(key, out ImmutableList<CharacterCustomizationEntry> entries) ? entries : null;
        }
    }
}
