using NexusForever.GameTable;

namespace NexusForever.Game.Map
{
    public static class WorldZoneHierarchy
    {
        /// <summary>
        /// Returns if <paramref name="zoneId"/> is <paramref name="targetZoneId"/> or lies inside it. A player's zone is the
        /// most specific one, so this walks up its parents (at most 8).
        /// </summary>
        public static bool Contains(uint zoneId, uint targetZoneId)
        {
            for (int depth = 0; zoneId != 0u && depth < 8; depth++)
            {
                if (zoneId == targetZoneId)
                    return true;

                zoneId = GameTableManager.Instance.WorldZone.GetEntry(zoneId)?.ParentZoneId ?? 0u;
            }

            return false;
        }
    }
}
