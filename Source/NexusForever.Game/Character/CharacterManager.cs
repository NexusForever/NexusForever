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

        private ImmutableList<IPropertyModifier> characterBaseProperties;
        private ImmutableDictionary<Class, ImmutableList<IPropertyModifier>> characterClassBaseProperties;

        private readonly Dictionary<ulong, ICharacter> characters = new();
        private readonly Dictionary<string, ulong> characterNameToId = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Called to Initialise the <see cref="CharacterManager"/> at server start
        /// </summary>
        public void Initialise()
        {
            nextCharacterId = DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetNextCharacterId() + 1ul;

            CacheCharacterCreate();

            CacheCharacterBaseProperties();
            CacheCharacterClassBaseProperties();

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

        private void CacheCharacterBaseProperties()
        {
            var entries = ImmutableList.CreateBuilder<IPropertyModifier>();
            foreach (PropertyBaseModel propertyModel in DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetProperties(0))
            {
                var newPropValue = new PropertyModifier((Property)propertyModel.Property, (ModType)propertyModel.ModType, propertyModel.Value);
                entries.Add(newPropValue);
            }

            characterBaseProperties = entries.ToImmutable();
        }

        private void CacheCharacterClassBaseProperties()
        {
            var entries = ImmutableDictionary.CreateBuilder<Class, ImmutableList<IPropertyModifier>>();

            foreach (IGrouping<uint, PropertyBaseModel> group in DatabaseManager.Instance.GetDatabase<CharacterDatabase>()
                .GetProperties(1)
                .GroupBy(p => p.Subtype)) // class
            {
                var propertyEntries = ImmutableList.CreateBuilder<IPropertyModifier>();

                foreach (PropertyBaseModel propertyModel in group)
                {
                    var newPropValue = new PropertyModifier((Property)propertyModel.Property, (ModType)propertyModel.ModType, propertyModel.Value);
                    propertyEntries.Add(newPropValue);
                }

                entries.Add((Class)group.Key, propertyEntries.ToImmutable());
            }

            characterClassBaseProperties = entries.ToImmutable();
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
        /// Returns a collection containing all base <see cref="IPropertyValue"/> for any character.
        /// </summary>
        public IEnumerable<IPropertyModifier> GetCharacterBaseProperties()
        {
            return characterBaseProperties;
        }

        /// <summary>
        /// Returns a collection containing all base <see cref="IPropertyValue"/> for a character class.
        /// </summary>
        public IEnumerable<IPropertyModifier> GetCharacterClassBaseProperties(Class @class)
        {
            return characterClassBaseProperties.TryGetValue(@class, out ImmutableList<IPropertyModifier> propertyValues) ? propertyValues : Enumerable.Empty<IPropertyModifier>();
        }
    }
}
