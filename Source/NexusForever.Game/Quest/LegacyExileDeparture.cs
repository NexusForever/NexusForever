using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;

namespace NexusForever.Game.Quest
{
    public static class LegacyExileDeparture
    {
        private const ushort Refuge = 3460;
        private const ushort Gambler = 1537;
        private const ushort NexusHo = 6728;
        private const uint Avra = 46505;
        private const uint Professor = 62232;
        private const uint Console = 62268;
        private const uint Circuit = 30965;
        private const uint CircuitObjective = 10475;
        private const uint ArrivalObjective = 10489;
        private static readonly Vector3 dock = new(-4149.8f, -449.5546f, -4190.1f);
        private static readonly Vector3 station = new(-992.34436f, 83.4518f, 748.423f);
        private static readonly Vector3 circuit = new(-926.0971f, -26.806496f, -1258.973f);

        private sealed record Briefing(ushort Quest, uint Objective, Vector3 ProfessorPosition, Vector3 ConsolePosition);
        private static readonly Briefing northern = new(9552, 18237u,
            new(-1039.5676f, 85.12731f, 638.30994f), new(-1041.1628f, 85.59846f, 644.462f));
        private static readonly Briefing southern = new(9555, 18239u,
            new(-1039.4116f, 85.11108f, 897.96045f), new(-1041.9016f, 85.59847f, 891.5207f));

        public static bool AllowsDockPrerequisite(IPlayer player, IQuestInfo info) =>
            info.Entry is { Id: NexusHo, PrerequisiteId: 4106u } && CanBoard(player) &&
            (IsNextTo(player, Avra, Refuge, dock) || IsNextTo(player, Avra, Gambler, station));

        public static bool AllowsContact(IPlayer player, ushort id, bool report, bool byItem = false)
        {
            if (id is not (NexusHo or 9552 or 9555))
                return true;

            if (byItem || !IsExile(player) || GetBriefing(player) is not { } briefing)
                return false;

            return id == NexusHo
                ? IsNextTo(player, Avra, Gambler, station) || !report && IsNextTo(player, Avra, Refuge, dock)
                : briefing.Quest == id && IsNextTo(player, Avra, Gambler, station);
        }

        public static bool AllowsConversation(IPlayer player, IWorldEntity target) =>
            target is not { CreatureId: Professor, Map.Entry.Id: Gambler } ||
            IsExile(player) && GetBriefing(player) is { } briefing && Active(player, briefing.Quest) != null &&
            Matches(target, Professor, Gambler, briefing.ProfessorPosition) && IsNear(player, target);

        /// <summary>Handles historical travel and instant quest-object activations.</summary>
        public static bool TryActivate(IPlayer player, IWorldEntity target)
        {
            if (Matches(target, Avra, Refuge, dock))
            {
                if (CanBoard(player) && IsNear(player, target) && CanTravel(player))
                {
                    if (player.QuestManager.GetQuestState(NexusHo) is null or QuestState.Unknown or QuestState.Mentioned or QuestState.Abandoned)
                        player.QuestManager.QuestAdd(NexusHo, null);
                    if (player.QuestManager.GetQuestState(NexusHo) is QuestState.Accepted or QuestState.Achieved or QuestState.Completed)
                    {
                        Vector3 destination = Active(player, NexusHo) is { } quest && !HasCredit(quest, CircuitObjective)
                            ? circuit : station;
                        Travel(player, Gambler, destination);
                    }
                }

                return true;
            }

            if (Matches(target, Circuit, Gambler, circuit))
            {
                if (IsExile(player) && IsNear(player, target) && CanTravel(player) &&
                    target.CreatureEntry is { Spell4IdActivate00: 1817u } && Active(player, NexusHo) is { } quest)
                {
                    quest.ObjectiveUpdate(CircuitObjective, 1u);
                    if (HasCredit(quest, CircuitObjective))
                        Travel(player, Gambler, station);
                }

                return true;
            }

            if (Matches(target, Console, Gambler, northern.ConsolePosition) ||
                Matches(target, Console, Gambler, southern.ConsolePosition))
            {
                if (IsExile(player) && GetBriefing(player) is { } briefing &&
                    Matches(target, Console, Gambler, briefing.ConsolePosition) && IsNear(player, target) &&
                    target.CreatureEntry is { Spell4IdActivate00: 72033u })
                    Active(player, briefing.Quest)?.ObjectiveUpdate(briefing.Objective, 1u);
                return true;
            }

            if (Matches(target, Avra, Gambler, station) && IsExile(player) &&
                GetBriefing(player) is { } signal && player.QuestManager.GetQuestState(signal.Quest) == QuestState.Completed)
            {
                if (IsNear(player, target) && CanTravel(player))
                    Travel(player, Refuge, dock);
                return true;
            }

            return false;
        }

        internal static void RecordArrival(IPlayer player)
        {
            if (player.Map?.Entry.Id == Gambler && IsExile(player) && Near(player.Position, station, 5f) &&
                Active(player, NexusHo) is { } quest && HasCredit(quest, CircuitObjective) && !HasCredit(quest, ArrivalObjective))
                quest.ObjectiveUpdate(ArrivalObjective, 1u);
        }

        private static bool CanBoard(IPlayer player) => IsExile(player) && GetBriefing(player) != null &&
            (player.QuestManager.GetQuestState(10528) == QuestState.Completed ||
             player.QuestManager.GetActiveQuests().Any(quest => quest.Id == 10528 &&
                 quest.State is QuestState.Accepted or QuestState.Achieved && HasCredit(quest, 21326u)));

        private static Briefing GetBriefing(IPlayer player) => (uint)player.Race switch
        {
            4 or 16 => northern,
            1 or 3 => southern,
            _ => null
        };

        private static bool IsExile(IPlayer player) =>
            player is { Faction1: Faction.Exile, Faction2: Faction.Exile, InWorld: true, IsAlive: true, IsLoading: false };

        private static IQuest Active(IPlayer player, ushort id) =>
            player.QuestManager.GetActiveQuests().FirstOrDefault(quest => quest.Id == id &&
                quest is { State: QuestState.Accepted, PendingDelete: false });

        private static bool HasCredit(IQuest quest, uint objective) =>
            quest.Any(goal => goal.ObjectiveInfo.Id == objective && goal.IsComplete());

        private static bool Matches(IWorldEntity actor, uint creature, ushort world, Vector3 expected) =>
            actor is { EntityId: > 0u } && actor.CreatureId == creature && actor.Map?.Entry.Id == world &&
            Near(actor.Position, expected, 1.5f);

        private static bool IsNextTo(IPlayer player, uint creature, ushort world, Vector3 expected) =>
            player.Map?.Entry.Id == world && player.GetVisibleCreature<IWorldEntity>(creature)
                .Any(actor => Matches(actor, creature, world, expected) && IsNear(player, actor));

        private static bool IsNear(IPlayer player, IWorldEntity target) =>
            target.Guid != 0u && ReferenceEquals(player.Map, target.Map) && Near(player.Position, target.Position, 5f);

        private static bool Near(Vector3 first, Vector3 second, float range) =>
            Vector3.DistanceSquared(first, second) <= range * range;

        private static bool CanTravel(IPlayer player) =>
            !player.InCombat && !player.PlatformGuid.HasValue && player.CanTeleport();

        private static void Travel(IPlayer player, ushort world, Vector3 destination)
        {
            if (GameTableManager.Instance.World.GetEntry(world) is { } entry &&
                !DisableManager.Instance.IsDisabled(DisableType.World, world))
                player.TeleportTo(entry, destination + new Vector3(1.15f, .1f, 0f));
        }
    }
}
