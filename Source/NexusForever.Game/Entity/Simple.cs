using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity;
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
            scriptCollection = ScriptManager.Instance.InitialiseEntityScripts<ISimple>(this);
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

            base.OnActivateSuccess(activator);
        }

        public override void OnActivateSuccess(IPlayer activator)
        {
            // TODO: Move below to Datacube Manager method
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

            base.OnActivateSuccess(activator);
        }

        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);

            afterAddToMap?.Invoke(this);
        }
    }
}
