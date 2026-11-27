using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Quest
{
    public sealed class FarsideExpeditionSupplies : QuestActorActivation
    {
        internal const ushort World = 1421;
        internal const ushort Ravine = 1424;
        internal const uint SupplyCrate = 27398;
        internal const byte CrateCount = 12;
        private const uint SupplyCache = 27935;
        private const uint SupplyItem = 64;
        private const uint GatherSpell = 1817;
        private const uint GatherEffect = 2821;
        private const float InteractionRange = 6f;

        private sealed record Route(ushort Quest, uint Objective, Faction Faction, uint Receiver);
        private static readonly Route exile = new(6155, 9387, Faction.Exile, 27366);
        private static readonly Route dominion = new(6459, 9967, Faction.Dominion, 29317);

        private readonly IPlayer player;
        private readonly CollectableUnitEntity crate;
        private readonly uint crateGuid;
        private readonly Quest quest;
        private readonly QuestObjective objective;
        private readonly uint bit;

        private FarsideExpeditionSupplies(IPlayer player, CollectableUnitEntity crate, Quest quest, QuestObjective objective, uint bit,
            Spell4EffectsEntry activationEffect)
        {
            this.player = player;
            this.crate = crate;
            this.quest = quest;
            this.objective = objective;
            this.bit = bit;
            crateGuid = crate.Guid;
            ActivationEffect = activationEffect;
        }

        internal override IWorldEntity Target => crate;
        internal override Spell4EffectsEntry ActivationEffect { get; }
        internal override double CastSeconds => crate.CreatureEntry.ActivateSpellCastTime / 1000d;

        internal static bool HandlesQuest(uint quest) => quest is 6155 or 6459;

        internal static bool HandlesObjective(ushort quest, uint objective) =>
            (quest, objective) is (6155, 9387) or (6459, 9967);

        internal static bool TracksCrate(IGridEntity entity) => entity is
            CollectableUnitEntity { EntityId: > 0u, CreatureId: SupplyCrate, Map.Entry.Id: World, Zone.Id: Ravine,
                QuestChecklistIdx: < CrateCount };

        internal static bool CanSee(IPlayer player, IGridEntity entity) => !TracksCrate(entity) ||
            FindObjective(player, (CollectableUnitEntity)entity).Objective != null;

        private static bool IsSupplyCache(IWorldEntity actor) =>
            actor is ISimpleEntity { EntityId: > 0u, CreatureId: SupplyCache, Map.Entry.Id: World, Zone.Id: Ravine };

        internal static bool CanVisitContact(IPlayer player, ushort quest, bool turnIn)
        {
            Route route = ForFaction(player.Faction1);
            if (route?.Quest != quest || player.Map?.Entry.Id != World)
                return false;

            if (turnIn)
                return player.GetVisibleCreature<INonPlayerEntity>(route.Receiver).Any(actor =>
                    actor is { EntityId: > 0u, IsAlive: true } && InRange(player, actor));

            return player.GetVisibleCreature<ISimpleEntity>(SupplyCache).Any(actor =>
                IsSupplyCache(actor) && InRange(player, actor));
        }

        public static bool TryInteract(IPlayer player, IWorldEntity actor)
        {
            if (IsSupplyCache(actor))
            {
                if (ForFaction(player.Faction1) != null && InRange(player, actor))
                    player.Session.EnqueueMessageEncrypted(new ServerDialogStart { DialogUnitId = actor.Guid });
                return true;
            }

            if (!TracksCrate(actor))
                return false;

            var crate = (CollectableUnitEntity)actor;
            if (!InRange(player, crate) ||
                player is UnitEntity unit && unit.GetActiveSpell(cast => cast.IsCasting && cast is Spell.Spell { QuestActorUse: not null }) != null)
                return true;

            var (quest, objective, bit) = FindObjective(player, crate);
            if (objective == null || crate.CreatureEntry.Spell4IdActivate00 != GatherSpell)
                return true;

            IGameTableManager tables = GameTableManager.Instance;
            if (tables.Spell4.GetEntry(GatherSpell) is not { TierIndex: > 0u and <= byte.MaxValue } entry ||
                tables.Spell4Base.GetEntry(entry.Spell4BaseIdBaseSpell) is not { CastMethod: 0u, ClientSideInteractionId: 0u })
                return true;

            ISpellInfo spell = GlobalSpellManager.Instance.GetSpellBaseInfo(entry.Spell4BaseIdBaseSpell)
                ?.GetSpellInfo((byte)entry.TierIndex);
            Spell4EffectsEntry effect = spell?.Effects.SingleOrDefault(effect => effect is
                { Id: GatherEffect, EffectType: SpellEffectType.Activate,
                    TargetFlags: (uint)SpellEffectTargetFlags.Target, DelayTime: 0u });
            if (effect == null || spell.Entry.Id != GatherSpell)
                return true;

            player.CastSpell(new SpellParameters
            {
                SpellInfo = spell,
                UserInitiatedSpellCast = true,
                QuestActorUse = new FarsideExpeditionSupplies(player, crate, quest, objective, bit, effect)
            });
            return true;
        }

        internal override bool CanFinish()
        {
            if (crate.Guid != crateGuid || !TracksCrate(crate) || !InRange(player, crate))
                return false;

            var (currentQuest, currentObjective, currentBit) = FindObjective(player, crate);
            return ReferenceEquals(currentQuest, quest) && ReferenceEquals(currentObjective, objective) && currentBit == bit;
        }

        internal override void Complete()
        {
            if (!objective.TryMarkChecklist(bit))
                return;

            quest.ObjectiveUpdate(objective.ObjectiveInfo.Id, 1u);
            if (player is Player viewer)
                viewer.RemoveVisible(crate);
        }

        private static Route ForFaction(Faction faction) => faction switch
        {
            Faction.Exile => exile,
            Faction.Dominion => dominion,
            _ => null
        };

        private static bool InRange(IPlayer player, IWorldEntity actor) => actor.Guid != 0u &&
            player is { InWorld: true, IsAlive: true, IsLoading: false } && ReferenceEquals(player.Map, actor.Map) &&
            ReferenceEquals(player.GetVisible<IWorldEntity>(actor.Guid), actor) &&
            Vector3.DistanceSquared(player.Position, actor.Position) <= InteractionRange * InteractionRange;

        private static (Quest Quest, QuestObjective Objective, uint Bit) FindObjective(IPlayer player, CollectableUnitEntity crate)
        {
            if (!ReferenceEquals(player.Map, crate.Map) || ForFaction(player.Faction1) is not { } route)
                return default;

            Quest quest = player.QuestManager.GetActiveQuests().OfType<Quest>().FirstOrDefault(active =>
                active.Id == route.Quest && active is { State: QuestState.Accepted, PendingDelete: false });
            if (quest?.FirstOrDefault(goal => goal.ObjectiveInfo.Id == route.Objective) is not QuestObjective objective ||
                objective.ObjectiveInfo.Type != QuestObjectiveType.VirtualCollect ||
                objective.ObjectiveInfo.Entry is not { Data: SupplyItem, Count: CrateCount } ||
                objective.IsComplete() || !quest.CanUpdateObjective(objective))
                return default;

            uint bit = 1u << crate.QuestChecklistIdx;
            return objective.HasChecklist(bit) ? default : (quest, objective, bit);
        }
    }
}
