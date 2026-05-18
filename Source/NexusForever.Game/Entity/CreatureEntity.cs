using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Network.World.Message.Model;
using NexusForever.Script;

namespace NexusForever.Game.Entity
{
    /// <summary>
    /// An <see cref="ICreatureEntity"/> is an extension to <see cref="IUnitEntity"/> which contains logic specific to non player controlled combat entities.
    /// </summary>
    public abstract class CreatureEntity : UnitEntity, ICreatureEntity
    {
        #region Dependency Injection

        public CreatureEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);

            scriptCollection = ScriptManager.Instance.InitialiseEntityScripts<ICreatureEntity>(this);
        }

        /// <summary>
        /// Set target to supplied <see cref="IUnitEntity"/>.
        /// </summary>
        /// <remarks>
        /// A null target will clear the current target.
        /// </remarks>
        public override void SetTarget(IWorldEntity target, uint threat = 0)
        {
            base.SetTarget(target, threat);

            if (target is IPlayer player)
            {
                // plays aggro sound at client, maybe more??
                player.Session.EnqueueMessageEncrypted(new ServerEntityAggroSwitch
                {
                    UnitId   = Guid,
                    TargetId = TargetGuid.Value
                });
            }
        }

        /// <summary>
        /// Invoked when <see cref="ICreatureEntity"/> is targeted by another <see cref="IUnitEntity"/>.
        /// </summary>
        public override void OnTargeted(IUnitEntity source)
        {
            base.OnTargeted(source);

            // client only processes threat list message if the source matches the current target
            if (InCombat && source is IPlayer player)
                ThreatManager.SendThreatList(player.Session);
        }

        /// <summary>
        /// Invoked when a new <see cref="IHostileEntity"/> is added to the threat list.
        /// </summary>
        public override void OnThreatAddTarget(IHostileEntity hostile)
        {
            ThreatManager.BroadcastThreatList();
            base.OnThreatAddTarget(hostile);
        }

        /// <summary>
        /// Invoked when an existing <see cref="IHostileEntity"/> is removed from the threat list.
        /// </summary>
        public override void OnThreatRemoveTarget(IHostileEntity hostile)
        {
            ThreatManager.BroadcastThreatList();
            base.OnThreatRemoveTarget(hostile);
        }

        /// <summary>
        /// Invoked when an existing <see cref="IHostileEntity"/> is update on the threat list.
        /// </summary>
        public override void OnThreatChange(IHostileEntity hostile)
        {
            ThreatManager.BroadcastThreatList();
            base.OnThreatChange(hostile);
        }
    }
}
