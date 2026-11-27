using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;

namespace NexusForever.Game.Quest
{
    public static class LegacyDestinyBoarding
    {
        private const ushort Destiny = 1634;
        private const ushort Crimson = 870;
        private const ushort Levian = 1387;
        private const uint Pheydra = 51115u;
        private const uint FlightGroup = 4031u;

        private sealed record Site(uint Creature, ushort World, Vector3 Position);
        private sealed record Route(ushort Quest, uint Race, uint Talk, uint Flight, uint Optional, ushort Default,
            Site Guide, Site CrimsonReceiver, Site LevianReceiver);
        private sealed record Terminal(Site Site, ushort World, Vector3 Arrival, uint Spell, uint Choice);

        private static readonly Site Officer = new(Pheydra, Destiny, new(-827.5711f, 81.78798f, 767.9716f));
        private static readonly Terminal CrimsonDesk = new(new(33803, Destiny, new(-699.17596f, 81.33406f, 687.691f)),
            Crimson, new(-8261.3984f, -995.471f, -242.3648f), 37270u, 1u);
        private static readonly Terminal LevianDesk = new(new(51014, Destiny, new(-699.54095f, 81.33406f, 701.54926f)),
            Levian, new(-3835.341f, -980.2174f, -6050.524f), 50279u, 2u);

        private static readonly Route[] Routes =
        [
            new(7058, 12u, 16130u, 12937u, 17996u, Levian,
                new(51124u, Destiny, new(-704.61005f, 81.68f, 590.32007f)),
                new(53656u, Crimson, new(-8259.936f, -995.34f, -241.324f)),
                new(53655u, Levian, new(-3833.920f, -980.08f, -6051.404f))),
            new(7059, 13u, 16129u, 12938u, 17997u, Crimson,
                new(45496u, Destiny, new(-702.72f, 81.67001f, 840.98975f)),
                new(53715u, Crimson, new(-8259.936f, -995.34f, -241.324f)),
                new(53716u, Levian, new(-3833.920f, -980.08f, -6051.404f))),
            new(7060, 5u, 16128u, 12939u, 17998u, Crimson,
                new(51116u, Destiny, new(-702.73f, 81.66998f, 945.60986f)),
                new(53725u, Crimson, new(-8259.936f, -995.34f, -241.324f)),
                new(53726u, Levian, new(-3833.920f, -980.08f, -6051.404f)))
        ];

        internal static IEnumerable<(ushort Quest, uint Giver, uint[] Receivers)> Relations() =>
            Routes.Select(route => (route.Quest, Pheydra, new[] { route.CrimsonReceiver.Creature, route.LevianReceiver.Creature }));

        internal static bool OwnsOptional(ushort quest, uint objective) =>
            Routes.Any(route => route.Quest == quest && route.Optional == objective);

        internal static bool OwnsFlight(ushort quest, uint objective) =>
            Routes.Any(route => route.Quest == quest && (route.Flight == objective || route.Optional == objective));

        private static Route For(IPlayer player) => Routes.FirstOrDefault(route => route.Race == (uint)player.Race);

        private static bool Is(IWorldEntity entity, Site site) => entity is { EntityId: > 0u } &&
            entity.CreatureId == site.Creature && entity.Map?.Entry.Id == site.World &&
            Vector3.DistanceSquared(entity.Position, site.Position) <= 9f;

        private static bool Close(IPlayer player, IWorldEntity entity) =>
            ReferenceEquals(player.Map, entity.Map) && Vector3.DistanceSquared(player.Position, entity.Position) <= 36f;

        private static bool Nearby(IPlayer player, Site site) => player.Map?.Entry.Id == site.World &&
            player.GetVisibleCreature<IWorldEntity>(site.Creature).Any(entity => Is(entity, site) && Close(player, entity));

        private static bool Dominion(IPlayer player) => player is
            { Faction1: Faction.Dominion, Faction2: Faction.Dominion, InWorld: true, IsAlive: true, IsLoading: false };

        private static IQuest WithState(IPlayer player, Route route, QuestState state) =>
            player.QuestManager.GetActiveQuests().FirstOrDefault(quest => quest.Id == route.Quest && !quest.PendingDelete && quest.State == state);

        private static QuestObjective Flight(IQuest quest, Route route) => quest?.FirstOrDefault(goal =>
            goal.ObjectiveInfo is { Type: QuestObjectiveType.ActivateTargetGroup, Entry: { Data: FlightGroup, Count: 1u } } &&
            goal.ObjectiveInfo.Id == route.Flight) as QuestObjective;

        internal static bool AllowsAccept(IPlayer player, ushort quest, bool byItem)
        {
            Route own = Routes.FirstOrDefault(route => route.Quest == quest);
            return own == null || (!byItem && Dominion(player) && own == For(player) && Nearby(player, Officer));
        }

        internal static bool AllowsReport(IPlayer player, ushort quest, bool remote)
        {
            Route own = Routes.FirstOrDefault(route => route.Quest == quest);
            if (own == null)
                return true;
            if (remote || !Dominion(player) || own != For(player) ||
                Flight(WithState(player, own, QuestState.Achieved), own) is not { } flight || !flight.IsComplete())
                return false;

            return (flight.HasChecklist(CrimsonDesk.Choice) && Nearby(player, own.CrimsonReceiver)) ||
                (flight.HasChecklist(LevianDesk.Choice) && Nearby(player, own.LevianReceiver));
        }

        public static bool AllowsTalk(IPlayer player, IWorldEntity entity, bool talk) =>
            !Routes.Any(route => Is(entity, route.Guide)) ||
                (talk && Dominion(player) && For(player) is { } route && Is(entity, route.Guide) && Close(player, entity));

        public static bool TryUse(IPlayer player, IWorldEntity entity)
        {
            Terminal terminal = Is(entity, CrimsonDesk.Site) ? CrimsonDesk : Is(entity, LevianDesk.Site) ? LevianDesk : null;
            if (terminal == null)
                return false;

            if (!Dominion(player) || !Close(player, entity) || player.InCombat || !player.CanTeleport() ||
                GameTableManager.Instance.Creature2.GetEntry(terminal.Site.Creature)?.Spell4IdActivate00 != terminal.Spell ||
                GameTableManager.Instance.World.GetEntry(terminal.World) is not { } destination ||
                DisableManager.Instance.IsDisabled(DisableType.World, terminal.World) || For(player) is not { } route ||
                WithState(player, route, QuestState.Accepted) is not { } quest ||
                !quest.Any(goal => goal.ObjectiveInfo.Id == route.Talk && goal.IsComplete()) ||
                Flight(quest, route) is not { } flight || flight.IsComplete() ||
                flight.HasChecklist(terminal.World == Crimson ? LevianDesk.Choice : CrimsonDesk.Choice))
                return true;

            flight.TryMarkChecklist(terminal.Choice);
            player.TeleportTo(destination, terminal.Arrival);
            return true;
        }

        internal static void RecordArrival(IPlayer player)
        {
            if (!Dominion(player) || For(player) is not { } route ||
                WithState(player, route, QuestState.Accepted) is not { } quest || Flight(quest, route) is not { } flight)
                return;

            Terminal terminal = player.Map?.Entry.Id switch { Crimson => CrimsonDesk, Levian => LevianDesk, _ => null };
            if (terminal == null || !flight.HasChecklist(terminal.Choice) ||
                Vector3.DistanceSquared(player.Position, terminal.Arrival) > 36f)
                return;

            if (terminal.World != route.Default)
                quest.ObjectiveUpdate(route.Optional, 1u);
            quest.ObjectiveUpdate(route.Flight, 1u);
        }
    }
}
