using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;

namespace NexusForever.Game.Quest
{
    public static class LegacyDestinyBriefing
    {
        private const ushort Embarkation = 7019;
        private const ushort Destiny = 1634;
        private const uint Pheydra = 51115u;
        private const uint Chancellor = 62234u;
        private const uint Console = 62303u;

        private static readonly Spot Terminal = new(Pheydra, Destiny, new(-827.5711f, 81.78798f, 767.9716f));

        private static readonly Spot[] Bridges =
        [
            new(Pheydra, 3460, new(-3572.190f, -436.93027f, -4314.400f)),
            new(Pheydra, Destiny, new(-916.800f, -32.340f, -1136.200f)),
            new(Pheydra, 870, new(-8262.9f, -995.38f, -241.20f)),
            new(Pheydra, 1387, new(-3837.0f, -980.14f, -6049.2f))
        ];

        private static readonly Briefing Mechari = new(9556, 18245u, 18246u,
            new(Chancellor, Destiny, new(-743.41797f, 81.389755f, 682.20215f)),
            new(Console, Destiny, new(-809.6202f, 83.2661f, 634.7396f)));

        private static readonly Briefing Chua = new(9557, 18247u, 18248u,
            new(Chancellor, Destiny, new(-809.6759f, 82.67818f, 885.2922f)),
            new(Console, Destiny, new(-809.70337f, 83.2661f, 901.19525f)));

        private sealed record Spot(uint Creature, ushort World, Vector3 Position);
        private sealed record Briefing(ushort Quest, uint Talk, uint Hologram, Spot Chancellor, Spot Console);

        internal static IEnumerable<(ushort Quest, uint Giver, uint Receiver)> Relations()
        {
            yield return (Embarkation, Pheydra, Pheydra);
            yield return (Mechari.Quest, Pheydra, Pheydra);
            yield return (Chua.Quest, Pheydra, Pheydra);
        }

        private static Briefing For(IPlayer player) => (uint)player.Race is 5u or 13u ? Chua : Mechari;
        private static bool Dominion(IPlayer player) => player is
            { Faction1: Faction.Dominion, Faction2: Faction.Dominion, InWorld: true, IsAlive: true, IsLoading: false };
        private static bool Finished(IPlayer player, ushort quest) => player.QuestManager.GetQuestState(quest) == QuestState.Completed;
        private static IQuest Active(IPlayer player, ushort quest) => player.QuestManager.GetActiveQuests().FirstOrDefault(candidate =>
            candidate.Id == quest && candidate is { State: QuestState.Accepted, PendingDelete: false });

        private static bool Is(IWorldEntity entity, Spot place) => entity is { EntityId: > 0u } &&
            entity.CreatureId == place.Creature && entity.Map?.Entry.Id == place.World &&
            Vector3.DistanceSquared(entity.Position, place.Position) <= 9f;

        private static bool Close(IPlayer player, IWorldEntity entity) =>
            ReferenceEquals(player.Map, entity.Map) && Vector3.DistanceSquared(player.Position, entity.Position) <= 36f;

        private static bool Nearby(IPlayer player, Spot spot) => player.Map?.Entry.Id == spot.World &&
            player.GetVisibleCreature<IWorldEntity>(spot.Creature).Any(entity => Is(entity, spot) && Close(player, entity));

        private static bool HasBoardingPrerequisite(IPlayer player) =>
            Dominion(player) && (uint)player.Race is 1u or 5u or 12u or 13u && (Finished(player, 9051) ||
                Finished(player, 7013) || Finished(player, 10530) ||
                player.QuestManager.GetActiveQuests().Any(quest => quest.Id == 10530 &&
                    quest.Any(objective => objective.ObjectiveInfo.Id == 21347u && objective.IsComplete())));

        internal static bool ReplacesPrerequisite(IPlayer player, ushort quest, ushort previous) =>
            quest == Embarkation && previous == 9051 && HasBoardingPrerequisite(player) &&
                (Nearby(player, Terminal) || Bridges.Any(spot => Nearby(player, spot)));

        internal static bool AllowsAccept(IPlayer player, ushort quest, bool byItem) => quest switch
        {
            Embarkation => !byItem && Dominion(player) && (Nearby(player, Terminal) || Bridges.Any(spot => Nearby(player, spot))),
            9556 or 9557 => !byItem && Dominion(player) && quest == For(player).Quest && Nearby(player, Terminal),
            _ => true
        };

        internal static bool AllowsReport(IPlayer player, ushort quest, bool remote)
        {
            if (quest == Embarkation)
                return !remote && Dominion(player) && Nearby(player, Terminal);
            if (quest is not (9556 or 9557))
                return true;

            return Dominion(player) && quest == For(player).Quest && (Nearby(player, Terminal) ||
                remote && player.Map?.Entry.Id == Destiny &&
                Vector3.DistanceSquared(player.Position, Terminal.Position) <= 25600f);
        }

        public static bool TryUse(IPlayer player, IWorldEntity entity)
        {
            if (Bridges.Any(spot => Is(entity, spot)))
            {
                if (HasBoardingPrerequisite(player) && Close(player, entity) && !player.InCombat && player.CanTeleport() &&
                    GameTableManager.Instance.World.GetEntry(Destiny) is { } ship &&
                    !DisableManager.Instance.IsDisabled(DisableType.World, Destiny))
                {
                    if (player.QuestManager.GetQuestState(Embarkation) is null or QuestState.Unknown or QuestState.Mentioned or QuestState.Abandoned)
                        player.QuestManager.QuestAdd(Embarkation, null);
                    if (player.QuestManager.GetQuestState(Embarkation) is QuestState.Accepted or QuestState.Achieved or QuestState.Completed)
                        player.TeleportTo(ship, Terminal.Position + new Vector3(1.4f, .1f, 0f));
                }
                return true;
            }

            if (!Is(entity, Mechari.Console) && !Is(entity, Chua.Console))
                return false;

            Briefing briefing = For(player);
            if (Dominion(player) && Is(entity, briefing.Console) && Close(player, entity) &&
                GameTableManager.Instance.Creature2.GetEntry(Console)?.Spell4IdActivate00 == 72033u &&
                GameTableManager.Instance.Spell4Effects.GetEntry(184973u) is { SpellId: 72033u, EffectType: SpellEffectType.Activate } &&
                Active(player, briefing.Quest) is { } quest &&
                quest.Any(o => o.ObjectiveInfo.Id == briefing.Talk && o.IsComplete()))
                quest.ObjectiveUpdate(briefing.Hologram, 1u);
            return true;
        }

        public static bool AllowsTalk(IPlayer player, IWorldEntity entity, bool talk)
        {
            if (!Is(entity, Mechari.Chancellor) && !Is(entity, Chua.Chancellor))
                return true;

            return talk && Dominion(player) && Is(entity, For(player).Chancellor) && Close(player, entity);
        }

        internal static void RecordArrival(IPlayer player)
        {
            LegacyDestinyBoarding.RecordArrival(player);
            if (!Dominion(player) || player.Map?.Entry.Id != Destiny ||
                Vector3.DistanceSquared(player.Position, Terminal.Position) > 25f ||
                Active(player, Embarkation) is not { } quest ||
                !quest.Any(goal => goal.ObjectiveInfo is { Id: 12866u, Type: QuestObjectiveType.EnterArea, Entry.Data: 6498u }))
                return;

            quest.ObjectiveUpdate(12866u, 1u);
        }
    }
}
