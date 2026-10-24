using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.InSubZone)]
    public class PrerequisiteCheckInSubZone : IPrerequisiteCheck
    {
        #region Dependency Injection

        private readonly ILogger<PrerequisiteCheckInSubZone> log;

        public PrerequisiteCheckInSubZone(
            ILogger<PrerequisiteCheckInSubZone> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// The row's value is a <see cref="WorldZoneEntry"/> id and objectId is read by nothing. The check
        /// is met when the zone the player is in is that zone, or any descendant of it: the client walks the
        /// WorldZone parent chain upwards from the player's current zone and stops at a match.
        /// </summary>
        /// <remarks>
        /// The client's handler calls the walk once and then negates uniformly: Equal returns the walk's
        /// result, NotEqual returns its negation, and every other comparison returns false. The walk's
        /// several ways of answering "no" - the current zone has no record, a parent record is missing, the
        /// chain ended without a match - are indistinguishable to the handler, so NotEqual passes for all of
        /// them. A player whose zone has no WorldZone row therefore MEETS a NotEqual row.
        ///
        /// Order inside the walk is taken from the client and matters: the current zone id is compared with
        /// value before any table lookup, so a value whose own WorldZone row is missing can still match the
        /// current zone.
        ///
        /// Two deliberate divergences from the client, both noted in the pull request:
        ///  - the client reads the local player's zone out of a global and ignores the unit it is handed; here
        ///    the zone of the player the check is evaluated for is used, which is the server-side equivalent.
        ///  - the walk is bounded. The client has no cycle guard; a server must not hang on bad table data.
        /// </remarks>
        private const int MaxParentChainDepth = 64;

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (comparison != PrerequisiteComparison.Equal && comparison != PrerequisiteComparison.NotEqual)
            {
                log.LogWarning($"Unhandled PrerequisiteComparison {comparison} for {PrerequisiteType.InSubZone}!");
                return false;
            }

            bool inZone = IsInZone(player, value);

            return comparison == PrerequisiteComparison.Equal ? inZone : !inZone;
        }

        /// <summary>
        /// Whether the player's current zone is <paramref name="value"/> or any descendant of it, by walking
        /// the WorldZone parent chain upwards.
        /// </summary>
        /// <remarks>
        /// Every way of answering false - no current zone record, a missing parent record, a chain that ends
        /// without a match, or the bounded loop running out - is the same answer to the caller, exactly as it
        /// is to the client's handler.
        /// </remarks>
        private static bool IsInZone(IPlayer player, uint value)
        {
            WorldZoneEntry zone = player.Zone;
            if (zone == null)
                return false;

            // the client compares the current zone id with value before it looks anything up
            if (zone.Id == value)
                return true;

            for (int i = 0; i < MaxParentChainDepth; i++)
            {
                zone = GameTableManager.Instance.WorldZone.GetEntry(zone.ParentZoneId);
                if (zone == null)
                    return false;

                if (zone.Id == value)
                    return true;
            }

            return false;
        }
    }
}
