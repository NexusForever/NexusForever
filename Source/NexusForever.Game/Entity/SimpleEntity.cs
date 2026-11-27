using System.Numerics;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Quest;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class SimpleEntity : UnitEntity, ISimpleEntity
    {
        public override EntityType Type => EntityType.Simple;

        public byte QuestChecklistIdx { get; private set; }

        #region Dependency Injection

        public SimpleEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);
            QuestChecklistIdx = model.QuestChecklistIdx;
        }

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
            if (TryOpenHalonDiscovery(activator) || DeadGone.TryOpen(activator, this) || ArcanusSilo.TryOpen(activator, this))
                return;

            if (CreatureEntry.DatacubeId != 0u)
            {
                // datacube is already known, resend it instead of adding it again which would throw
                IDatacube datacube = activator.DatacubeManager.GetDatacube((ushort)CreatureEntry.DatacubeId, DatacubeType.Datacube);
                if (datacube == null)
                    activator.DatacubeManager.AddDatacube((ushort)CreatureEntry.DatacubeId, int.MaxValue);
                else
                    activator.DatacubeManager.SendDatacube(datacube);
            }
        }

        public override void OnActivateCast(IPlayer activator)
        {
            if (TryOpenHalonDiscovery(activator) || DeadGone.TryOpen(activator, this) || ArcanusSilo.TryOpen(activator, this))
                return;

            uint progress = (uint)(1 << QuestChecklistIdx);

            if (CreatureEntry.DatacubeId != 0u)
            {
                IDatacube datacube = activator.DatacubeManager.GetDatacube((ushort)CreatureEntry.DatacubeId, DatacubeType.Datacube);
                if (datacube == null)
                    activator.DatacubeManager.AddDatacube((ushort)CreatureEntry.DatacubeId, progress);
                else
                {
                    datacube.Progress |= progress;
                    activator.DatacubeManager.SendDatacube(datacube);
                }
            }

            if (CreatureEntry.DatacubeVolumeId != 0u)
            {
                IDatacube datacube = activator.DatacubeManager.GetDatacube((ushort)CreatureEntry.DatacubeVolumeId, DatacubeType.Journal);
                if (datacube == null)
                    activator.DatacubeManager.AddDatacubeVolume((ushort)CreatureEntry.DatacubeVolumeId, progress);
                else
                {
                    datacube.Progress |= progress;
                    activator.DatacubeManager.SendDatacubeVolume(datacube);
                }
            }

            //TODO: cast "116,Generic Quest Spell - Activating - Activate - Tier 1" by 0x07FD
        }

        private bool TryOpenHalonDiscovery(IPlayer activator)
        {
            if (Map?.Entry.Id != 1068 || CreatureId is not (68052u or 67889u))
                return false;

            ushort questId = CreatureId == 68052u ? (ushort)6278 : (ushort)9770;
            if (EntityId > 0u && activator is { InWorld: true, IsLoading: false, IsAlive: true } &&
                ReferenceEquals(Map, activator.Map) && Vector3.DistanceSquared(Position, activator.Position) <= 25f &&
                GlobalQuestManager.Instance.GetQuestInfo(questId) is { } quest && activator.Level >= quest.Entry.PrerequisiteLevel &&
                activator.QuestManager.GetQuestState(questId) is null or QuestState.Unknown or QuestState.Abandoned or QuestState.Achieved)
                activator.Session.EnqueueMessageEncrypted(new ServerDialogStart { DialogUnitId = Guid });

            return true;
        }
    }
}
