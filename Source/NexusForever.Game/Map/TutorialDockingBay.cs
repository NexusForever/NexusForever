using System.Numerics;
using NexusForever.GameTable;

namespace NexusForever.Game.Map
{
    /// <summary>
    /// Resolve the two Arkship interiors when their prop geometry has no terrain zone.
    /// </summary>
    internal static class TutorialDockingBay
    {
        internal const uint WorldId = 3460u;
        private const float FloorTolerance = 4f;

        private readonly record struct Bay(uint Zone, uint ZoneMap, uint LowestFloor, uint HighestFloor);

        // The original NPEUExile and NPEUDominion maps define the horizontal bounds.
        // Use walking-floor markers from each bay to keep exterior, overhead and lower decks out.
        private static readonly Bay[] Bays =
        [
            new(5998u, 545u, 51697u, 51723u),
            new(5999u, 546u, 52749u, 52762u)
        ];

        internal static uint? GetZoneId(Vector3 position, IGameTableManager tables)
        {
            var hex = ZoneMapManager.Points2ZoneMapCoordinate(position);
            uint? identifiedZone = null;

            foreach (Bay bay in Bays)
            {
                if (tables.MapZone.GetEntry(bay.ZoneMap) is not { WorldZoneId: var zone } bounds || zone != bay.Zone ||
                    hex.X < bounds.HexMinX || hex.X > bounds.HexLimX ||
                    hex.Y < bounds.HexMinY || hex.Y > bounds.HexLimY ||
                    tables.WorldLocation2.GetEntry(bay.LowestFloor) is not { WorldId: WorldId } low ||
                    tables.WorldLocation2.GetEntry(bay.HighestFloor) is not { WorldId: WorldId } high ||
                    low.WorldZoneId != zone || high.WorldZoneId != zone ||
                    !(position.Y >= low.Position1 - FloorTolerance && position.Y <= high.Position1 + FloorTolerance))
                    continue;

                if (identifiedZone.HasValue)
                    return null;

                identifiedZone = zone;
            }

            return identifiedZone;
        }
    }
}
