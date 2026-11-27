using System.Numerics;
using System.Runtime.CompilerServices;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Message.Model;
using SpellCast = NexusForever.Game.Spell.Spell;

namespace NexusForever.Game.Quest
{
    public static class ShadeCapitalActivities
    {
        private const uint ThrowTorch = 72901;
        private const uint ThrowIlliumEgg = 72383;
        private const uint ThrowThaydEgg = 72636;
        private const int FirstBonfireSlot = 232;
        // Keep authored Hooligan slots separate from indices used by the stock Thayd spawns.
        private const int FirstHooliganSlot = 42;
        private const float CastRange = 20f;
        private const float GroundRadius = 5f;

        private sealed record Route(ushort Quest, uint Goal, uint World, Faction Faction, uint Contact, uint Target,
            uint ContactMarker, uint[] Markers, uint Data, uint RewardGroup, int Count, uint Spell, uint Proxy, uint Credit);

        private static readonly Route[] routes =
        [
            new(9588, 18322, 51, Faction.Exile, 63142, 63228, 46654, [47386, 47387, 47388, 47389],
                ThrowTorch, 10889, 15, ThrowTorch, 76765, 199189),
            new(9589, 18324, 22, Faction.Dominion, 63143, 63228, 46845, [47247, 47248, 47249, 47250],
                ThrowTorch, 10889, 15, ThrowTorch, 76771, 199210),
            new(9590, 18325, 51, Faction.Exile, 63144, 62672, 46768, [47122],
                10762, 10903, 12, ThrowThaydEgg, 76764, 199177),
            new(9591, 18326, 22, Faction.Dominion, 63673, 62672, 47061, [47185, 47186, 47187, 47188],
                ThrowIlliumEgg, 10903, 12, ThrowIlliumEgg, 76760, 199147)
        ];

        internal sealed class ShadeProjectile(IPlayer player, IWorldEntity target, IQuest startedQuest,
            uint bit, Spell4EffectsEntry visualEffect)
        {
            internal Vector3 Destination { get; } = target.Position;
            internal uint TargetId { get; } = target.Guid;

            internal SpellTargetInfo Resolve()
            {
                if (target.Guid != TargetId || !CanCastAt(player, target) || ActorBit(target) != bit)
                    return null;

                Route route = ActorRoute(target);
                IQuest quest = ActiveQuest(player, route);
                if (!ReferenceEquals(quest, startedQuest) || FindGoal(quest, route) is not { } objective ||
                    !objective.TryMarkChecklist(bit))
                    return null;

                quest.ObjectiveUpdate(route.Goal, 1u);
                SyncShortcut(player, quest);

                var hit = new SpellTargetInfo(SpellEffectTargetFlags.Telegraph, (IUnitEntity)target);
                hit.Effects.Add(new SpellTargetInfo.SpellTargetEffectInfo(GlobalSpellManager.Instance.NextEffectId, visualEffect));
                return hit;
            }

            internal void HideHitTarget()
            {
                if (player is Player viewer)
                    viewer.RemoveVisible(target);
            }
        }

        private sealed class ShortcutState
        {
            internal bool Shown { get; set; }
        }

        private static readonly Dictionary<ushort, Route> byQuest = routes.ToDictionary(route => route.Quest);
        private static readonly Dictionary<(uint World, uint Creature), Route> byActor = routes
            .SelectMany(route => new[] { (route.World, route.Contact, route), (route.World, route.Target, route) })
            .ToDictionary(pair => (pair.Item1, pair.Item2), pair => pair.route);
        private static readonly ConditionalWeakTable<IQuest, ShortcutState> shortcuts = new();

        internal static bool Owns(uint quest) => quest is 9588 or 9589 or 9590 or 9591;

        internal static (ushort Bonfires, ushort Hooligans) FindQuests(uint world) =>
            world == 51 ? ((ushort)9588, (ushort)9590) : ((ushort)9589, (ushort)9591);

        internal static bool HandlesObjective(ushort quest, uint objective) =>
            byQuest.GetValueOrDefault(quest)?.Goal == objective;

        public static bool HandlesSpell(uint spell) => spell is ThrowTorch or ThrowThaydEgg or ThrowIlliumEgg;

        internal static void SyncActiveShortcuts(IPlayer player, IReadOnlyDictionary<ushort, IQuest> quests)
        {
            foreach (Route route in routes)
                if (quests.TryGetValue(route.Quest, out IQuest quest))
                    SyncShortcut(player, quest);
        }

        internal static void SyncShortcut(IPlayer player, IQuest quest)
        {
            Route route = byQuest[quest.Id];
            ShortcutState state = shortcuts.GetValue(quest, _ => new ShortcutState());
            bool display = CanParticipate(player, route) && FindGoal(quest, route) != null;
            if (display == state.Shown)
                return;

            state.Shown = display;
            player.Session.EnqueueMessageEncrypted(new ServerQuestSpellShortcut
                { SpellId = route.Spell, SourceId = route.Goal, Reason = 2, AddRemove = display });
        }

        private static Route ActorRoute(IGridEntity entity) =>
            entity is IWorldEntity { EntityId: > 0u, Map.Entry.Id: var world } actor
                ? byActor.GetValueOrDefault((world, actor.CreatureId)) : null;

        internal static bool IsSeasonalActor(IGridEntity entity) => ActorRoute(entity) != null;

        internal static bool CanSeeActor(IPlayer visitor, IGridEntity entity)
        {
            Route route = ActorRoute(entity);
            if (route == null)
                return true;

            if (visitor.Faction1 != route.Faction || !ReferenceEquals(visitor.Map, entity.Map) ||
                visitor.QuestManager is not QuestManager journal)
                return false;

            if (((IWorldEntity)entity).CreatureId == route.Contact)
                return journal.CanSeeShadePlantQuest(route.Quest);

            uint bit = ActorBit(entity);
            return bit != 0u && FindGoal(ActiveQuest(visitor, route), route) is { } goal && !goal.HasChecklist(bit);
        }

        internal static uint BindSlot(EntityModel model)
        {
            Route route = byActor.GetValueOrDefault(((uint)model.World, model.Creature));
            int firstSlot = model.Creature == 62672u ? FirstHooliganSlot : FirstBonfireSlot;
            int slot = model.QuestChecklistIdx - firstSlot;
            if (route == null || model.Id == 0u || model.Creature != route.Target || slot < 0 || slot >= route.Count)
                return 0u;

            var point = new Vector3(model.X, model.Y, model.Z);
            return route.Markers.Any(marker => InMarker(route, marker, point, 0f)) ? 1u << slot : 0u;
        }

        internal static bool CanVisitContact(IPlayer player, ushort quest)
        {
            Route route = byQuest[quest];
            return CanParticipate(player, route) && player.GetVisibleCreature<INonPlayerEntity>(route.Contact).Any(actor =>
                actor.EntityId != 0u && actor.IsAlive && ReferenceEquals(player.Map, actor.Map) &&
                Vector3.DistanceSquared(player.Position, actor.Position) <= 36f &&
                InMarker(route, route.ContactMarker, actor.Position, 12f));
        }

        public static void TryStart(IPlayer player, uint spellId, uint targetGuid, Vector3 ground)
        {
            if (!HandlesSpell(spellId) || player.Map?.Entry.Id is not { } world ||
                routes.FirstOrDefault(route => route.Spell == spellId && route.World == world) is not { } route ||
                !CanParticipate(player, route) ||
                player.GetActiveSpell(cast => cast.IsCasting && cast is SpellCast { ShadeProjectile: not null }) != null)
                return;

            IQuest quest = ActiveQuest(player, route);
            QuestObjective goal = FindGoal(quest, route);
            if (goal == null)
                return;

            IWorldEntity target = targetGuid == 0u ? GroundTarget(player, route, goal, ground) :
                player.GetVisible<IWorldEntity>(targetGuid);
            if (target?.CreatureId != route.Target || !CanCastAt(player, target))
                return;

            uint bit = ActorBit(target);
            if (goal.HasChecklist(bit))
                return;

            GameTableManager tables = GameTableManager.Instance;
            Spell4Entry entry = tables.Spell4.GetEntry(spellId);
            Spell4EffectsEntry credit = tables.Spell4Effects.GetEntry(route.Credit);
            if (entry is not { TargetMaxRange: CastRange, TierIndex: > 0u and <= byte.MaxValue } ||
                credit is not { EffectType: SpellEffectType.QuestAdvanceObjective, TargetFlags: (uint)SpellEffectTargetFlags.Caster,
                    DataBits01: 0u, DataBits02: 1u } || credit.SpellId != route.Proxy || credit.DataBits00 != route.Quest)
                return;

            var spell = GlobalSpellManager.Instance.GetSpellBaseInfo(entry.Spell4BaseIdBaseSpell)?.GetSpellInfo((byte)entry.TierIndex);
            if (spell?.Entry.Id != spellId || !spell.Telegraphs.Any(telegraph =>
                    telegraph.DamageShapeEnum == (uint)DamageShape.Circle && telegraph.Param00 == GroundRadius))
                return;
            Spell4EffectsEntry link = spell.Effects.SingleOrDefault(effect => effect is
                { EffectType: SpellEffectType.Proxy, TargetFlags: (uint)SpellEffectTargetFlags.Telegraph } &&
                effect.DataBits00 == route.Proxy);
            if (link == null)
                return;

            player.CastSpell(new SpellParameters
            {
                SpellInfo = spell,
                PrimaryTargetId = target.Guid,
                Position = new Position(target.Position),
                UserInitiatedSpellCast = true,
                ShadeProjectile = new ShadeProjectile(player, target, quest, bit, link)
            });
        }

        private static IWorldEntity GroundTarget(IPlayer player, Route route, QuestObjective goal, Vector3 ground)
        {
            if (!(Vector3.DistanceSquared(ground, player.Position) <= CastRange * CastRange))
                return null;

            return player.GetVisibleCreature<IWorldEntity>(route.Target)
                .Where(actor => CanCastAt(player, actor) && !goal.HasChecklist(ActorBit(actor)))
                .Select(actor => (Actor: actor, Distance: Vector3.DistanceSquared(ground, actor.Position)))
                .Where(hit => hit.Distance <= GroundRadius * GroundRadius)
                .OrderBy(hit => hit.Distance)
                .ThenBy(hit => hit.Actor.Guid)
                .Select(hit => hit.Actor)
                .FirstOrDefault();
        }

        internal static bool CanCastAt(IPlayer player, IWorldEntity actor) =>
            ActorRoute(actor) is { } route && actor.CreatureId == route.Target &&
            ActorBit(actor) != 0u && actor is IUnitEntity { IsAlive: true } && CanParticipate(player, route) &&
            actor.Guid != 0u && ReferenceEquals(player.Map, actor.Map) &&
            ReferenceEquals(player.GetVisible<IWorldEntity>(actor.Guid), actor) &&
            Vector3.DistanceSquared(player.Position, actor.Position) <= CastRange * CastRange;

        private static uint ActorBit(IGridEntity actor) => actor switch
        {
            NonPlayerEntity unit => unit.ShadeCapitalSlot,
            SimpleEntity prop => prop.ShadeCapitalSlot,
            _ => 0u
        };

        private static IQuest ActiveQuest(IPlayer player, Route route) =>
            player.QuestManager.GetActiveQuests().FirstOrDefault(quest => quest.Id == route.Quest);

        private static QuestObjective FindGoal(IQuest quest, Route route)
        {
            if (quest is not { State: QuestState.Accepted, PendingDelete: false } ||
                quest.FirstOrDefault(objective => objective.ObjectiveInfo.Id == route.Goal) is not QuestObjective goal ||
                goal.ObjectiveInfo.Type != QuestObjectiveType.ActivateEntity || goal.ObjectiveInfo.Entry.Data != route.Data ||
                goal.ObjectiveInfo.Entry.Count != route.Count || goal.ObjectiveInfo.Entry.TargetGroupIdRewardPane != route.RewardGroup ||
                goal.IsComplete())
                return null;

            return goal;
        }

        private static bool CanParticipate(IPlayer player, Route route) =>
            player is { IsLoading: false, InWorld: true, IsAlive: true, Map.Entry.Id: var world } &&
            world == route.World && player.Faction1 == route.Faction;

        private static bool InMarker(Route route, uint markerId, Vector3 point, float minimumRadius)
        {
            if (GameTableManager.Instance.WorldLocation2.GetEntry(markerId) is not { } marker || marker.WorldId != route.World)
                return false;

            float radius = Math.Max(minimumRadius, marker.Radius);
            return Vector2.DistanceSquared(new(point.X, point.Z), new(marker.Position0, marker.Position2)) <= radius * radius;
        }
    }
}
