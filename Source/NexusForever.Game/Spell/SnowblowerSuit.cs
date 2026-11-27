using System.Numerics;
using System.Runtime.CompilerServices;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Map.Search;
using NexusForever.Game.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Abilities;

namespace NexusForever.Game.Spell
{
    public sealed class SnowblowerSuit
    {
        private const uint Snowblower = 70903;
        private const ushort SuitOutfit = 9286;
        private const ushort IlliumBar = 1639;
        private const ushort ThaydBar = 1646;
        private const uint StaticCharge = 80406;
        private const uint ThaydCharge = 83522;
        private const uint IlliumRemoveSuit = 80398;
        private const uint ThaydRemoveSuit = 83760;
        private const float ChargeRange = 5f;

        private sealed record Route(ushort Quest, uint EquipGoal, uint ChargerGoal, uint ReequipGoal,
            Faction Faction, uint World, uint FieldZone, uint Suit, uint EquipSpell, uint ChargeSpell,
            uint RemoveSpell, ushort ActionBar, uint ChargeData, float FieldRadius, uint[] SuitMarkers, uint[] FieldMarkers);

        private static readonly Route illium = new(10034, 19131, 19132, 19133, Faction.Dominion, 22, 2194,
            70902, 80397, StaticCharge, IlliumRemoveSuit, IlliumBar, 12985, 50f, [50228, 51271], [50224, 50225, 50226, 50227]);
        private static readonly Route thayd = new(10035, 21153, 21154, 21155, Faction.Exile, 51, 2554,
            72523, 83529, ThaydCharge, ThaydRemoveSuit, ThaydBar, 0, 70f, [51196], [51193, 51194]);

        private sealed class SuitState(IQuest quest, Route route, IPlayer player)
        {
            public IQuest Quest { get; } = quest;
            public Route Route { get; } = route;
            public IBaseMap Map { get; } = player.Map;
            public uint Display { get; } = player.DisplayInfo;
            public ushort Outfit { get; } = player.OutfitInfo;
        }

        private static readonly ConditionalWeakTable<IPlayer, SuitState> suits = new();

        private readonly IPlayer player;
        private readonly SuitState suit;
        private readonly IUnitEntity target;
        private readonly uint bit;
        private readonly Spell4EffectsEntry visual;

        private SnowblowerSuit(IPlayer player, SuitState suit, IUnitEntity target, uint bit, Spell4EffectsEntry visual)
        {
            this.player = player;
            this.suit = suit;
            this.target = target;
            this.bit = bit;
            this.visual = visual;
        }

        internal static bool IsSeasonalActor(IGridEntity entity) => ActorRoute(entity) != null;

        internal static bool CanSeeActor(IPlayer player, IGridEntity entity) =>
            ActorRoute(entity) is not { } route ||
            player.Faction1 == route.Faction &&
            (IsOpen(DateTime.UtcNow) || player.QuestManager.GetActiveQuests().Any(q => q.Id == route.Quest &&
                !q.PendingDelete && q.State is QuestState.Accepted or QuestState.Achieved));

        internal const uint ChecklistMask = 0x3f;

        internal static bool IsOpen(DateTime now) =>
            now < new DateTime(now.Year, 1, 1, 10, 0, 0, DateTimeKind.Utc) ||
            now >= new DateTime(now.Year, 12, 18, 10, 0, 0, DateTimeKind.Utc);

        internal static bool OwnsQuest(uint quest) => quest is 10034 or 10035;

        internal static bool IsSuit(IWorldEntity actor) => SuitRoute(actor) != null;

        internal static bool AtContact(IPlayer player, uint quest) =>
            GetFactionRoute(player) is { } route && route.Quest == quest && player.InWorld && !player.IsLoading && player.IsAlive &&
            player.GetVisibleCreature<INonPlayerEntity>(70909u).Any(actor => actor.EntityId != 0u && actor.IsAlive &&
                ReferenceEquals(actor.Map, player.Map) && Vector3.DistanceSquared(player.Position, actor.Position) <= 36f);

        private static Route GetFactionRoute(IPlayer player) =>
            player.Map?.Entry?.Id switch
            {
                22u when player.Faction1 == Faction.Dominion => illium,
                51u when player.Faction1 == Faction.Exile => thayd,
                _ => null
            };

        private static Route ActorRoute(IGridEntity entity)
        {
            if (entity is not IWorldEntity { EntityId: > 0u, Map.Entry: { } world } actor)
                return null;

            return (world.Id, actor.CreatureId) switch
            {
                (22u, 70909u) when actor is INonPlayerEntity => illium,
                (51u, 70909u) when actor is INonPlayerEntity => thayd,
                (22u, 70902u or Snowblower) when actor is ISimpleEntity => illium,
                (51u, 72523u or Snowblower) when actor is ISimpleEntity => thayd,
                _ => null
            };
        }

        internal static bool UsesMachineChecklist(uint questId, uint objectiveId) =>
            (questId == illium.Quest && objectiveId == illium.ChargerGoal) ||
            (questId == thayd.Quest && objectiveId == thayd.ChargerGoal);

        public static bool Handles(uint spellId) => spellId is StaticCharge or ThaydCharge or IlliumRemoveSuit or ThaydRemoveSuit;

        internal static bool AllowsCsi(IPlayer player, IWorldEntity target)
        {
            Route route = SuitRoute(target);
            if (route == null)
                return true;
            return EquipQuest(player, target, route) != null;
        }

        private static bool IsEquipEffect(IWorldEntity target, Spell4EffectsEntry effect)
        {
            Route route = SuitRoute(target);
            if (route == null || effect.SpellId != route.EquipSpell ||
                effect is not { TargetFlags: (uint)SpellEffectTargetFlags.Caster, DelayTime: 0u, TickTime: 0u, DurationTime: 0u })
                return false;

            return effect.EffectType switch
            {
                SpellEffectType.ActionBarSet => effect.DataBits00 is IlliumBar or ThaydBar,
                SpellEffectType.DisguiseOutfit => effect.DataBits00 == SuitOutfit,
                SpellEffectType.UnitPropertyModifier => effect.DataBits00 == 134u,
                _ => false
            };
        }

        internal static bool ValidateEquipEffects(IWorldEntity target, ISpellInfo info)
        {
            Route route = SuitRoute(target);
            return route != null && info.Entry.Id == route.EquipSpell &&
                info.Effects.Any(e => e.EffectType == SpellEffectType.ActionBarSet && e.DataBits00 == route.ActionBar) &&
                info.Effects.Any(e => e.EffectType == SpellEffectType.DisguiseOutfit && e.DataBits00 == SuitOutfit) &&
                info.Effects.All(e => IsEquipEffect(target, e) ||
                    e is { EffectType: SpellEffectType.Activate, TargetFlags: (uint)SpellEffectTargetFlags.Target } ||
                    e is { EffectType: SpellEffectType.Fluff, TargetFlags: (uint)SpellEffectTargetFlags.Caster });
        }

        internal static bool CompleteEquip(IPlayer player, IWorldEntity target, uint spell)
        {
            Route route = SuitRoute(target);
            if (route == null || spell != route.EquipSpell)
                return false;

            if (EquipQuest(player, target, route) is not { } quest)
                return true;

            var state = new SuitState(quest, route, player);
            suits.Add(player, state);
            player.SetVisualInfo(state.Display, SuitOutfit);
            ShowBar(player, route.ActionBar);

            if (ActiveGoal(quest, route.EquipGoal, QuestObjectiveType.SucceedCSI, route.Suit) != null)
                quest.ObjectiveUpdate(route.EquipGoal, 1u);
            else if (ActiveGoal(quest, route.ReequipGoal, QuestObjectiveType.SucceedCSI, 0u) != null)
                quest.ObjectiveUpdate(route.ReequipGoal, 1u);
            return true;
        }

        private static Route SuitRoute(IWorldEntity target) => target?.CreatureId switch
        {
            70902 when target.Map?.Entry.Id == illium.World => illium,
            72523 when target.Map?.Entry.Id == thayd.World => thayd,
            _ => null
        };

        private static IQuest EquipQuest(IPlayer player, IWorldEntity target, Route route)
        {
            if (target is not ISimpleEntity { EntityId: > 0u } || player.Faction1 != route.Faction ||
                player.PlatformGuid != null || suits.TryGetValue(player, out _) ||
                !route.SuitMarkers.Any(marker => NearMarker(target, marker, route.World, 4f)) ||
                !QuestCsiInteraction.CanInteract(player, target))
                return null;
            IQuest quest = player.QuestManager.GetActiveQuests().FirstOrDefault(q => q.Id == route.Quest);
            return ActiveGoal(quest, route.EquipGoal, QuestObjectiveType.SucceedCSI, route.Suit) != null ||
                ActiveGoal(quest, route.ChargerGoal, QuestObjectiveType.Unknown10, route.ChargeData) != null ? quest : null;
        }

        private static QuestObjective ActiveGoal(IQuest quest, uint id, QuestObjectiveType type, uint data)
        {
            if (quest is not { State: QuestState.Accepted, PendingDelete: false })
                return null;
            QuestObjective goal = quest.FirstOrDefault(candidate => candidate.ObjectiveInfo.Id == id &&
                candidate.ObjectiveInfo.Type == type && candidate.ObjectiveInfo.Entry.Data == data) as QuestObjective;
            return goal != null && !goal.IsComplete() && (!goal.ObjectiveInfo.IsSequential() ||
                quest.Where(candidate => candidate.Index < goal.Index).All(candidate => candidate.IsComplete())) ? goal : null;
        }

        private static bool NearMarker(IWorldEntity target, uint markerId, uint world, float range) =>
            GameTableManager.Instance.WorldLocation2.GetEntry(markerId) is { } marker && marker.WorldId == world &&
            Vector3.DistanceSquared(target.Position, new(marker.Position0, marker.Position1, marker.Position2)) <= range * range;

        private static void ShowBar(IPlayer player, ushort bar) => player.Session.EnqueueMessageEncrypted(new ServerShowActionBar
        {
            ShortcutSet = ShortcutSet.FloatingSpellBar, ActionBarShortcutSetId = bar, AssociatedUnitId = player.Guid
        });

        internal static void Clear(IPlayer player)
        {
            if (!suits.TryGetValue(player, out SuitState suit))
                return;
            suits.Remove(player);
            if (player.OutfitInfo == SuitOutfit)
                player.SetVisualInfo(suit.Display, suit.Outfit);
            ShowBar(player, 0);
        }

        internal static void Update(IPlayer player)
        {
            if (suits.TryGetValue(player, out SuitState suit) && !IsEquipped(player, suit))
                Clear(player);
        }

        private static bool IsEquipped(IPlayer player, SuitState suit) =>
            player is { IsAlive: true, IsLoading: false, InWorld: true, OutfitInfo: SuitOutfit, PlatformGuid: null } &&
            ReferenceEquals(player.Map, suit.Map) && player.Faction1 == suit.Route.Faction &&
            player.QuestManager.GetActiveQuests().Contains(suit.Quest) &&
            ActiveGoal(suit.Quest, suit.Route.ChargerGoal, QuestObjectiveType.Unknown10, suit.Route.ChargeData) != null;

        public static void TryChargeFromBar(IPlayer player, uint targetId)
        {
            if (suits.TryGetValue(player, out SuitState suit))
                TryStart(player, suit.Route.ChargeSpell, targetId);
        }

        public static void TryStart(IPlayer player, uint spell, uint targetId)
        {
            if (!suits.TryGetValue(player, out SuitState suit) || !IsEquipped(player, suit))
                return;

            if (spell == suit.Route.RemoveSpell)
            {
                Clear(player);
                return;
            }
            if (spell != suit.Route.ChargeSpell)
                return;
            if (player.GetActiveSpell(cast => !cast.IsFinished && cast.Parameters is SpellParameters { Snowblower: not null }) != null)
                return;

            IWorldEntity target = targetId != 0u && targetId != player.Guid
                ? player.GetVisible<IWorldEntity>(targetId)
                : player.Map.Search(player.Position, ChargeRange,
                    new SearchCheckRange<IWorldEntity>(player.Position, ChargeRange))
                    .Where(candidate => TargetBit(player, suit, candidate) != 0u)
                    .MinBy(candidate => Vector3.DistanceSquared(player.Position, candidate.Position));
            uint bit = TargetBit(player, suit, target);
            if (bit == 0u)
                return;

            Spell4Entry row = GameTableManager.Instance.Spell4.GetEntry(suit.Route.ChargeSpell);
            if (row is not { TierIndex: > 0u and <= byte.MaxValue })
                return;
            ISpellInfo info = GlobalSpellManager.Instance.GetSpellBaseInfo(row.Spell4BaseIdBaseSpell)
                ?.GetSpellInfo((byte)row.TierIndex);
            uint effectId = suit.Route.ChargeSpell == StaticCharge ? 212904u : 220767u;
            Spell4EffectsEntry visual = info?.Effects.SingleOrDefault(effect => effect.Id == effectId && effect.EffectType == SpellEffectType.Fluff);
            if (info?.Entry.Id != row.Id || visual == null)
                return;

            player.CastSpell(new SpellParameters
            {
                SpellInfo = info, UserInitiatedSpellCast = true, PrimaryTargetId = target.Guid,
                Snowblower = new SnowblowerSuit(player, suit, (IUnitEntity)target, bit, visual)
            });
        }

        private static uint TargetBit(IPlayer player, SuitState suit, IWorldEntity target)
        {
            if (target is not ISimpleEntity { EntityId: > 0u, CreatureId: Snowblower, QuestChecklistIdx: < 6 } machine ||
                !ReferenceEquals(player.Map, target.Map) || target.Zone?.Id != suit.Route.FieldZone ||
                !ReferenceEquals(player.GetVisible<IWorldEntity>(target.Guid), target) ||
                Vector3.DistanceSquared(player.Position, target.Position) > ChargeRange * ChargeRange ||
                !suit.Route.FieldMarkers.Any(marker => NearMarker(target, marker, suit.Route.World, suit.Route.FieldRadius)))
                return 0u;

            uint bit = 1u << machine.QuestChecklistIdx;
            QuestObjective goal = ActiveGoal(suit.Quest, suit.Route.ChargerGoal, QuestObjectiveType.Unknown10, suit.Route.ChargeData);
            return goal != null && (goal.Progress & bit) == 0u ? bit : 0u;
        }

        internal bool IsCurrent() => suits.TryGetValue(player, out SuitState active) && ReferenceEquals(active, suit) &&
            IsEquipped(player, suit) && TargetBit(player, suit, target) == bit;

        internal SpellTargetInfo Charge()
        {
            QuestObjective goal = ActiveGoal(suit.Quest, suit.Route.ChargerGoal, QuestObjectiveType.Unknown10, suit.Route.ChargeData);
            if (!IsCurrent() || goal == null || (goal.Progress & bit) != 0u)
                return null;

            suit.Quest.ObjectiveUpdate(suit.Route.ChargerGoal, bit);
            var hit = new SpellTargetInfo(SpellEffectTargetFlags.Telegraph, target);
            hit.Effects.Add(new SpellTargetInfo.SpellTargetEffectInfo(GlobalSpellManager.Instance.NextEffectId, visual));
            return hit;
        }
    }
}
