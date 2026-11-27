using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Map;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Quest
{
    public static class WinterfestConversions
    {
        internal const uint ChecklistMask = 0x3f;
        private const uint Count = 6;
        private const float CastRangeSquared = 400f;
        private sealed record Route(ushort Quest, uint Goal, uint World, Faction Faction, uint Contact, uint Target,
            uint ContactMarker, uint TargetMarker, uint CastSpell, uint Hologram);

        private static readonly Route Illium = new(10416, 21095, 22, Faction.Dominion, 72263, 72248, 51031, 51030, 83225, 72252);
        private static readonly Route Thayd = new(10417, 21096, 51, Faction.Exile, 72264, 72257, 51199, 51189, 83244, 72259);
        private sealed class Shortcut { public Route Shown { get; set; } }
        private static readonly ConditionalWeakTable<IPlayer, Shortcut> shortcuts = new();

        internal static bool Owns(uint id) => id is 10416 or 10417;
        internal static bool OwnsObjective(uint quest, uint objective) => GetRoute(quest)?.Goal == objective;
        internal static uint ClientProgress(ushort quest, IQuestObjective objective) => OwnsObjective(quest, objective.ObjectiveInfo.Id)
            ? (uint)BitOperations.PopCount(objective.Progress & ChecklistMask)
            : objective.Progress;

        internal static bool IsOpen(DateTime now)
        {
            var end = new DateTime(now.Year, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var start = new DateTime(now.Year, 12, 18, 10, 0, 0, DateTimeKind.Utc);
            return now < end || now >= start;
        }

        private static Route GetRoute(uint id) => id switch { 10416 => Illium, 10417 => Thayd, _ => null };
        private static Route GetWorld(uint id) => id switch { 22 => Illium, 51 => Thayd, _ => null };

        internal static uint BindTarget(EntityModel model)
        {
            Route route = GetWorld(model.World);
            return route != null && model.Id != 0u && model.Creature == route.Target && model.QuestChecklistIdx < Count &&
                AtMarker(route, route.TargetMarker, model.Area, new Vector3(model.X, model.Y, model.Z), 0f)
                ? 1u << model.QuestChecklistIdx : 0u;
        }

        internal static bool AtContact(IPlayer player, uint id)
        {
            Route route = GetRoute(id);
            return route != null && CanTakePart(player, route) &&
                player.GetVisibleCreature<INonPlayerEntity>(route.Contact).Any(actor =>
                    actor.EntityId != 0u && actor.IsAlive && ReferenceEquals(player.Map, actor.Map) &&
                    Vector3.DistanceSquared(player.Position, actor.Position) <= 36f &&
                    actor.Zone != null && AtMarker(route, route.ContactMarker, actor.Zone.Id, actor.Position, 12f));
        }

        internal static bool CanSee(IPlayer player, IGridEntity entity)
        {
            if (entity is WinterfestHologram hologram)
                return ReferenceEquals(hologram.Owner, player);
            if (entity is not NonPlayerEntity { EntityId: > 0u, Map.Entry.Id: var world } actor ||
                GetWorld(world) is not { } route || (actor.CreatureId != route.Contact && actor.WinterfestConversionBit == 0u))
                return true;
            if (player.Faction1 != route.Faction)
                return false;

            IQuest quest = ActiveQuest(player, route);
            if (actor.CreatureId == route.Contact)
                return IsOpen(DateTime.UtcNow) || quest is { PendingDelete: false, State: QuestState.Accepted or QuestState.Achieved };
            return GetGoal(quest, route) != null;
        }

        internal static void UpdateShortcut(IPlayer player)
        {
            Route available = player.Map?.Entry?.Id is { } world && GetWorld(world) is { } route &&
                CanTakePart(player, route) && GetGoal(ActiveQuest(player, route), route) != null ? route : null;
            Shortcut state = shortcuts.GetValue(player, _ => new Shortcut());
            if (ReferenceEquals(state.Shown, available) || !player.Session.CanProcessIncomingPackets)
                return;
            if (state.Shown != null)
                SendShortcut(player, state.Shown, false);
            if (available != null)
                SendShortcut(player, available, true);
            state.Shown = available;
        }

        private static void SendShortcut(IPlayer player, Route route, bool show) =>
            player.Session.EnqueueMessageEncrypted(new ServerQuestSpellShortcut
            { SpellId = route.CastSpell, SourceId = route.Goal, Reason = 2, AddRemove = show });

        public static void StartCast(IPlayer player, uint spellId, uint targetGuid)
        {
            if (player.Map?.Entry?.Id is not { } world || GetWorld(world) is not { } route ||
                route.CastSpell != spellId || !CanTakePart(player, route) ||
                GetTarget(player, route, targetGuid) is not { } target ||
                GetGoal(ActiveQuest(player, route), route) is not { } objective ||
                (objective.Progress & target.WinterfestConversionBit) != 0u ||
                (player is UnitEntity unit && unit.GetActiveSpell(s => s.IsCasting && s.Parameters.SpellInfo.Entry.Id == spellId) != null))
                return;

            player.CastSpell(spellId, new SpellParameters { UserInitiatedSpellCast = true, PrimaryTargetId = targetGuid });
        }

        internal static void CompleteCast(ISpell spell)
        {
            if (spell.Caster is not IPlayer player || player.Map?.Entry?.Id is not { } world ||
                GetWorld(world) is not { } route || spell.Parameters.SpellInfo.Entry.Id != route.CastSpell ||
                !CanTakePart(player, route) || GetTarget(player, route, spell.Parameters.PrimaryTargetId) is not { } target)
                return;
            IQuest quest = ActiveQuest(player, route);
            if (GetGoal(quest, route) is not { } objective || (objective.Progress & target.WinterfestConversionBit) != 0u)
                return;

            var effect = spell.Parameters.SpellInfo.Effects.SingleOrDefault(e => e.EffectType == SpellEffectType.SummonCreature &&
                e.DataBits00 == route.Hologram && e.TargetFlags == (uint)SpellEffectTargetFlags.Target &&
                e.DelayTime == 0u && e.DurationTime == 7000u);
            if (effect == null)
                return;

            var hologram = LegacyServiceProvider.Provider.GetRequiredService<IEntityFactory>().CreateEntity<WinterfestHologram>();
            var position = new MapPosition { Position = target.Position + new Vector3(1.5f, 0f, 0f) };
            hologram.Prepare(player, route.Hologram, effect.DurationTime / 1000d);
            if (!player.Map.CanEnter(hologram, position))
            {
                hologram.Dispose();
                return;
            }

            player.Map.EnqueueAdd(hologram, position);
            quest.ObjectiveUpdate(route.Goal, target.WinterfestConversionBit);
        }

        private static NonPlayerEntity GetTarget(IPlayer player, Route route, uint guid) =>
            guid != 0u && player.GetVisible<IWorldEntity>(guid) is NonPlayerEntity { IsAlive: true, WinterfestConversionBit: > 0u } target &&
            target.CreatureId == route.Target && ReferenceEquals(target.Map, player.Map) &&
            Vector3.DistanceSquared(player.Position, target.Position) <= CastRangeSquared ? target : null;

        private static IQuest ActiveQuest(IPlayer player, Route route) =>
            player.QuestManager.GetActiveQuests().FirstOrDefault(quest => quest.Id == route.Quest);
        private static IQuestObjective GetGoal(IQuest quest, Route route) =>
            quest is { State: QuestState.Accepted, PendingDelete: false }
                ? quest.FirstOrDefault(o => o.ObjectiveInfo.Id == route.Goal &&
                    o.ObjectiveInfo is { Type: QuestObjectiveType.ActivateEntity, Entry: { Data: 72383u, Count: Count } } && !o.IsComplete())
                : null;

        private static bool CanTakePart(IPlayer player, Route route) => player.InWorld && !player.IsLoading && player.IsAlive &&
            player.Map?.Entry?.Id == route.World && player.Faction1 == route.Faction;
        private static bool AtMarker(Route route, uint id, uint zone, Vector3 position, float minimumRadius)
        {
            if (GameTableManager.Instance.WorldLocation2.GetEntry(id) is not { } marker ||
                marker.WorldId != route.World || marker.WorldZoneId != zone)
                return false;
            float radius = Math.Max(minimumRadius, marker.Radius);
            return Vector2.DistanceSquared(new(position.X, position.Z), new(marker.Position0, marker.Position2)) <= radius * radius;
        }
    }
}
