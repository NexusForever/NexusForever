using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    [ScriptFilterCreatureId(ProjectorCreature)]
    public sealed class MissionsProjectorScript : IWorldEntityScript, IOwnedScript<ISimpleCollidableEntity>
    {
        private const uint ProjectorCreature = 73743u;
        private const uint ProjectorSpell = 87061u;
        private const uint ArrivalMarker = 51610u;
        private const uint HousingArea = 5969u;
        private const float PlacementTolerance = 1f;
        private const float ActivationRange = 5f;

        private readonly IGameTableManager gameTables;
        private ISimpleCollidableEntity projector;

        public MissionsProjectorScript(IGameTableManager gameTables)
        {
            this.gameTables = gameTables;
        }

        public void OnLoad(ISimpleCollidableEntity owner) => projector = owner;

        public void OnActivate(IPlayer activator) => TryTeleport(activator);

        public void OnActivateCast(IPlayer activator) => TryTeleport(activator);

        private void TryTeleport(IPlayer player)
        {
            if (projector is not { EntityId: > 0u, CreatureId: ProjectorCreature,
                    CreatureEntry.Spell4IdActivate00: ProjectorSpell,
                    Map: IMapInstance { Entry.Id: MissionsArrivalScript.TutorialWorld } map } ||
                player is not { IsAlive: true, IsLoading: false, Zone.Id: HousingArea } ||
                !ReferenceEquals(player.Map, map) || !player.CanTeleport() ||
                !ReferenceEquals(player.GetVisible<IWorldEntity>(projector.Guid), projector) ||
                !(Vector3.DistanceSquared(player.Position, projector.Position) <= ActivationRange * ActivationRange))
                return;

            if (MissionsArrivalScript.GetQuest(player, out _) == null)
                return;

            WorldLocation2Entry source = gameTables.WorldLocation2.GetEntry(MissionsArrivalScript.ProjectorMarker);
            WorldLocation2Entry arrival = gameTables.WorldLocation2.GetEntry(ArrivalMarker);
            if (source is not { WorldId: MissionsArrivalScript.TutorialWorld, WorldZoneId: HousingArea } ||
                arrival is not { WorldId: MissionsArrivalScript.TutorialWorld, WorldZoneId: MissionsArrivalScript.MissionsArea } ||
                gameTables.WorldZone.GetEntry(arrival.WorldZoneId)?.ParentZoneId != MissionsArrivalScript.MissionsZone ||
                !(Vector3.DistanceSquared(projector.Position, Position(source)) <= PlacementTolerance * PlacementTolerance))
                return;

            player.TeleportTo(map.Entry, Position(arrival), map.MapLock);
        }

        private static Vector3 Position(WorldLocation2Entry location)
            => new(location.Position0, location.Position1, location.Position2);
    }
}
