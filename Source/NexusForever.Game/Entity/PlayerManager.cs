using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Shared;

namespace NexusForever.Game.Entity
{
    public sealed class PlayerManager : Singleton<PlayerManager>, IPlayerManager
    {
        private readonly ConcurrentDictionary<Identity, IPlayer> players = [];
        private readonly ConcurrentDictionary<uint, Identity> accountPlayer = [];

        #region Dependency Injection

        private readonly ILogger<PlayerManager> log;
        private readonly ICharacterManager characterManager;

        public PlayerManager(
            ILogger<PlayerManager> log,
            ICharacterManager characterManager)
        {
            this.log              = log;
            this.characterManager = characterManager;
        }

        #endregion

        /// <summary>
        /// Add new <see cref="IPlayer"/>.
        /// </summary>
        public void AddPlayer(IPlayer player)
        {
            if (!players.TryAdd(player.Identity, player))
                return;

            if (!accountPlayer.TryAdd(player.Account.Id, player.Identity))
                return;

            log.LogTrace($"Added player {player.Identity}.");
        }

        /// <summary>
        /// Remove existing <see cref="IPlayer"/>.
        /// </summary>
        public void RemovePlayer(IPlayer player)
        {
            if (!players.TryRemove(player.Identity, out _))
                return;

            if (!accountPlayer.TryRemove(player.Account.Id, out _))
                return;

            log.LogTrace($"Removed player {player.Identity}.");
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
            ICharacter character = characterManager.GetCharacter(name);
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

        /// <summary>
        /// Return <see cref="IPlayer"/> with supplied account id.
        /// </summary>
        public IPlayer GetPlayerByAccountId(uint accountId)
        {
            if (!accountPlayer.TryGetValue(accountId, out Identity identity))
                return null;

            return GetPlayer(identity);
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
