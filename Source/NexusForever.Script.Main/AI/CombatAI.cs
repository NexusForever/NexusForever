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

        public void OnLoad(ICreatureEntity owner)
        {
            this.owner = owner;
        }

        public void Update(double lastTick)
        {
            if (!owner.IsAlive)
                return;

            // TODO
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
