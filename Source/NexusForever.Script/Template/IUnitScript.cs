using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Spell;

namespace NexusForever.Script.Template
{
    public interface IUnitScript : IWorldEntityScript
    {
        /// <summary>
        /// Invoked when a new <see cref="IHostileEntity"/> is added to the threat list.
        /// </summary>
        void OnThreatAddTarget(IHostileEntity hostile)
        {
        }

        /// <summary>
        /// Invoked when an existing <see cref="IHostileEntity"/> is removed from the threat list.
        /// </summary>
        void OnThreatRemoveTarget(IHostileEntity hostile)
        {
        }

        /// <summary>
        /// Invoked when an existing <see cref="IHostileEntity"/> is update on the threat list.
        /// </summary>
        void OnThreatChange(IHostileEntity hostile)
        {
        }

        /// <summary>
        /// Invoked when health is changed by source <see cref="IUnitEntity"/>.
        /// </summary>
        void OnHealthChange(IUnitEntity source, uint amount, DamageType type)
        {
        }

        /// <summary>
        /// Invoked when <see cref="IUnitEntity"/> is killed.
        /// </summary>
        void OnDeath()
        {
        }
    }
}
