using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Combat;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Script;

namespace NexusForever.Game.Entity
{
    [DatabaseEntity(EntityType.NonPlayer)]
    public class NonPlayer : UnitEntity, INonPlayer
    {
        public IVendorInfo VendorInfo { get; private set; }

        public NonPlayer()
            : base(EntityType.NonPlayer)
        {
        }

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);
            scriptCollection = ScriptManager.Instance.InitialiseEntityScripts<INonPlayer>(this);

            if (model.EntityVendor != null)
            {
                CreateFlags |= EntityCreateFlag.Vendor;
                VendorInfo = new VendorInfo(model);
            }
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new NonPlayerEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = 0
            };
        }

        protected override float CalculateDefaultProperty(Property property)
        {
            float value = base.CalculateDefaultProperty(property);

            Creature2Entry creatureEntry = GameTableManager.Instance.Creature2.GetEntry(CreatureId);

            Creature2ArcheTypeEntry archeTypeEntry = GameTableManager.Instance.Creature2ArcheType.GetEntry(creatureEntry.Creature2ArcheTypeId);
            if (archeTypeEntry != null)
                value *= archeTypeEntry.UnitPropertyMultiplier[(uint)property];

            Creature2DifficultyEntry difficultyEntry = GameTableManager.Instance.Creature2Difficulty.GetEntry(creatureEntry.Creature2DifficultyId);
            if (difficultyEntry != null)
                value *= difficultyEntry.UnitPropertyMultiplier[(uint)property];

            Creature2TierEntry tierEntry = GameTableManager.Instance.Creature2Tier.GetEntry(creatureEntry.Creature2TierId);
            if (tierEntry != null)
                value *= tierEntry.UnitPropertyMultiplier[(uint)property];

            return value;
        }

        public override void SelectTarget(IEnumerable<IHostileEntity> hostiles = null)
        {
            base.SelectTarget(hostiles);

            hostiles ??= ThreatManager.GetThreatList();

            if (hostiles.Count() == 0)
            {
                SetTarget(0u);
                return;
            }

            if (currentTargetUnitId != hostiles.First().HatedUnitId)
                SetTarget(hostiles.First().HatedUnitId, hostiles.First().Threat);
        }
    }
}
