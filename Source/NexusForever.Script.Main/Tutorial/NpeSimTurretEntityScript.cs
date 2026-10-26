using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Reputation;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.Tutorial
{
    /// <summary>
    /// NPE sim-turrets (Dominion 73494 / Exile 74862).
    /// Periodically starts an 8s Charging cast while a player is nearby; any damaging hit during that cast
    /// interrupts and opens a kill window (see AiTurretEntity.TakeDamage). No special interrupt ability required.
    /// </summary>
    [ScriptFilterCreatureId(73494, 74862)]
    public class NpeSimTurretEntityScript : IOwnedScript<ICreatureEntity>, IUnitScript
    {
        // Hostile to both Dominion and Exile (Faction2Relationship Reviled).
        private const Faction HostileSimFaction = (Faction)880u;

        private const uint ChargingDominionSpellId = 85469u;
        private const uint ChargingExileSpellId    = 86978u;
        private const float AggroRange             = 30f;

        private ICreatureEntity owner;
        private double nextChargeSeconds = 1.5d;
        private IPlayer nearbyPlayer;

        public void OnLoad(ICreatureEntity owner)
        {
            this.owner = owner;
            // DB may still say 219 (world/interactive); force attackable for both factions.
            owner.SetFaction(HostileSimFaction);
        }

        public void OnAddToMap(IBaseMap map)
        {
            owner.SetInRangeCheck(AggroRange);
            owner.SetFaction(HostileSimFaction);
        }

        public void OnEnterRange(IGridEntity entity)
        {
            if (entity is IPlayer player && player.IsAlive)
                nearbyPlayer = player;
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

            nextChargeSeconds -= lastTick;
            if (nextChargeSeconds > 0d)
                return;

            if (owner.GetActiveSpell(s => s.IsCasting) != null)
            {
                nextChargeSeconds = 0.5d;
                return;
            }

            IPlayer target = ResolveTarget();
            if (target == null)
            {
                nextChargeSeconds = 2d;
                return;
            }

            owner.SetTarget(target);
            uint chargingSpellId = owner.CreatureId == 74862u
                ? ChargingExileSpellId
                : ChargingDominionSpellId;

            owner.CastSpell(chargingSpellId);
            // Cast time is 8s; leave a short gap after the cast finishes before recharging.
            nextChargeSeconds = 12d;
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
