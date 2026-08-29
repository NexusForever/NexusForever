using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Game.Abstract;
using NexusForever.Game.Configuration.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;

namespace NexusForever.Game
{
    public sealed class RealmContext : Singleton<RealmContext>, IRealmContext
    {
        public ushort RealmId { get; private set; }
        public string RealmName { get; private set; }
        public string Motd { get; set; }

        private readonly TimeSpan serverTimeOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

        #region Dependency Injection

        private readonly IDatabaseManager databaseManager;

        public RealmContext(
            IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        #endregion

        public void Initialise()
        {
            RealmConfig config = SharedConfiguration.Instance.Get<RealmConfig>();
            RealmId = config.RealmId;
            Motd    = config.MessageOfTheDay;

            var server = databaseManager.GetDatabase<AuthDatabase>().GetServer(RealmId);
            if (server == null)
                throw new InvalidOperationException($"Realm with ID {RealmId} does not exist in the database.");

            RealmName = server.Name;
        }

        /// <summary>
        /// Get the current Server Time in FileTime format.
        /// </summary>
        public ulong GetServerTime()
        {
            return (ulong)DateTime.UtcNow.Add(serverTimeOffset).ToFileTime();
        }
    }
}
