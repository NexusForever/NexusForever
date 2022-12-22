using System.Text;
using NexusForever.Game.Entity;
using NexusForever.Game.Network;
using NexusForever.Game.Social;
using NexusForever.Game.Static.Social;
using NexusForever.Network;
using NexusForever.Shared;
using NexusForever.Shared.Game;

namespace NexusForever.Game
{
    public class ShutdownManager : Singleton<ShutdownManager>, IUpdate
    {
        private UpdateTimer shutdownTick;
        private TimeSpan? shutdownSpan;

        private Action shutdown;

        /// <summary>
        /// Returns if realm is currently pending a shutdown.
        /// </summary>
        public bool IsShutdownPending => shutdownTick != null;

        private ShutdownManager()
        {
        }

        public void Initialise(Action callback)
        {
            if (shutdown != null)
                throw new InvalidOperationException();

            shutdown = callback;
        }

        /// <summary>
        /// Start a realm shutdown with the supplied <see cref="TimeSpan"/> to shutdown.
        /// </summary>
        public void StartShutdown(TimeSpan span)
        {
            if (shutdownSpan.HasValue)
                throw new InvalidOperationException();

            BroadcastMessage(FormatShutdownTime(span));

            TimeSpan nextTick = NextShutdownTick(span);
            shutdownSpan = span.Subtract(nextTick);
            shutdownTick = new UpdateTimer(nextTick);
        }

        /// <summary>
        /// Cancel a pending realm shutdown.
        /// </summary>
        public void CancelShutdown()
        {
            if (!shutdownSpan.HasValue)
                throw new InvalidOperationException();

            shutdownSpan = null;
            shutdownTick = null;

            BroadcastMessage("Realm shutdown has been cancelled.");
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (!shutdownSpan.HasValue)
                return;

            shutdownTick.Update(lastTick);
            if (shutdownTick.HasElapsed)
            {
                if (shutdownSpan.Value <= TimeSpan.Zero)
                {
                    shutdown.Invoke();
                    return;
                }

                BroadcastMessage(FormatShutdownTime(shutdownSpan.Value));

                TimeSpan nextTick = NextShutdownTick(shutdownSpan.Value);
                shutdownSpan = shutdownSpan.Value.Subtract(nextTick);
                shutdownTick = new UpdateTimer(nextTick);
            }
        }

        /// <summary>
        /// Display pending shutdown time to <see cref="Player"/> on login.
        /// </summary>
        public void OnLogin(Player player)
        {
            if (!shutdownSpan.HasValue)
                return;

            string message = FormatShutdownTime(shutdownSpan.Value + TimeSpan.FromSeconds(shutdownTick.Time));
            GlobalChatManager.Instance.SendMessage(player.Session, message, type: ChatChannelType.Realm);
        }

        /// <summary>
        /// Absolute time till next shutdown tick.
        /// </summary>
        private TimeSpan NextShutdownTick(TimeSpan span)
        {
            TimeSpan NextShutdownTickAligned(Func<TimeSpan, bool> predicate, TimeSpan interval)
            {
                if (predicate(span))
                    return span.Subtract(span.RoundDown(interval));
                return interval;
            }

            if (span.TotalDays > 1)
                return NextShutdownTickAligned(s => s.TotalDays % 1 != 0, TimeSpan.FromDays(1));
            if (span.TotalHours > 1)
                return NextShutdownTickAligned(s => s.TotalHours % 1 != 0, TimeSpan.FromHours(1));
            if (span.TotalMinutes > 1)
                return NextShutdownTickAligned(s => s.TotalMinutes % 1 != 0, TimeSpan.FromMinutes(1));
            if (span.TotalSeconds > 30)
                return NextShutdownTickAligned(s => s.TotalSeconds % 10 != 0, TimeSpan.FromSeconds(10));
            if (span.TotalSeconds > 10)
                return NextShutdownTickAligned(s => s.TotalSeconds % 5 != 0, TimeSpan.FromSeconds(5));

            return TimeSpan.FromSeconds(1);
        }

        private string FormatShutdownTime(TimeSpan span)
        {
            string PluralFormatter(string single, string plural, int value)
            {
                return value is 0 or > 1 ? plural : single;
            }

            var sb = new StringBuilder();
            sb.Append("Realm shutting down in");

            if (span.Days >= 1)
                sb.Append($" {span.Days} {PluralFormatter("day", "days", span.Days)}");
            if (span.Hours >= 1)
                sb.Append($" {span.Hours} {PluralFormatter("hour", "hours", span.Hours)}");
            if (span.Minutes >= 1)
                sb.Append($" {span.Minutes} {PluralFormatter("minute", "minutes", span.Minutes)}");
            if (span.Seconds >= 1)
                sb.Append($" {span.Seconds} {PluralFormatter("second", "seconds", span.Seconds)}");

            sb.Append('.');
            return sb.ToString();
        }

        private void BroadcastMessage(string message)
        {
            foreach (WorldSession session in NetworkManager<WorldSession>.Instance)
                GlobalChatManager.Instance.SendMessage(session, message, type: ChatChannelType.Realm);
        }
    }
}
