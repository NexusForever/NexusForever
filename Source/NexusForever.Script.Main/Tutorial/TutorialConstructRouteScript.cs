using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Spell;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;
using NexusForever.Shared.Game;

namespace NexusForever.Script.Main.Tutorial
{
    [ScriptFilterOwnerId(TutorialWorld)]
    public sealed class TutorialConstructRouteScript : IMapScript, IOwnedScript<IBaseMap>
    {
        private const uint TutorialWorld = 3460u;
        private const uint ConstructZone = 5968u;
        private const uint Manifestation = 73500u;
        private const uint PlatformLight = 74988u;
        private const uint ManifestationGroup = 14425u;
        private const uint PlatformArea = 8548u;
        private const uint PlatformMarker = 51674u;
        private const uint PrimalJumpSpell = 85708u;

        private static readonly uint[] RingMarkers = [51698u, 51699u, 51700u];
        private static readonly (Property Property, uint Effect, float Multiplier)[] JumpEffects =
        [
            (Property.JumpHeight, 225375u, 4f),
            (Property.SlowFallMultiplier, 225376u, .05f),
            (Property.FallingDamageMultiplier, 225385u, 0f)
        ];

        private sealed record SpellPropertyModifier(Property Property, uint Priority, float Multiplier) : ISpellPropertyModifier
        {
            public List<IPropertyModifier> Alterations { get; } = [new PropertyMultiplier(Property, Multiplier)];
            public uint StackCount => 1u;
        }

        private sealed record PropertyMultiplier(Property Property, float Multiplier) : IPropertyModifier
        {
            public ModType ModType => ModType.Percentage;
            public float GetValue(uint level = 1u) => Multiplier;
        }

        private readonly IGameTableManager gameTableManager;
        private readonly UpdateTimer routeTimer = new(.25d);
        private readonly HashSet<IPlayer> players = new(ReferenceEqualityComparer.Instance);
        private readonly HashSet<IPlayer> jumping = new(ReferenceEqualityComparer.Instance);
        private IBaseMap tutorial;

        public TutorialConstructRouteScript(IGameTableManager gameTableManager) =>
            this.gameTableManager = gameTableManager;

        public void OnLoad(IBaseMap owner) => tutorial = owner;

        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is IPlayer player)
                players.Add(player);
        }

        public void OnRemoveFromMap(IGridEntity entity)
        {
            if (entity is IPlayer player)
            {
                players.Remove(player);
                ClearJump(player);
            }
        }

        public void Update(double lastTick)
        {
            routeTimer.Update(lastTick);
            if (!routeTimer.HasElapsed)
                return;

            routeTimer.Reset();
            if (tutorial?.Entry.Id != TutorialWorld)
                return;

            foreach (IPlayer player in players)
            {
                if (player is not { IsAlive: true, IsLoading: false, Zone.Id: ConstructZone } ||
                    !ReferenceEquals(player.Map, tutorial) || player.Faction1 != player.Faction2)
                {
                    ClearJump(player);
                    continue;
                }

                (ushort Quest, uint Rings, uint Platform) route = player.Faction1 switch
                {
                    Faction.Exile => ((ushort)10520, 21294u, 21295u),
                    Faction.Dominion => ((ushort)10523, 21307u, 21308u),
                    _ => default
                };
                if (route.Quest == 0u)
                {
                    ClearJump(player);
                    continue;
                }

                IQuest quest = player.QuestManager.GetActiveQuests()
                    .FirstOrDefault(active => active.Id == route.Quest && active.State == QuestState.Accepted &&
                        !active.PendingDelete);
                if (quest == null)
                {
                    ClearJump(player);
                    continue;
                }

                IQuestObjective ringGoal = quest.FirstOrDefault(goal => goal.Index == 0u);
                if (IsActive(quest, ringGoal))
                    CreditRings(player, quest, ringGoal, route.Rings);
                IQuestObjective platformGoal = quest.FirstOrDefault(goal => goal.Index == 2u);
                if (IsActive(quest, platformGoal))
                    CreditPlatform(player, quest, route.Platform);

                bool isClimbing = ringGoal is { Progress: > 0u } &&
                    (IsActive(quest, ringGoal) || IsActive(quest, platformGoal)) &&
                    (RingMarkers.Any(marker => IsNearRoute(player.Position, marker)) ||
                        IsNearRoute(player.Position, PlatformMarker));
                if (isClimbing)
                    ApplyJump(player);
                else
                    ClearJump(player);
            }
        }

        private static bool IsActive(IQuest quest, IQuestObjective goal) =>
            goal != null && !goal.IsComplete() &&
                (!goal.ObjectiveInfo.IsSequential() ||
                    quest.Where(previous => previous.Index < goal.Index && !previous.ObjectiveInfo.IsOptional()).All(previous => previous.IsComplete()));

        private void CreditRings(IPlayer player, IQuest quest, IQuestObjective goal, uint objectiveId)
        {
            if (goal?.ObjectiveInfo is not { Type: QuestObjectiveType.ActivateTargetGroupChecklist, Entry:
                    { Data: ManifestationGroup, Count: 3u } } info || info.Id != objectiveId)
                return;

            for (int index = 0; index < RingMarkers.Length; index++)
            {
                uint bit = 1u << index;
                // The visible effects are seeded below the authored pass-through locations.
                if ((goal.Progress & bit) == 0u && AtMarker(player, RingMarkers[index], Manifestation, 6f))
                    quest.ObjectiveChecklistBit(ManifestationGroup, (byte)index);
            }
        }

        private void CreditPlatform(IPlayer player, IQuest quest, uint objectiveId)
        {
            IQuestObjective goal = quest.FirstOrDefault(objective => objective.Index == 2u);
            if (goal?.ObjectiveInfo is not { Type: QuestObjectiveType.EnterArea, Entry:
                    { Data: PlatformArea, Count: 1u, WorldLocationsIdIndicator00: PlatformMarker } } info ||
                info.Id != objectiveId || !AtMarker(player, PlatformMarker, PlatformLight, 3f))
                return;

            quest.ObjectiveUpdate(objectiveId, 1u);
        }

        private void ApplyJump(IPlayer player)
        {
            if (jumping.Contains(player))
                return;

            var effects = new List<SpellPropertyModifier>(JumpEffects.Length);
            foreach ((Property property, uint effectId, float multiplier) in JumpEffects)
            {
                if (gameTableManager.Spell4Effects.GetEntry(effectId) is not
                        { SpellId: PrimalJumpSpell, EffectType: SpellEffectType.UnitPropertyModifier,
                            TargetFlags: (uint)SpellEffectTargetFlags.Target,
                            DataBits01: (uint)ModType.Percentage, DurationTime: 0u } effect ||
                    effect.DataBits00 != (uint)property ||
                    BitConverter.Int32BitsToSingle((int)effect.DataBits02) != multiplier)
                    return;
                effects.Add(new SpellPropertyModifier(property, effect.DataBits01, multiplier));
            }

            // All three authored rings use these identical movement modifiers. Their duration
            // is controlled by the tutorial, so keep them only while the player is on the route.
            foreach (SpellPropertyModifier effect in effects)
                player.AddSpellModifierProperty(effect, PrimalJumpSpell);
            jumping.Add(player);
        }

        private void ClearJump(IPlayer player)
        {
            if (!jumping.Remove(player))
                return;
            foreach ((Property property, _, _) in JumpEffects)
                player.RemoveSpellProperty(property, PrimalJumpSpell);
        }

        private bool IsNearRoute(Vector3 position, uint marker) =>
            gameTableManager.WorldLocation2.GetEntry(marker) is { WorldId: TutorialWorld, WorldZoneId: ConstructZone } at &&
            Within(position, new Vector3(at.Position0, at.Position1, at.Position2), 21f, 22f);

        private bool AtMarker(IPlayer player, uint markerId, uint creatureId, float propHeightTolerance)
        {
            if (gameTableManager.WorldLocation2.GetEntry(markerId) is not
                    { WorldId: TutorialWorld, WorldZoneId: ConstructZone } marker)
                return false;

            Vector3 position = new(marker.Position0, marker.Position1, marker.Position2);
            if (!Within(player.Position, position, 3.5f, 2.5f))
                return false;

            return player.GetVisibleCreature<IWorldEntity>(creatureId).Any(prop => prop.EntityId > 0u &&
                ReferenceEquals(prop.Map, tutorial) && Within(prop.Position, position, 1f, propHeightTolerance));
        }

        private static bool Within(Vector3 left, Vector3 right, float radius, float height)
        {
            Vector3 offset = left - right;
            return offset.X * offset.X + offset.Z * offset.Z <= radius * radius &&
                MathF.Abs(offset.Y) <= height;
        }
    }
}
