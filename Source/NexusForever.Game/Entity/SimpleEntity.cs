using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Creature;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Stat;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;

namespace NexusForever.Game.Entity
{
    public class SimpleEntity : UnitEntity, ISimpleEntity
    {
        public override EntityType Type => EntityType.Simple;

        #region Dependency Injection

        public SimpleEntity(
            IMovementManager movementManager,
            IEntitySummonFactory entitySummonFactory,
            IStatUpdateManager<IUnitEntity> statUpdateManager,
            ISpellFactory spellFactory,
            ICreatureInfoManager creatureInfoManager)
            : base(movementManager, entitySummonFactory, statUpdateManager, spellFactory)
        {
            statUpdateManager.Initialise(this);
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new SimpleEntityModel
            {
                CreatureId        = CreatureId,
                QuestChecklistIdx = QuestChecklistIdx
            };
        }

        public override void OnActivate(IPlayer activator)
        {

            if (CreatureInfo.Entry.DatacubeId != 0u)
                activator.DatacubeManager.AddDatacube((ushort)CreatureInfo.Entry.DatacubeId, int.MaxValue);

            if (CreatureInfo.Entry.DatacubeId != 0u)
            {
                // datacube is already known, resend it instead of adding it again which would throw
                IDatacube datacube = activator.DatacubeManager.GetDatacube((ushort)CreatureInfo.Entry.DatacubeId, DatacubeType.Datacube);
                if (datacube == null)
                    activator.DatacubeManager.AddDatacube((ushort)CreatureInfo.Entry.DatacubeId, int.MaxValue);
                else
                    activator.DatacubeManager.SendDatacube(datacube);
            }
        }

        public override void OnActivateSuccess(IPlayer activator)
        {
            base.OnActivateSuccess(activator);

            uint progress = (uint)(1 << QuestChecklistIdx);

            if (CreatureInfo.Entry.DatacubeId != 0u)
            {
                IDatacube datacube = activator.DatacubeManager.GetDatacube((ushort)CreatureInfo.Entry.DatacubeId, DatacubeType.Datacube);
                if (datacube == null)
                    activator.DatacubeManager.AddDatacube((ushort)CreatureInfo.Entry.DatacubeId, progress);
                else
                {
                    datacube.Progress |= progress;
                    activator.DatacubeManager.SendDatacube(datacube);
                }
            }

            if (CreatureInfo.Entry.DatacubeVolumeId != 0u)
            {
                IDatacube datacube = activator.DatacubeManager.GetDatacube((ushort)CreatureInfo.Entry.DatacubeVolumeId, DatacubeType.Journal);
                if (datacube == null)
                    activator.DatacubeManager.AddDatacubeVolume((ushort)CreatureInfo.Entry.DatacubeVolumeId, progress);
                else
                {
                    datacube.Progress |= progress;
                    activator.DatacubeManager.SendDatacubeVolume(datacube);
                }
            }
        }
    }
}
