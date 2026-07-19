using System.Numerics;
using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.AI
{
    //[ScriptFilterIgnore]
    public class CombatAI : IOwnedScript<ICreatureEntity>, IUnitScript
    {
        private ICreatureEntity owner;

        // Interval in seconds between chase path recalculations.
        private const double ChaseUpdateInterval = 0.5d;
        private double chaseTimer = 0d;

        public void OnLoad(ICreatureEntity owner)
        {
            this.owner = owner;
        }

        public void Update(double lastTick)
        {
            if (!owner.IsAlive)
                return;

            if (!owner.InCombat || owner.TargetGuid == null)
                return;

            chaseTimer -= lastTick;
            if (chaseTimer > 0d)
                return;
            chaseTimer = ChaseUpdateInterval;

            IUnitEntity target = owner.GetVisible<IUnitEntity>(owner.TargetGuid.Value);
            if (target == null || !target.IsAlive)
                return;

            float distance = Vector3.Distance(owner.Position, target.Position);
            if (distance > owner.HitRadius + target.HitRadius)
                owner.MovementManager.Follow(target, target.HitRadius);
        }

        public void OnThreatAddTarget(IHostileEntity hostile)
        {
            SelectTarget();
        }

        public void OnThreatRemoveTarget(IHostileEntity hostile)
        {
            SelectTarget();
        }

        public void OnThreatChange(IHostileEntity hostile)
        {
            SelectTarget();
        }

        protected virtual void SelectTarget()
        {
            IHostileEntity hostile = owner.ThreatManager.GetTopHostile();
            if (hostile == null)
            {
                owner.SetTarget((IWorldEntity)null);
                return;
            }

            owner.SetTarget(hostile.HatedUnitId, hostile.Threat);
        }
    }
}
