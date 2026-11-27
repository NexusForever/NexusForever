using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Map
{
    public static class TutorialConstructProjector
    {
        private const uint Projector = 73745u;
        private const uint ProjectorMarker = 51742u;
        private const uint ConstructZone = 5968u;
        private const uint TransferSpell = 87061u;
        private const float RangeSquared = 25f;

        private readonly record struct DockingRoute(ushort Quest, uint Zone, uint Goal, uint Destination);

        /// <summary>
        /// Handle the simulation exit on the Construct platform. Arrival triggers the native zone objective.
        /// </summary>
        public static bool TryActivate(IPlayer player, IWorldEntity projector, IGameTableManager tables)
        {
            if (projector.CreatureId != Projector || projector.Map is not { Entry.Id: TutorialDockingBay.WorldId } map ||
                tables.Creature2.GetEntry(Projector)?.Spell4IdActivate00 != TransferSpell ||
                tables.WorldLocation2.GetEntry(ProjectorMarker) is not
                    { WorldId: TutorialDockingBay.WorldId, WorldZoneId: ConstructZone } source ||
                !(Vector3.DistanceSquared(projector.Position, Position(source)) <= 1f))
                return false;

            if (!ReferenceEquals(player.Map, map) ||
                player is not { IsAlive: true, IsLoading: false, InCombat: false, Zone.Id: ConstructZone } ||
                !(Vector3.DistanceSquared(player.Position, projector.Position) <= RangeSquared) ||
                !player.CanTeleport() || player.Faction1 != player.Faction2)
                return true;

            DockingRoute? selected = player.Faction1 switch
            {
                Faction.Exile => new(10520, 5998u, 21298u, 51675u),
                Faction.Dominion => new(10523, 5999u, 21311u, 52851u),
                _ => null
            };
            if (selected is not { } route ||
                player.QuestManager.GetActiveQuests().FirstOrDefault(quest =>
                    quest.Id == route.Quest && quest.State == QuestState.Accepted && !quest.PendingDelete) is not { } active ||
                !CanEnterBay(active, route) ||
                tables.WorldLocation2.GetEntry(route.Destination) is not { WorldId: TutorialDockingBay.WorldId } destination ||
                destination.WorldZoneId != route.Zone)
                return true;

            player.TeleportTo(map.Entry, Position(destination), (map as IMapInstance)?.MapLock);
            return true;
        }

        private static bool CanEnterBay(IQuest quest, DockingRoute route)
        {
            const uint index = 4u;
            IQuestObjective objective = quest.FirstOrDefault(goal => goal.Index == index);
            return objective?.ObjectiveInfo is { Type: QuestObjectiveType.EnterZone } info &&
                info.Id == route.Goal && info.Entry.Data == route.Zone &&
                !objective.IsComplete() && quest.Count(goal => goal.Index < index && (goal.IsComplete() || goal.ObjectiveInfo.IsOptional())) == index;
        }

        private static Vector3 Position(WorldLocation2Entry entry) => new(entry.Position0, entry.Position1, entry.Position2);
    }
}
