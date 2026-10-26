using System.Numerics;
using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Reputation;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    /// <summary>
    /// NPE hologram soldiers for Face of the Enemy final kill objective:
    /// Dominion Legionnaires (73492 / 73567) — Exile quest 10518, TargetGroup 14356;
    /// Exile Elites (73473 / 73566) — Dominion quest 10524, TargetGroup 14402.
    /// </summary>
    [ScriptFilterCreatureId(73492, 73567, 73473, 73566)]
    public class NpeHoloSoldierEntityScript : IOwnedScript<ICreatureEntity>, IUnitScript
    {
        private const Faction HostileSimFaction = (Faction)880u;

        // Warrior sword auto-attack — instant melee Damage vs PrimaryTarget.
        private const uint MeleeSpellId = 398u;
        private const float AggroRange  = 30f;
        private const float MeleeRange  = 6f;
        private const float FollowGap  = 2.5f;

        private ICreatureEntity owner;
        private IPlayer nearbyPlayer;
        private double nextAttackSeconds = 0.5d;
        private double nextFollowSeconds;

        public void OnLoad(ICreatureEntity owner)
        {
            this.owner = owner;
            owner.SetFaction(HostileSimFaction);
        }

        public void OnAddToMap(IBaseMap map)
        {
            owner.SetInRangeCheck(AggroRange);
            owner.SetFaction(HostileSimFaction);
        }

        public void OnEnterRange(IGridEntity entity)
        {
            if (entity is not IPlayer player || !player.IsAlive)
                return;

            nearbyPlayer = player;
            owner.ThreatManager.UpdateThreat(player, 1);
            owner.SetTarget(player);
        }

        public void OnExitRange(IGridEntity entity)
        {
            if (entity is IPlayer player && nearbyPlayer?.Guid == player.Guid)
                nearbyPlayer = null;
        }

        public void Update(double lastTick)
        {
            if (!owner.IsAlive)
                return;

            nextAttackSeconds -= lastTick;
            nextFollowSeconds -= lastTick;

            IPlayer target = ResolveTarget();
            if (target == null)
                return;

            owner.SetTarget(target);

            float distance = Vector3.Distance(owner.Position, target.Position);
            if (distance > MeleeRange)
            {
                if (nextFollowSeconds <= 0d)
                {
                    owner.MovementManager.Follow(target, FollowGap);
                    nextFollowSeconds = 1.25d;
                }
                return;
            }

            if (nextAttackSeconds > 0d)
                return;

            if (owner.GetActiveSpell(s => s.IsCasting) != null)
            {
                nextAttackSeconds = 0.25d;
                return;
            }

            owner.CastSpell(MeleeSpellId);
            nextAttackSeconds = 1.5d;
        }

        private IPlayer ResolveTarget()
        {
            IHostileEntity top = owner.ThreatManager.GetTopHostile();
            if (top != null)
            {
                IUnitEntity hated = owner.GetVisible<IUnitEntity>(top.HatedUnitId);
                if (hated is IPlayer player && player.IsAlive)
                    return player;
            }

            if (nearbyPlayer is { IsAlive: true })
                return nearbyPlayer;

            return null;
        }
    }
}
