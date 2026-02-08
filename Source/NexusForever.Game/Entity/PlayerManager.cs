using System.Collections;
using System.Collections.Concurrent;
using NexusForever.Game.Abstract;
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

        private readonly ConcurrentDictionary<Identity, IPlayer> players = new();

        /// <summary>
        /// Add new <see cref="IPlayer"/>.
        /// </summary>
        public void AddPlayer(IPlayer player)
        {
            players.TryAdd(player.Identity, player);
            log.Trace($"Added player {player.Identity}.");
        }

        /// <summary>
        /// Remove existing <see cref="IPlayer"/>.
        /// </summary>
        public void RemovePlayer(IPlayer player)
        {
            players.TryRemove(player.Identity, out _);
            log.Trace($"Removed player {player.Identity}.");
        }

        /// <summary>
        /// Returns <see cref="IPlayer"/> with supplied character id. Assumes <see cref="IPlayer"/> is in the same realm.
        /// </summary>
        public IPlayer GetPlayer(ulong characterId)
        {
            return players.TryGetValue( new Identity{ Id = characterId, RealmId = RealmContext.Instance.RealmId }, out IPlayer player) ? player : null;
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

        /// <summary>
        /// Returns <see cref="IPlayer"/> with supplied identity.
        /// </summary>
        public IPlayer GetPlayer(Identity identity)
        {
            return players.TryGetValue(identity, out IPlayer player) ? player : null;
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
