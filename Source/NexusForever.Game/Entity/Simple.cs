using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Script;
using System.Numerics;

namespace NexusForever.Game.Entity
{
    [DatabaseEntity(EntityType.Simple)]
    public class Simple : UnitEntity, ISimple
    {
        public byte QuestChecklistIdx { get; private set; }
        public Action<ISimple> afterAddToMap;

        public Simple()
            : base(EntityType.Simple)
        {
        }

        public Simple(uint creatureId, Action<ISimple> actionAfterAddToMap = null)
            : base(EntityType.Simple)
        {
            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(creatureId);
            if (entry == null)
                throw new ArgumentNullException();

            Initialise(creatureId, 0, 0); // TODO: Get display info from TBL or optional override params
            afterAddToMap = actionAfterAddToMap;

            SetProperty(Property.BaseHealth, 101.0f);

            SetStat(Stat.Health, 101u);
            SetStat(Stat.Level, 1u);

            Creature2DisplayGroupEntryEntry displayGroupEntry = GameTableManager.Instance.
                Creature2DisplayGroupEntry.
                Entries.
                FirstOrDefault(d => d.Creature2DisplayGroupId == entry.Creature2DisplayGroupId);
            if (displayGroupEntry != null)
                DisplayInfo = displayGroupEntry.Creature2DisplayInfoId;
        }

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
            if (CreatureEntry.DatacubeId != 0u)
                activator.DatacubeManager.AddDatacube((ushort)CreatureEntry.DatacubeId, int.MaxValue);
        }

        public override void OnActivateSuccess(IPlayer activator)
        {
            uint progress = (uint)(1 << QuestChecklistIdx);

            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(CreatureId);
            if (entry.DatacubeId != 0u)
            {
                IDatacube datacube = activator.DatacubeManager.GetDatacube((ushort)entry.DatacubeId, DatacubeType.Datacube);
                if (datacube == null)
                    activator.DatacubeManager.AddDatacube((ushort)entry.DatacubeId, progress);
                else
                {
                    datacube.Progress |= progress;
                    activator.DatacubeManager.SendDatacube(datacube);
                }
            }

            if (entry.DatacubeVolumeId != 0u)
            {
                IDatacube datacube = activator.DatacubeManager.GetDatacube((ushort)entry.DatacubeVolumeId, DatacubeType.Journal);
                if (datacube == null)
                    activator.DatacubeManager.AddDatacubeVolume((ushort)entry.DatacubeVolumeId, progress);
                else
                {
                    datacube.Progress |= progress;
                    activator.DatacubeManager.SendDatacubeVolume(datacube);
                }
            }

            activator.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, CreatureId, 1u);
            activator.QuestManager.ObjectiveUpdate(QuestObjectiveType.SucceedCSI, CreatureId, 1u);
            activator.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateTargetGroupChecklist, CreatureId, QuestChecklistIdx);
        }

        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);

            afterAddToMap?.Invoke(this);
        }
    }
}
