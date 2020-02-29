using NexusForever.Database.World.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.NonPlayer)]
    public class NonPlayer : UnitEntity
    {
        public VendorInfo VendorInfo { get; private set; }

        public NonPlayer()
            : base(EntityType.NonPlayer)
        {
        }

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);

            if (model.EntityVendor != null)
            {
                CreateFlags |= EntityCreateFlag.Vendor;
                VendorInfo = new VendorInfo(model);
            }

            CalculateProperties();
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new NonPlayerEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = 0
            };
        }

        private void CalculateProperties()
        {
            Creature2Entry creatureEntry = GameTableManager.Instance.Creature2.GetEntry(CreatureId);

            // TODO: research this some more
            /*float[] values = new float[200];

            CreatureLevelEntry levelEntry = GameTableManager.Instance.CreatureLevel.GetEntry(6);
            for (uint i = 0u; i < levelEntry.UnitPropertyValue.Length; i++)
                values[i] = levelEntry.UnitPropertyValue[i];

            Creature2ArcheTypeEntry archeTypeEntry = GameTableManager.Instance.Creature2ArcheType.GetEntry(creatureEntry.Creature2ArcheTypeId);
            for (uint i = 0u; i < archeTypeEntry.UnitPropertyMultiplier.Length; i++)
                values[i] *= archeTypeEntry.UnitPropertyMultiplier[i];

            Creature2DifficultyEntry difficultyEntry = GameTableManager.Instance.Creature2Difficulty.GetEntry(creatureEntry.Creature2DifficultyId);
            for (uint i = 0u; i < difficultyEntry.UnitPropertyMultiplier.Length; i++)
                values[i] *= archeTypeEntry.UnitPropertyMultiplier[i];

            Creature2TierEntry tierEntry = GameTableManager.Instance.Creature2Tier.GetEntry(creatureEntry.Creature2TierId);
            for (uint i = 0u; i < tierEntry.UnitPropertyMultiplier.Length; i++)
                values[i] *= archeTypeEntry.UnitPropertyMultiplier[i];

            for (uint i = 0u; i < levelEntry.UnitPropertyValue.Length; i++)
                SetProperty((Property)i, values[i]);*/
        }
    }
}
