using System.Collections;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Character;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Entity
{
    public sealed class PlayerManager : Singleton<PlayerManager>, IPlayerManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ulong, IPlayer> players = new();

        /// <summary>
        /// Add new <see cref="IPlayer"/>.
        /// </summary>
        public void AddPlayer(IPlayer player)
        {
            players.Add(player.CharacterId, player);
            log.Trace($"Added player {player.CharacterId}.");
        }

        /// <summary>
        /// Remove existing <see cref="IPlayer"/>.
        /// </summary>
        public void RemovePlayer(IPlayer player)
        {
            players.Remove(player.CharacterId);
            log.Trace($"Removed player {player.CharacterId}.");
        }

        /// <summary>
        /// Returns <see cref="IPlayer"/> with supplied character id.
        /// </summary>
        public IPlayer GetPlayer(ulong characterId)
        {
            return players.TryGetValue(characterId, out IPlayer player) ? player : null;
        }

        /// <summary>
        /// Return <see cref="IPlayer"/> with supplied character name.
        /// </summary>
        public IPlayer GetPlayer(string name)
        {
            ICharacter character = CharacterManager.Instance.GetCharacter(name);
            if (character == null)
                return null;

            return GetPlayer(character.CharacterId);
        }

        public IEnumerator<IPlayer> GetEnumerator()
        {
            return players.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
