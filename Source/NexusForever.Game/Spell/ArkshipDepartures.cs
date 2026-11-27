using System.Numerics;
using System.Runtime.CompilerServices;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Map;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Spell
{
    public static class ArkshipDepartures
    {
        private const ushort ArkshipWorld = 3460;

        private sealed record Assignment(ushort Quest, Faction Faction, uint Contact, uint Briefing, uint Departure, uint TargetGroup);

        private sealed record Route(uint Terminal, uint Spell, uint Option, Vector3 TerminalPosition, Assignment Assignment,
            ushort World, uint Zone, uint Receiver, Vector3 Landing);

        private sealed record PendingDeparture(Route Route, IQuest Quest);

        private static readonly Assignment Exile = new(10528, Faction.Exile, 73604, 21326, 21327, 14367);
        private static readonly Assignment Dominion = new(10530, Faction.Dominion, 74778, 21347, 21348, 14387);

        // Stock terminal placements or authored objective locations, paired with the
        // existing Nexus starting points in the character creation data.
        private static readonly Route[] Routes =
        [
            new(73606, 34680, 21328, new(-4152.0664f, -449.55457f, -4187.1313f), Exile,
                426, 648u, 53532u, new(4110.71f, -658.6249f, -5145.48f)),
            new(73605, 34681, 21329, new(-4152.045f, -449.55466f, -4193.5903f), Exile,
                990, 1417u, 53533u, new(-771.823f, -904.2852f, -2269.56f)),
            new(74773, 50279, 21349, new(-3570.5374f, -436.93027f, -4316.3125f), Dominion,
                1387, 1411u, 53619u, new(-3835.341f, -980.2174f, -6050.524f)),
            new(74772, 37270, 21350, new(-3567.1978f, -436.9302f, -4312.825f), Dominion,
                870, 1236u, 53620u, new(-8261.3984f, -995.471f, -242.3648f))
        ];

        private static readonly ConditionalWeakTable<IPlayer, PendingDeparture> pending = new();

        private static Route FindRoute(IWorldEntity target) =>
            target.Map?.Entry.Id == ArkshipWorld ? Routes.FirstOrDefault(route => route.Terminal == target.CreatureId) : null;

        public static bool TryBeginDirect(IPlayer player, IWorldEntity target)
        {
            if (FindRoute(target) == null)
                return false;

            QuestCsiInteraction.TryBegin(player, target, static (_, _) => { });
            return true;
        }

        internal static bool CanBegin(IPlayer player, IWorldEntity target) =>
            FindRoute(target) is not { } route || GetEligibleQuest(player, target, route) != null;

        internal static bool IsSupportingEffect(IWorldEntity target, Spell4EffectsEntry effect) =>
            FindRoute(target) is { } route && route.Assignment == Dominion && effect.SpellId == route.Spell &&
            effect is { EffectType: SpellEffectType.RavelSignal, TargetFlags: (uint)SpellEffectTargetFlags.Target,
                        DataBits00: 1u, DataBits01: 10785u, DataBits02: 0u } or
                      { EffectType: SpellEffectType.Fluff, TargetFlags: (uint)SpellEffectTargetFlags.Caster,
                        DataBits00: 0u, DataBits01: 0u, DataBits02: 0u };

        internal static bool Complete(IPlayer player, IWorldEntity target, uint spell)
        {
            if (FindRoute(target) is not { } route)
                return false;

            if (spell != route.Spell || GetEligibleQuest(player, target, route) is not { } quest)
                return true;

            WorldEntry destination = GameTableManager.Instance.World.GetEntry(route.World);
            if (destination == null || DisableManager.Instance.IsDisabled(DisableType.World, destination.Id) ||
                !IsLandingAvailable(destination, route))
                return true;

            // A failed map transfer must leave the terminal available for retry.
            pending.Remove(player);
            pending.Add(player, new(route, quest));
            player.TeleportTo(destination, route.Landing);
            return true;
        }

        internal static bool RecordArrival(IPlayer player)
        {
            if (!pending.TryGetValue(player, out PendingDeparture departure))
                return false;

            pending.Remove(player);
            Route route = departure.Route;
            IQuest quest = departure.Quest;
            if (player.Map?.Entry.Id != route.World || player.Zone?.Id != route.Zone ||
                player.Faction1 != route.Assignment.Faction ||
                player.Faction2 != route.Assignment.Faction ||
                !(Vector3.DistanceSquared(player.Position, route.Landing) <= 1f) ||
                quest.PendingDelete || !player.QuestManager.GetActiveQuests().Any(active => ReferenceEquals(active, quest)) ||
                !BriefingComplete(quest, route.Assignment))
                return false;

            if (quest.State == QuestState.Accepted)
            {
                quest.ObjectiveUpdate(route.Option, 1u);
                quest.ObjectiveUpdate(route.Assignment.Departure, 1u);
            }
            return true;
        }

        internal static void Cancel(IPlayer player) => pending.Remove(player);

        internal static bool CanReport(IPlayer player, IQuest quest, bool communicator)
        {
            if (quest.Id is not (10528 or 10530))
                return true;

            if (communicator)
                return false;

            return Routes.Any(route => route.Assignment.Quest == quest.Id &&
                player.Faction1 == route.Assignment.Faction && player.Faction2 == route.Assignment.Faction &&
                player.Map?.Entry.Id == route.World && player.Zone?.Id == route.Zone &&
                quest.Any(goal => MatchesDepartureGoal(goal, route.Option, route.Assignment.TargetGroup) && goal.IsComplete()) &&
                player.GetVisibleCreature<IWorldEntity>(route.Receiver).Any(receiver =>
                    ReferenceEquals(receiver.Map, player.Map) &&
                    Vector3.DistanceSquared(player.Position, receiver.Position) <= 100f));
        }

        private static bool BriefingComplete(IQuest quest, Assignment assignment) =>
            quest.Any(goal => goal.ObjectiveInfo is { Type: QuestObjectiveType.TalkTo } info &&
                info.Id == assignment.Briefing && info.Entry.Data == assignment.Contact && goal.IsComplete());

        private static bool IsLandingAvailable(WorldEntry world, Route route)
        {
            try
            {
                return MapIOManager.Instance.GetBaseMap(world.AssetPath).GetWorldAreaId(route.Landing) == route.Zone;
            }
            catch (IOException)
            {
                return false;
            }
        }

        // These alternative boarding objectives and hidden sightseeing goals must keep
        // their real values after the one required departure objective is fulfilled.
        internal static bool PreserveOptionalObjective(ushort quest, uint objective) => (quest, objective) switch
        {
            (10528, 21328 or 21329 or 21342) => true,
            (10530, 21349 or 21350 or 21351) => true,
            _ => false
        };

        private static IQuest GetEligibleQuest(IPlayer player, IWorldEntity target, Route route)
        {
            Assignment assignment = route.Assignment;
            if (target is not { EntityId: > 0u } || target.CreatureEntry.Spell4IdActivate00 != route.Spell ||
                !player.InWorld || player.InCombat || !player.CanTeleport() ||
                player.Faction1 != assignment.Faction || player.Faction2 != assignment.Faction ||
                !(Vector3.DistanceSquared(target.Position, route.TerminalPosition) <= 0.25f) ||
                !QuestCsiInteraction.CanInteract(player, target))
                return null;

            IQuest quest = player.QuestManager.GetActiveQuests().FirstOrDefault(active =>
                active.Id == assignment.Quest && !active.PendingDelete &&
                active.State is QuestState.Accepted or QuestState.Achieved);
            if (quest == null || !BriefingComplete(quest, assignment))
                return null;

            IQuestObjective departure = quest.FirstOrDefault(goal =>
                MatchesDepartureGoal(goal, assignment.Departure, assignment.TargetGroup));
            IQuestObjective option = quest.FirstOrDefault(goal =>
                MatchesDepartureGoal(goal, route.Option, assignment.TargetGroup) && goal.ObjectiveInfo.IsOptional());
            if (departure == null || option == null ||
                (quest.State == QuestState.Accepted && departure.IsComplete()))
                return null;

            // The earlier generic handler credited both alternatives without travel.
            // Allow that old saved state to recover at the terminal, but retain
            // the chosen destination for a departure completed by the new handler.
            if (quest.State == QuestState.Achieved &&
                (!departure.IsComplete() || !option.IsComplete() ||
                    !quest.Any(goal => goal.ObjectiveInfo.Id != route.Option && goal.ObjectiveInfo.IsOptional() &&
                        MatchesDepartureGoal(goal, goal.ObjectiveInfo.Id, assignment.TargetGroup) && goal.IsComplete())))
                return null;

            return quest;
        }

        private static bool MatchesDepartureGoal(IQuestObjective goal, uint id, uint group) =>
            goal.ObjectiveInfo is { Type: QuestObjectiveType.ActivateTargetGroup, Entry.Count: 1u } info &&
            info.Id == id && info.Entry.Data == group;
    }
}
