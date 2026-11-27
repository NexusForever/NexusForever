using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;

namespace NexusForever.Game.Quest
{
    internal static class ShadeEvePlants
    {
        private const int FirstPlantSlot = 240;
        private const int PlantCount = 8;
        private const float GatherDistance = 3f;
        private const uint GatherSpell = 83750;

        private sealed record Route(ushort Quest, uint Objective, uint World, Faction Faction,
            uint Contact, uint ContactMarker, uint Plant, uint RewardGroup, uint Credit, uint[] PlantMarkers);

        private static readonly Route thayd = new(9686, 18570, 51, Faction.Exile,
            65410, 47314, 65430, 11004, 221454, [47585, 47586, 47587]);

        private static readonly Route illium = new(9687, 18567, 22, Faction.Dominion,
            65409, 47588, 65425, 11005, 221455, [47185, 47186, 47187, 47188]);

        private static Route ForWorld(uint? world) => world switch { 51 => thayd, 22 => illium, _ => null };

        internal static ushort? FindQuest(uint? world) => ForWorld(world)?.Quest;

        internal static bool HandlesQuest(uint quest) => quest is 9686 or 9687;

        internal static bool HandlesObjective(ushort quest, uint objective) =>
            (quest, objective) is (9686, 18570) or (9687, 18567);

        private static Route ForActor(IGridEntity actor)
        {
            if (actor is not IWorldEntity { EntityId: > 0, CreatureId: var creature })
                return null;
            Route route = ForWorld(actor.Map?.Entry.Id);
            return route != null && (creature == route.Contact || creature == route.Plant) ? route : null;
        }

        internal static bool Tracks(IGridEntity entity) => ForActor(entity) != null;

        internal static uint BindPlant(EntityModel actor)
        {
            Route route = ForWorld(actor.World);
            int slot = actor.QuestChecklistIdx - FirstPlantSlot;
            if (route == null || actor.Id == 0 || actor.Creature != route.Plant || slot < 0 || slot >= PlantCount ||
                !route.PlantMarkers.Any(marker => NearMarker(route, marker, new Vector3(actor.X, actor.Y, actor.Z))))
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

            return actor is NonPlayerEntity { ShadePlantSlot: > 0 } plant &&
                Goal(ActiveQuest(player, route), route) is { } goal && !goal.HasChecklist(plant.ShadePlantSlot);
        }

        internal static bool CanVisitContact(IPlayer player, ushort quest)
        {
            Route route = ForWorld(player.Map?.Entry.Id);
            return route?.Quest == quest && ValidPlayer(player, route) &&
                player.GetVisibleCreature<INonPlayerEntity>(route.Contact).Any(actor =>
                    actor.EntityId > 0 && actor.IsAlive && ReferenceEquals(player.Map, actor.Map) &&
                    Vector3.DistanceSquared(player.Position, actor.Position) <= 36f &&
                    NearMarker(route, route.ContactMarker, actor.Position, 12f));
        }

        internal static void Gather(IPlayer player, IReadOnlyDictionary<ushort, IQuest> active)
        {
            Route route = ForWorld(player.Map?.Entry.Id);
            if (route == null || !ValidPlayer(player, route) ||
                !active.TryGetValue(route.Quest, out IQuest quest) || Goal(quest, route) is not { } goal)
                return;
            if (GameTableManager.Instance.Spell4Effects.GetEntry(route.Credit) is not
                { SpellId: GatherSpell, EffectType: SpellEffectType.QuestAdvanceObjective,
                  TargetFlags: (uint)SpellEffectTargetFlags.Target, DataBits01: 0u, DataBits02: 1u } effect ||
                effect.DataBits00 != route.Quest)
                return;

            var plant = player.GetVisibleCreature<INonPlayerEntity>(route.Plant)
                .OfType<NonPlayerEntity>()
                .FirstOrDefault(actor => actor.IsAlive && actor.ShadePlantSlot != 0 &&
                    ReferenceEquals(player.Map, actor.Map) &&
                    Vector3.DistanceSquared(player.Position, actor.Position) <= GatherDistance * GatherDistance &&
                    !goal.HasChecklist(actor.ShadePlantSlot));
            if (plant == null || !goal.TryMarkChecklist(plant.ShadePlantSlot))
                return;

            quest.ObjectiveUpdate(route.Objective, 1u);
            if (player is Player viewer)
                viewer.RemoveVisible(plant);
        }

        private static IQuest ActiveQuest(IPlayer player, Route route) =>
            player.QuestManager.GetActiveQuests().FirstOrDefault(quest => quest.Id == route.Quest);

        private static QuestObjective Goal(IQuest quest, Route route) =>
            quest is { State: QuestState.Accepted, PendingDelete: false } &&
            quest.FirstOrDefault(objective => objective.ObjectiveInfo.Id == route.Objective) is QuestObjective goal &&
            goal.ObjectiveInfo.Type == QuestObjectiveType.ActivateEntity &&
            goal.ObjectiveInfo.Entry is { Data: 0, Count: PlantCount } entry &&
            entry.TargetGroupIdRewardPane == route.RewardGroup ? goal : null;

        private static bool ValidPlayer(IPlayer player, Route route) =>
            player is { IsLoading: false, InWorld: true, IsAlive: true } && player.Faction1 == route.Faction;

        private static bool NearMarker(Route route, uint markerId, Vector3 point, float minimumRadius = 0f)
        {
            if (GameTableManager.Instance.WorldLocation2.GetEntry(markerId) is not { } marker || marker.WorldId != route.World)
                return false;
            float radius = Math.Max(marker.Radius, minimumRadius);
            return Vector2.DistanceSquared(new(point.X, point.Z), new(marker.Position0, marker.Position2)) <= radius * radius;
        }
    }
}

