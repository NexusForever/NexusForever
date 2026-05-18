using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Account;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game
{
    public sealed class CleanupManager : Singleton<CleanupManager>, ICleanupManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ulong, IPlayer> pending = new();
        private readonly HashSet<uint> lockedAccounts = new();

        /// <summary>
        /// Start tracking supplied <see cref="IPlayer"/> for pending cleanup.
        /// </summary>
        public void AddPlayer(IPlayer player)
        {
            lockedAccounts.Add(player.Account.Id);
            pending.Add(player.CharacterId, player);

            log.Trace($"Added player {player.CharacterId}.");
        }

        /// <summary>
        /// Stops tracking supplied <see cref="IPlayer"/> for pending cleanup.
        /// </summary>
        public void RemovePlayer(IPlayer player)
        {
            pending.Remove(player.CharacterId);
            lockedAccounts.Remove(player.Account.Id);

            log.Trace($"Removed player {player.CharacterId}.");
        }

        /// <summary>
        /// Returns if supplied <see cref="IAccount"/> is locked due to pending <see cref="IPlayer"/> cleanup.
        /// </summary>
        public bool IsAccountLocked(IAccount account)
        {
            return lockedAccounts.Contains(account.Id);
        }
    }
}
