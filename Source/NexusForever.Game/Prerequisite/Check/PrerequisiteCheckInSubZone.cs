using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Prerequisite;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Prerequisite.Check
{
    [PrerequisiteCheck(PrerequisiteType.InSubZone)]
    public sealed class PrerequisiteCheckInSubZone : IPrerequisiteCheck
    {
        private readonly IGameTableManager gameTableManager;

        public PrerequisiteCheckInSubZone(IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        public bool Meets(IPlayer player, PrerequisiteComparison comparison, uint value, uint objectId, IPrerequisiteParameters parameters)
        {
            if (player.Map == null || value == 0u || gameTableManager.WorldZone.GetEntry(value) == null ||
                comparison is not (PrerequisiteComparison.Equal or PrerequisiteComparison.NotEqual))
                return false;

            var visitedZones = new HashSet<uint>();
            for (WorldZoneEntry zone = player.Zone; zone is { Id: > 0u } && visitedZones.Add(zone.Id);
                 zone = gameTableManager.WorldZone.GetEntry(zone.ParentZoneId))
            {
                if (zone.Id == value)
                    return comparison == PrerequisiteComparison.Equal;
                if (zone.ParentZoneId == 0u)
                    return comparison == PrerequisiteComparison.NotEqual;
            }

            return false;
        }
    }
}
