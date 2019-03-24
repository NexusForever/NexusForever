using System.Collections.Generic;
using NexusForever.Shared.Database.Auth.Model;

namespace NexusForever.WorldServer.Game
{
    public static class CleanupManager
    {
        private static readonly HashSet<uint> pendingCleanup = new HashSet<uint>();

        /// <summary>
        /// Start tracking supplied <see cref="Account"/> for pending character cleanup.
        /// </summary>
        public static void Track(Account account)
        {
            pendingCleanup.Add(account.Id);
        }

        /// <summary>
        /// Stops tracking supplied <see cref="Account"/> for pending character cleanup.
        /// </summary>
        public static void Untrack(Account account)
        {
            pendingCleanup.Remove(account.Id);
        }

        /// <summary>
        /// Returns if supplied <see cref="Account"/> has a character pending cleanup.
        /// </summary>
        public static bool HasPendingCleanup(Account account)
        {
            return pendingCleanup.Contains(account.Id);
        }
    }
}
