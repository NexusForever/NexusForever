using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.WorldServer.Game.Entity;
using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Game.CharacterCache
{
    public sealed class CharacterManager : AbstractManager<CharacterManager>
    {
        private readonly Dictionary<ulong, ICharacter> characters = new Dictionary<ulong, ICharacter>();
        private readonly Dictionary<string, ulong> characterNameToId = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);

        private CharacterManager()
        {
        }

        /// <summary>
        /// Called to Initialise the <see cref="CharacterManager"/> at server start
        /// </summary>
        public override CharacterManager Initialise()
        {
            BuildCharacterInfoFromDb();
            return Instance;
        }

        /// <summary>
        /// Adds <see cref="CharacterModel"/> data from the database to the cache
        /// </summary>
        private void BuildCharacterInfoFromDb()
        {
            List<CharacterModel> allCharactersInDb = DatabaseManager.Instance.CharacterDatabase.GetAllCharacters();
            foreach (CharacterModel character in allCharactersInDb)
                AddPlayer(character.Id, new CharacterInfo(character));

            Log.Info($"Stored {characters.Count} characters in Character Cache");
        }

        /// <summary>
        /// Add <see cref="ICharacter"/> to the cache with associated ID
        /// </summary>
        private void AddPlayer(ulong characterId, ICharacter character)
        {
            characters.TryAdd(characterId, character);
            characterNameToId.Add(character.Name, characterId);
        }

        /// <summary>
        /// Used to register a <see cref="Player"/> in the cache in place of an existing one or as an addition. Used to provide real time data to the client of a player. Should be used on player login.
        /// </summary>
        public void RegisterPlayer(Player player)
        {
            if (characters.ContainsKey(player.CharacterId))
                characters[player.CharacterId] = player;
            else
                AddPlayer(player.CharacterId, player);

            Log.Trace($"{player.Name} (ID: {player.CharacterId}) logged in.");
        }

        /// <summary>
        /// Used to deregister a <see cref="Player"/> from the cache and build a snapshot of the data. Should be used on player logout to keep the server's character data consistent.
        /// </summary>
        public void DeregisterPlayer(Player player)
        {
            if (!characters.ContainsKey(player.CharacterId))
                throw new Exception($"{player.CharacterId} should exist in characters dictionary.");

            characters[player.CharacterId] = new CharacterInfo(player);

            Log.Trace($"{player.Name} (ID: {player.CharacterId}) logged out.");
        }

        /// <summary>
        /// Used to delete a <see cref="ICharacter"/> from the cache when the <see cref="CharacterModel"/> is deleted
        /// </summary>
        public void DeleteCharacter(ulong id, string name)
        {
            if (!characters.ContainsKey(id))
                throw new ArgumentNullException(nameof(id));

            characters.Remove(id, out ICharacter character);
            if (character == null)
                throw new ArgumentNullException();

            characterNameToId.Remove(name);

            Log.Trace($"Removed character {character.Name} (ID: {id}) from the cache due to player delete.");
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
        public ulong GetCharacterIdByName(string name)
        {
            return characterNameToId.TryGetValue(name, out ulong characterId) ? characterId : 0;
        }

        /// <summary>
        /// Returns an <see cref="ICharacter"/> instance that matches the passed character ID, should one exist.
        /// </summary>
        public ICharacter GetCharacterInfo(ulong characterId)
        {
            return characters.TryGetValue(characterId, out ICharacter characterInfo) ? characterInfo : null;
        }

        /// <summary>
        /// Returns an <see cref="ICharacter"/> instance that matches the passed character name, should one exist.
        /// </summary>
        public ICharacter GetCharacterInfo(string name)
        {
            return characterNameToId.TryGetValue(name, out ulong characterId) ? GetCharacterInfo(characterId) : null;
        }

        /// <summary>
        /// Returns a <see cref="Player"/> for the supplied name.
        /// </summary>
        public Player GetPlayer(string name)
        {
            return characterNameToId.TryGetValue(name, out ulong characterId) ? GetPlayer(characterId) : null;
        }

        /// <summary>
        /// Returns a <see cref="Player"/> for the character id.
        /// </summary>
        public Player GetPlayer(ulong characterId)
        {
            ICharacter character = GetCharacterInfo(characterId);
            if (character is Player player)
                return player;

            return null;
        }
    }
}
