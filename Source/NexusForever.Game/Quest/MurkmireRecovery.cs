using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Quest
{
    public sealed class MurkmireRecovery : QuestActorActivation
    {
        internal const ushort World = 51;
        private const uint Cocoon = 42664;
        private const uint Pipe = 42142;
        private const int FirstSlot = 200;
        private const int RequiredCount = 10;
        private const uint UseSpell = 1817;
        private const uint UseEffect = 2821;
        private const uint UseTime = 3000;
        private const float UseRange = 5f;

        private sealed record Route(ushort Quest, uint Goal, Faction Faction, uint Actor,
            QuestObjectiveType Type, uint Data, uint[] Markers);

        private static readonly Route[] routes =
        [
            new(8240, 14988, Faction.Dominion, Cocoon, QuestObjectiveType.ActivateTargetGroup, 5454, [33075]),
            new(8241, 15019, Faction.Exile, Cocoon, QuestObjectiveType.ActivateTargetGroup, 5593, [33754, 33073]),
            new(8246, 15038, Faction.Exile, Pipe, QuestObjectiveType.ActivateTargetGroupChecklist, 5586, [33077, 34078, 34081]),
            new(8247, 15023, Faction.Dominion, Pipe, QuestObjectiveType.SucceedCSI, Pipe, [33077, 34078, 34081])
        ];

        private readonly IPlayer player;
        private readonly IWorldEntity target;
        private readonly uint targetGuid;
        private readonly Quest quest;
        private readonly QuestObjective objective;
        private readonly uint bit;

        private MurkmireRecovery(IPlayer player, IWorldEntity target, Quest quest, QuestObjective objective, uint bit, Spell4EffectsEntry effect)
        {
            this.player = player;
            this.target = target;
            targetGuid = target.Guid;
            this.quest = quest;
            this.objective = objective;
            this.bit = bit;
            ActivationEffect = effect;
        }

        internal override IWorldEntity Target => target;
        internal override Spell4EffectsEntry ActivationEffect { get; }

        internal override double CastSeconds => UseTime / 1000d;

        internal static bool CanVisitContact(IPlayer player, uint quest)
        {
            (uint contact, uint area) = quest switch
            {
                8240u => (41419u, 955u),
                8241u => (41414u, 954u),
                8246u => (41358u, 956u),
                8247u => (41359u, 956u),
                _ => (0u, 0u)
            };
            if (contact == 0u)
                return true;

            const float contactRange = 6f;
            return player.Map?.Entry.Id == World && player.GetVisibleCreature<INonPlayerEntity>(contact)
                .Any(actor => actor.IsAlive && ReferenceEquals(actor.Map, player.Map) && actor.Zone?.Id == area &&
                    Vector3.DistanceSquared(player.Position, actor.Position) <= contactRange * contactRange);
        }

        internal static bool? MeetsPeniaPrerequisite(IPlayer player, Quest2Entry quest)
        {
            if (quest is not { Id: 8240u, PrerequisiteId: 16528u } ||
                GameTableManager.Instance.Prerequisite.GetEntry(16528) is not
                    { Flags: EvaluationMode.EvaluateAND } requirement ||
                requirement.PrerequisiteTypeId.Count(type => type != PrerequisiteType.None) != 1 ||
                requirement.PrerequisiteTypeId[0] != PrerequisiteType.IsObjectiveActive ||
                requirement.PrerequisiteComparisonId[0] != PrerequisiteComparison.Equal ||
                requirement.ObjectId[0] != 8193u || requirement.Value[0] != 0u)
                return null;

            // This Dominion contact requires the active energy-cell objective of its faction's parallel quest.
            Quest prerequisite = player.QuestManager.GetActiveQuests().OfType<Quest>()
                .SingleOrDefault(active => active is { Id: 8200, State: QuestState.Accepted, PendingDelete: false });
            return player.Faction1 == Faction.Dominion &&
                prerequisite?.FirstOrDefault(goal => goal.ObjectiveInfo.Id == 15059u) is { Index: 0 } objective &&
                objective.ObjectiveInfo.Type == QuestObjectiveType.VirtualCollect &&
                objective.ObjectiveInfo.Entry is { Data: 605u, Count: 10u } &&
                !objective.IsComplete() && prerequisite.CanUpdateObjective(objective);
        }

        internal static bool HandlesObjective(ushort quest, uint objective) =>
            routes.Any(route => route.Quest == quest && route.Goal == objective);

        internal static bool Tracks(IGridEntity actor) => GetSlot(actor) != 0u;

        internal static uint BindSlot(EntityModel model)
        {
            int index = model.QuestChecklistIdx - FirstSlot;
            if (index < 0 || index >= RequiredCount || model.World != World || model.Id == 0u ||
                model is not ({ Type: EntityType.NonPlayer, Creature: Cocoon } or { Type: EntityType.Simple, Creature: Pipe }))
                return 0u;

            return routes.Any(route => route.Actor == model.Creature &&
                InMarkers(route, model.Area, new Vector3(model.X, model.Y, model.Z))) ? 1u << index : 0u;
        }

        internal static bool CanSee(IPlayer player, IGridEntity actor) =>
            !Tracks(actor) || FindObjective(player, (IWorldEntity)actor).Objective != null;

        public static bool TryBeginUse(IPlayer player, IWorldEntity target)
        {
            if (!Tracks(target))
                return false;

            if (!InRange(player, target) ||
                player is UnitEntity unit && unit.GetActiveSpell(spell => spell.IsCasting && spell is Spell.Spell { QuestActorUse: not null }) != null)
                return true;

            var (quest, objective, bit) = FindObjective(player, target);
            if (objective == null ||
                target.CreatureEntry.Spell4IdActivate00 != UseSpell ||
                target.CreatureEntry.ActivateSpellCastTime != UseTime ||
                GameTableManager.Instance.Spell4.GetEntry(UseSpell) is not { TierIndex: > 0u and <= byte.MaxValue } entry)
                return true;

            ISpellInfo spellInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(entry.Spell4BaseIdBaseSpell)?.GetSpellInfo((byte)entry.TierIndex);
            Spell4EffectsEntry activation = spellInfo?.Effects.SingleOrDefault(effect => effect is
                { Id: UseEffect, EffectType: SpellEffectType.Activate, TargetFlags: (uint)SpellEffectTargetFlags.Target });
            if (activation == null || spellInfo.Entry.Id != UseSpell)
                return true;

            player.CastSpell(new SpellParameters
            {
                SpellInfo = spellInfo,
                UserInitiatedSpellCast = true,
                PrimaryTargetId = target.Guid,
                QuestActorUse = new MurkmireRecovery(player, target, quest, objective, bit, activation)
            });
            return true;
        }

        internal override bool CanFinish()
        {
            if (target.Guid != targetGuid || !InRange(player, target))
                return false;

            var (currentQuest, currentObjective, currentBit) = FindObjective(player, target);
            return ReferenceEquals(currentQuest, quest) && ReferenceEquals(currentObjective, objective) && currentBit == bit;
        }

        internal override void Complete()
        {
            if (objective.ObjectiveInfo.Type == QuestObjectiveType.ActivateTargetGroupChecklist)
                quest.ObjectiveUpdate(objective.ObjectiveInfo.Id, bit);
            else if (objective.TryMarkChecklist(bit))
                quest.ObjectiveUpdate(objective.ObjectiveInfo.Id, 1u);

            if (player is Player viewer)
                viewer.RefreshMurkmireRecoveryActors();
        }

        private static uint GetSlot(IGridEntity actor) => actor switch
        {
            NonPlayerEntity unit => unit.MurkmireRecoverySlot,
            SimpleEntity prop => prop.MurkmireRecoverySlot,
            _ => 0u
        };

        private static bool InRange(IPlayer player, IWorldEntity target) =>
            target.Guid != 0u && player.InWorld && ReferenceEquals(player.GetVisible<IWorldEntity>(target.Guid), target) &&
            Vector3.DistanceSquared(player.Position, target.Position) <= UseRange * UseRange;

        private static (Quest Quest, QuestObjective Objective, uint Bit) FindObjective(IPlayer player, IWorldEntity actor)
        {
            if (player is not { Map.Entry.Id: World, IsAlive: true, IsLoading: false } ||
                !ReferenceEquals(player.Map, actor.Map) || actor is not IUnitEntity { IsAlive: true })
                return default;

            uint bit = GetSlot(actor);
            if (bit == 0u || actor.Zone == null)
                return default;

            Route route = routes.FirstOrDefault(route => route.Actor == actor.CreatureId &&
                route.Faction == player.Faction1 && InMarkers(route, actor.Zone.Id, actor.LeashPosition));
            if (route == null)
                return default;

            Quest quest = player.QuestManager.GetActiveQuests().OfType<Quest>().FirstOrDefault(active =>
                active.Id == route.Quest && active is { State: QuestState.Accepted, PendingDelete: false });
            if (quest?.FirstOrDefault(objective => objective.ObjectiveInfo.Id == route.Goal) is not QuestObjective goal ||
                goal.ObjectiveInfo.Type != route.Type || goal.ObjectiveInfo.Entry.Data != route.Data ||
                goal.ObjectiveInfo.Entry.Count != RequiredCount || goal.IsComplete() || !quest.CanUpdateObjective(goal) ||
                (route.Type == QuestObjectiveType.ActivateTargetGroupChecklist ? (goal.Progress & bit) != 0u : goal.HasChecklist(bit)))
                return default;

            return (quest, goal, bit);
        }

        private static bool InMarkers(Route route, uint zone, Vector3 position) =>
            route.Markers.Any(id => GameTableManager.Instance.WorldLocation2.GetEntry(id) is { WorldId: World } marker &&
                marker.WorldZoneId == zone && Vector2.DistanceSquared(new(position.X, position.Z),
                    new(marker.Position0, marker.Position2)) <= marker.Radius * marker.Radius);
    }
}
