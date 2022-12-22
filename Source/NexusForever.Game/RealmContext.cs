using NexusForever.Game.Configuration.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;

namespace NexusForever.Game;

public class RealmContext : Singleton<RealmContext>
{
    public ushort RealmId { get; private set; }
    public string Motd { get; set; }

    private static readonly TimeSpan serverTimeOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

    public void Initialise()
    {
        RealmConfig config = SharedConfiguration.Instance.Get<RealmConfig>();
        RealmId = config.RealmId;
        Motd = config.MessageOfTheDay;
    }

    /// <summary>
    /// Get the current Server Time in FileTime format.
    /// </summary>
    public ulong GetServerTime()
    {
        return (ulong)DateTime.UtcNow.Add(serverTimeOffset).ToFileTime();
    }
}
