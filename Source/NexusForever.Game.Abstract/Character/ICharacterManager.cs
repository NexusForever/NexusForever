using System.Collections.Immutable;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Character
{
    public interface ICharacterManager
    {
        /// <summary>
        /// Id to be assigned to the next created character.
        /// </summary>
        ulong NextCharacterId { get; }

        /// <summary>
        /// Called to Initialise the <see cref="ICharacterManager"/> at server start.
        /// </summary>
        void Initialise();

        void AddCharacter(CharacterModel character);

        /// <summary>
        /// Used to delete a <see cref="ICharacter"/> from the cache when the <see cref="CharacterModel"/> is deleted.
        /// </summary>
        void DeleteCharacter(ulong id, string name);

        /// <summary>
        /// Returns a <see cref="bool"/> whether there is an <see cref="ICharacter"/> that exists with the name passed in.
        /// </summary>
        bool IsCharacter(string name);

        /// <summary>
        /// Returns the character ID of a player with the name passed in.
        /// </summary>
        ulong? GetCharacterIdByName(string name);

        /// <summary>
        /// Returns an <see cref="ICharacter"/> instance that matches the passed character ID, should one exist.
        /// </summary>
        ICharacter GetCharacter(ulong characterId);

        /// <summary>
        /// Returns an <see cref="ICharacter"/> instance that matches the passed character name, should one exist.
        /// </summary>
        ICharacter GetCharacter(string name);

        /// <summary>
        /// Returns a <see cref="ILocation"/> describing the starting location for a given <see cref="Race"/>, <see cref="Faction"/> and Creation Type combination.
        /// </summary>
        ILocation GetStartingLocation(Race race, Faction faction, CharacterCreationStart creationStart);

        /// <summary>
        /// Returns an <see cref="ImmutableList[T]"/> containing all base <see cref="IPropertyValue"/> for any character
        /// </summary>
        ImmutableList<IPropertyModifier> GetCharacterBaseProperties();

        /// <summary>
        /// Returns an <see cref="ImmutableList[T]"/> containing all base <see cref="IPropertyValue"/> for a character class
        /// </summary>
        ImmutableList<IPropertyModifier> GetCharacterClassBaseProperties(Class @class);
    }
}