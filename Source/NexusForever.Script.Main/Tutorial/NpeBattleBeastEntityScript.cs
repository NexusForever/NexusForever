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
    /// NPE Dominion Battle Beasts (73464). CombatAI.Update is still TODO, so these need a
    /// lightweight aggro + melee loop for Face of the Enemy objective 0.
    /// </summary>
    [ScriptFilterCreatureId(73464)]
    public class NpeBattleBeastEntityScript : IOwnedScript<ICreatureEntity>, IUnitScript
    {
        // Hostile to both Dominion and Exile (same sim faction as NPE turrets).
        private const Faction HostileSimFaction = (Faction)880u;

        // Warhound Unarmed auto-attack — closest themed instant melee with a Damage effect.
        private const uint MeleeSpellId = 25875u;
        private const float AggroRange  = 25f;
        private const float MeleeRange  = 5.5f;
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
            nextAttackSeconds = 1.6d;
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
