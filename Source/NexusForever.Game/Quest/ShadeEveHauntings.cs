using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;

namespace NexusForever.Game.Quest
{
    internal static class ShadeEveHauntings
    {
        private const int FirstHauntingSlot = 224;
        private const int HauntingCount = 6;

        private sealed record Route(ushort Quest, uint Objective, uint World, Faction Faction,
            uint Contact, uint ContactMarker, uint Haunting, uint RewardGroup, uint Spell, uint Csi, uint Credit, uint[] Markers);

        private static readonly Route thayd = new(9578, 18293, 51, Faction.Exile,
            62909, 46472, 65286, 12332, 74659, 1172, 196542, [47179, 47180, 47181]);
        private static readonly Route illium = new(9565, 18279, 22, Faction.Dominion,
            62669, 47093, 62680, 10942, 74603, 1169, 196573, [46359, 46421, 46422]);

        private static Route ForWorld(uint? world) => world switch { 51 => thayd, 22 => illium, _ => null };

        internal static ushort? FindQuest(uint? world) => ForWorld(world)?.Quest;

        internal static bool HandlesQuest(uint quest) => quest is 9565 or 9578;

        internal static bool HandlesObjective(ushort quest, uint objective) =>
            (quest, objective) is (9565, 18279) or (9578, 18293);

        private static Route ForActor(IGridEntity actor)
        {
            if (actor is not IWorldEntity { EntityId: > 0, CreatureId: var creature })
                return null;

            Route route = ForWorld(actor.Map?.Entry.Id);
            return route != null && (creature == route.Contact || creature == route.Haunting) ? route : null;
        }

        internal static bool Tracks(IGridEntity actor) => ForActor(actor) != null;

        internal static bool RecognizesTarget(IWorldEntity actor) =>
            ForActor(actor) is { } route && actor.CreatureId == route.Haunting;

        internal static uint BindActor(EntityModel actor)
        {
            Route route = ForWorld(actor.World);
            int slot = actor.QuestChecklistIdx - FirstHauntingSlot;
            if (route == null || actor.Id == 0 || actor.Creature != route.Haunting || slot < 0 || slot >= HauntingCount ||
                !route.Markers.Any(marker => NearMarker(route, marker, new Vector3(actor.X, actor.Y, actor.Z))))
                return 0;

            return 1u << slot;
        }

        internal static bool CanSee(IPlayer player, IGridEntity actor)
        {
            Route route = ForActor(actor);
            if (route == null)
                return true;
            if (!ReferenceEquals(player.Map, actor.Map) || player.Faction1 != route.Faction ||
                player.QuestManager is not QuestManager journal)
                return false;
            if (((IWorldEntity)actor).CreatureId == route.Contact)
                return journal.CanSeeShadePlantQuest(route.Quest);

            return actor is NonPlayerEntity { ShadePlantSlot: > 0 } haunted &&
                FindGoal(player, route).Goal is { } goal && !goal.HasChecklist(haunted.ShadePlantSlot);
        }

        internal static bool CanVisitContact(IPlayer player, ushort quest)
        {
            Route route = ForWorld(player.Map?.Entry.Id);
            return route?.Quest == quest && CanParticipate(player, route) &&
                player.GetVisibleCreature<INonPlayerEntity>(route.Contact).Any(actor =>
                    actor.EntityId > 0 && actor.IsAlive && ReferenceEquals(player.Map, actor.Map) &&
                    Vector3.DistanceSquared(actor.Position, player.Position) <= 36f &&
                    NearMarker(route, route.ContactMarker, actor.Position, 12f));
        }

        internal static (uint Spell, uint Csi, uint Credit) Prepare(IPlayer player, IWorldEntity actor)
        {
            if (ForActor(actor) is not { } route || actor.CreatureId != route.Haunting ||
                actor is not NonPlayerEntity { IsAlive: true, ShadePlantSlot: > 0 } haunting ||
                !ReferenceEquals(actor.Map, player.Map) || !CanParticipate(player, route) ||
                FindGoal(player, route).Goal is not { } goal || goal.HasChecklist(haunting.ShadePlantSlot))
                return default;

            return (route.Spell, route.Csi, route.Credit);
        }

        internal static void Complete(IPlayer player, IWorldEntity actor)
        {
            if (Prepare(player, actor).Spell == 0)
                return;

            Route route = ForActor(actor);
            (IQuest quest, QuestObjective goal) = FindGoal(player, route);
            if (!goal.TryMarkChecklist(((NonPlayerEntity)actor).ShadePlantSlot))
                return;

            quest.ObjectiveUpdate(route.Objective, 1u);
            if (player is Player viewer)
                viewer.RemoveVisible(actor);
        }

        private static (IQuest Quest, QuestObjective Goal) FindGoal(IPlayer player, Route route)
        {
            var quest = player.QuestManager.GetActiveQuests().FirstOrDefault(q => q.Id == route.Quest);
            if (quest is not { State: QuestState.Accepted, PendingDelete: false } ||
                quest.FirstOrDefault(objective => objective.ObjectiveInfo.Id == route.Objective) is not QuestObjective goal ||
                goal.ObjectiveInfo.Type != QuestObjectiveType.ActivateEntity ||
                goal.ObjectiveInfo.Entry.Data != route.Haunting || goal.ObjectiveInfo.Entry.Count != HauntingCount ||
                goal.ObjectiveInfo.Entry.TargetGroupIdRewardPane != route.RewardGroup)
                return default;

            return goal.IsComplete() ? default : (quest, goal);
        }

        private static bool CanParticipate(IPlayer player, Route route) =>
            player is { InWorld: true, IsAlive: true, IsLoading: false } && player.Faction1 == route.Faction;

        private static bool NearMarker(Route route, uint markerId, Vector3 point, float minimumRadius = 0f)
        {
            if (GameTableManager.Instance.WorldLocation2.GetEntry(markerId) is not { } marker || marker.WorldId != route.World)
                return false;
            float radius = Math.Max(marker.Radius, minimumRadius);
            return Vector2.DistanceSquared(new(point.X, point.Z), new(marker.Position0, marker.Position2)) <= radius * radius;
        }
    }
}
