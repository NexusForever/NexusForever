using NexusForever.Database.World.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.Simple)]
    public class Simple : UnitEntity
    {
        public byte QuestChecklistIdx { get; private set; }

        public Simple()
            : base(EntityType.Simple)
        {
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

        public override void OnActivate(Player activator)
        {
            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(CreatureId);
            if (entry.DatacubeId != 0u)
                activator.DatacubeManager.AddDatacube((ushort)entry.DatacubeId, int.MaxValue);
        }

        public override void OnActivateCast(Player activator)
        {
            uint progress = (uint)(1 << QuestChecklistIdx);

            Creature2Entry entry = GameTableManager.Instance.Creature2.GetEntry(CreatureId);
            if (entry.DatacubeId != 0u)
            {
                Datacube datacube = activator.DatacubeManager.GetDatacube((ushort)entry.DatacubeId, DatacubeType.Datacube);
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
                Datacube datacube = activator.DatacubeManager.GetDatacube((ushort)entry.DatacubeVolumeId, DatacubeType.Journal);
                if (datacube == null)
                    activator.DatacubeManager.AddDatacubeVolume((ushort)entry.DatacubeVolumeId, progress);
                else
                {
                    datacube.Progress |= progress;
                    activator.DatacubeManager.SendDatacubeVolume(datacube);
                }
            }

            //TODO: cast "116,Generic Quest Spell - Activating - Activate - Tier 1" by 0x07FD
        }
    }
}
