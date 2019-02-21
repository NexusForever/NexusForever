using System.Numerics;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using EntityModel = NexusForever.WorldServer.Database.World.Model.Entity;
using NexusForever.WorldServer.Database.World.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.NonPlayer)]
    public class NonPlayer : UnitEntity, IDatabaseEntity
    {
        public uint CreatureId { get; private set; }
        public VendorInfo VendorInfo { get; private set; }

        public NonPlayer()
            : base(EntityType.NonPlayer)
        {
        }

        public void Initialise(EntityModel model)
        {
            CreatureId  = model.Creature;
            DisplayInfo = model.DisplayInfo;
            OutfitInfo  = model.OutfitInfo;
            Faction1    = (Faction)model.Faction1;
            Faction2    = (Faction)model.Faction2;
            Rotation    = new Vector3(model.Rx, model.Ry, model.Rz);

            if (EntityManager.VendorInfo.TryGetValue(model.Id, out VendorInfo vendorInfo))
                VendorInfo = vendorInfo;

            CalculateProperties();

            // FIXME: move to WorldEntity?
            foreach(EntityStat stat in model.EntityStat)
                    Stats.Add((Stat)stat.Stat, new StatValue((Stat)stat.Stat, stat.Value));
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new NonPlayerEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = 0
            };
        }

        public override ServerEntityCreate BuildCreatePacket()
        {
            ServerEntityCreate entityCreate = base.BuildCreatePacket();
            entityCreate.CreateFlags = (byte)(VendorInfo != null ? 4 : 0); // show vendor icon above entity
            return entityCreate;
        }

        private void CalculateProperties()
        {
            Creature2Entry creatureEntry = GameTableManager.Creature2.GetEntry(CreatureId);

            // TODO: research this some more
            /*float[] values = new float[200];

            CreatureLevelEntry levelEntry = GameTableManager.CreatureLevel.GetEntry(6);
            for (uint i = 0u; i < levelEntry.UnitPropertyValue.Length; i++)
                values[i] = levelEntry.UnitPropertyValue[i];

            Creature2ArcheTypeEntry archeTypeEntry = GameTableManager.Creature2ArcheType.GetEntry(creatureEntry.Creature2ArcheTypeId);
            for (uint i = 0u; i < archeTypeEntry.UnitPropertyMultiplier.Length; i++)
                values[i] *= archeTypeEntry.UnitPropertyMultiplier[i];

            Creature2DifficultyEntry difficultyEntry = GameTableManager.Creature2Difficulty.GetEntry(creatureEntry.Creature2DifficultyId);
            for (uint i = 0u; i < difficultyEntry.UnitPropertyMultiplier.Length; i++)
                values[i] *= archeTypeEntry.UnitPropertyMultiplier[i];

            Creature2TierEntry tierEntry = GameTableManager.Creature2Tier.GetEntry(creatureEntry.Creature2TierId);
            for (uint i = 0u; i < tierEntry.UnitPropertyMultiplier.Length; i++)
                values[i] *= archeTypeEntry.UnitPropertyMultiplier[i];

            for (uint i = 0u; i < levelEntry.UnitPropertyValue.Length; i++)
                SetProperty((Property)i, values[i]);*/
        }
    }
}
