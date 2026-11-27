using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Quest;
using NexusForever.Game.Static.Prerequisite;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Spell
{
    public sealed class QuestCsiInteraction
    {
        private const uint ClientSideInteractionCastMethod = 5u;
        private const float DefaultInteractionRange = 5f;

        internal IWorldEntity Target { get; }
        internal Spell4EffectsEntry ActivateEffect { get; }
        internal double TimeoutSeconds { get; }

        private readonly IPlayer player;
        private readonly IBaseMap map;
        private readonly uint targetGuid;
        private readonly uint creatureId;
        private readonly uint prerequisite;
        private readonly Action<IPlayer, IWorldEntity> activate;
        private readonly Func<bool> stillEligible;

        private QuestCsiInteraction(IPlayer player, IWorldEntity target, uint prerequisite,
            ClientSideInteractionEntry descriptor, Spell4EffectsEntry activateEffect, Action<IPlayer, IWorldEntity> activate,
            Func<bool> stillEligible = null)
        {
            this.player = player;
            map = player.Map;
            Target = target;
            targetGuid = target.Guid;
            creatureId = target.CreatureId;
            this.prerequisite = prerequisite;
            this.activate = activate;
            this.stillEligible = stillEligible;
            ActivateEffect = activateEffect;
            // Give the reply time to arrive after the client's configured puzzle timer.
            TimeoutSeconds = descriptor.Duration is 0u or uint.MaxValue ? 300d : descriptor.Duration / 1000d + 5d;
        }

        /// <summary>
        /// Returns true when the activation belongs to a client-side interaction, even if the cast cannot start.
        /// </summary>
        public static bool TryBegin(IPlayer player, IWorldEntity target, Action<IPlayer, IWorldEntity> activate)
        {
            if (TryBeginTavern(player, target))
                return true;

            if (target.CreatureEntry is not { } creature)
                return false;

            var tables = GameTableManager.Instance;
            (uint Spell, uint Prerequisite)[] slots =
            [
                (creature.Spell4IdActivate00, creature.PrerequisiteIdActivateSpell00),
                (creature.Spell4IdActivate01, creature.PrerequisiteIdActivateSpell01),
                (creature.Spell4IdActivate02, creature.PrerequisiteIdActivateSpell02),
                (creature.Spell4IdActivate03, creature.PrerequisiteIdActivateSpell03)
            ];

            var variants = slots
                .Where(slot => slot.Spell != 0u)
                .Select(slot => (Spell: tables.Spell4.GetEntry(slot.Spell), slot.Prerequisite))
                .Where(slot => slot.Spell != null)
                .Select(slot => (slot.Spell, slot.Prerequisite, Base: tables.Spell4Base.GetEntry(slot.Spell.Spell4BaseIdBaseSpell)))
                .Where(slot => slot.Base != null)
                .ToArray();
            if (!variants.Any(variant => variant.Base.CastMethod == ClientSideInteractionCastMethod))
                return false;

            var selected = variants.FirstOrDefault(variant => variant.Prerequisite != 0u && Meets(player, variant.Prerequisite));
            if (selected.Spell == null)
                selected = variants.FirstOrDefault(variant => variant.Prerequisite == 0u);

            // An eligible ordinary activation continues through the existing non-CSI path.
            if (selected.Spell != null && selected.Base.CastMethod != ClientSideInteractionCastMethod)
                return false;

            if (selected.Spell == null || !CanInteract(player, target))
                return true;

            if (player.GetActiveSpell(cast => cast.IsCasting && cast is Spell { Interaction: not null }) is Spell pending)
            {
                if (pending.Interaction.IsCurrent(player))
                    return true;

                pending.CancelCast(CastResult.ClientSideInteractionFail);
            }

            ClientSideInteractionEntry descriptor = tables.ClientSideInteraction.GetEntry(selected.Base.ClientSideInteractionId);
            if (descriptor == null || selected.Spell.TierIndex is 0u or > byte.MaxValue)
                return true;

            ISpellInfo info = GlobalSpellManager.Instance.GetSpellBaseInfo(selected.Base.Id)
                ?.GetSpellInfo((byte)selected.Spell.TierIndex);
            // Other spells can have effects requiring their own server implementation.
            if (info?.Entry.Id != selected.Spell.Id || info.Effects.Count != 1 ||
                info.Effects[0].EffectType != SpellEffectType.Activate ||
                info.Effects[0].TargetFlags != (uint)SpellEffectTargetFlags.Target)
                return true;

            player.CastSpell(new SpellParameters
            {
                SpellInfo = info,
                UserInitiatedSpellCast = true,
                Interaction = new QuestCsiInteraction(player, target, selected.Prerequisite, descriptor, info.Effects[0], activate)
            });
            return true;
        }

        public static bool TryBeginTavern(IPlayer player, IWorldEntity target)
        {
            if (!BorellianTavern.IsMug(target))
                return false;
            var (quest, goal, spellId, csi, effectId, bit) = BorellianTavern.PrepareCsi(player, target);
            if (goal == null || target.CreatureEntry.Spell4IdActivate00 != spellId ||
                player.GetActiveSpell(cast => cast.IsCasting && cast is Spell { Interaction: not null }) != null)
                return true;

            var tables = GameTableManager.Instance;
            if (tables.Spell4.GetEntry(spellId) is not { TierIndex: > 0u and <= byte.MaxValue } row ||
                tables.Spell4Base.GetEntry(row.Spell4BaseIdBaseSpell) is not { CastMethod: ClientSideInteractionCastMethod } baseSpell ||
                baseSpell.ClientSideInteractionId != csi || tables.ClientSideInteraction.GetEntry(csi) is not { } descriptor ||
                tables.Spell4Effects.GetEntry(effectId) is not { } effect || effect.SpellId != spellId)
                return true;

            if (effect.EffectType != SpellEffectType.QuestAdvanceObjective || effect.DataBits00 != quest.Id ||
                  effect.DataBits01 != goal.Index || effect.DataBits02 != 1u ||
                  effect.TargetFlags != (uint)SpellEffectTargetFlags.Caster || effect.DelayTime != 0u || effect.PhaseFlags != 1u ||
                  tables.Prerequisite.GetEntry(effect.PrerequisiteIdCasterApply) is not { Flags: EvaluationMode.EvaluateAND } rule ||
                  rule.PrerequisiteTypeId[0] != PrerequisiteType.IsObjectiveActive ||
                  rule.PrerequisiteComparisonId[0] != PrerequisiteComparison.Equal ||
                  rule.ObjectId[0] != quest.Id || rule.Value[0] != goal.Index ||
                  rule.PrerequisiteTypeId.Skip(1).Any(type => type != PrerequisiteType.None))
                return true;

            var spell = GlobalSpellManager.Instance.GetSpellBaseInfo(baseSpell.Id)?.GetSpellInfo((byte)row.TierIndex);
            if (spell?.Entry.Id != spellId)
                return true;

            bool SameRound()
            {
                var current = BorellianTavern.PrepareCsi(player, target);
                return ReferenceEquals(current.Quest, quest) && ReferenceEquals(current.Goal, goal) && current.Bit == bit;
            }

            player.CastSpell(new SpellParameters
            {
                SpellInfo = spell,
                UserInitiatedSpellCast = true,
                Interaction = new QuestCsiInteraction(player, target, 0u, descriptor, effect,
                    BorellianTavern.CompleteCsi, SameRound)
            });
            return true;
        }

        public static void Resolve(IPlayer player, uint castingId, byte response)
        {
            if (player?.GetActiveSpell(cast => cast.CastingId == castingId && cast.IsCasting) is Spell spell &&
                spell.Interaction != null)
                spell.ResolveInteraction(player, response);
        }

        internal bool IsCurrent(IPlayer owner) =>
            ReferenceEquals(owner, player) && ReferenceEquals(player.Map, map) &&
            Target.Guid == targetGuid && Target.CreatureId == creatureId &&
            CanInteract(player, Target) && Meets(player, prerequisite) && (stillEligible?.Invoke() ?? true);

        internal void Complete()
        {
            activate(player, Target);
            if (!BorellianTavern.IsMug(Target))
                player.QuestManager.ObjectiveUpdate(QuestObjectiveType.SucceedCSI, creatureId, 1u);
        }

        private static bool Meets(IPlayer player, uint id) =>
            id == 0u || PrerequisiteManager.Instance.Meets(player, id);

        internal static bool CanInteract(IPlayer player, IWorldEntity target)
        {
            if (!player.IsAlive || player.IsLoading || player.Map == null ||
                !ReferenceEquals(player.Map, target.Map) || target.Guid == 0u ||
                !ReferenceEquals(player.GetVisible<IWorldEntity>(target.Guid), target))
                return false;

            float min = target.CreatureEntry.ActivateSpellMinRange;
            float max = target.CreatureEntry.ActivateSpellMaxRange;
            if (!float.IsFinite(min) || min < 0f)
                min = 0f;
            if (!float.IsFinite(max) || max <= 0f)
                max = DefaultInteractionRange;

            float distance = Vector3.Distance(player.Position, target.Position);
            return distance >= min && distance <= max;
        }
    }
}
