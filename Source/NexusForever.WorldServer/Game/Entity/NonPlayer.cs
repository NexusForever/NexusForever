using System.Numerics;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class NonPlayer : WorldEntity
    {
        public uint CreatureId { get; }
        public VendorInfo VendorInfo { get; }

        public NonPlayer(Database.World.Model.Entity entity)
            : base(EntityType.NonPlayer)
        {
            CreatureId  = entity.Creature;
            DisplayInfo = entity.DisplayInfo;
            OutfitInfo  = entity.OutfitInfo;
            Faction1    = (Faction)entity.Faction1;
            Faction2    = (Faction)entity.Faction2;
            Rotation    = new Vector3(entity.Rx, entity.Ry, entity.Rz);

            if (EntityManager.VendorInfo.TryGetValue(entity.Id, out VendorInfo vendorInfo))
                VendorInfo = vendorInfo;

            CalculateProperties();

            // temp shit
            Stats.Add(Stat.Health, new StatValue(Stat.Health, 800));
            Stats.Add(Stat.Level, new StatValue(Stat.Level, 1));
            Stats.Add((Stat)15, new StatValue((Stat)15, 1));
            Stats.Add((Stat)20, new StatValue((Stat)20, 1));
            Stats.Add((Stat)21, new StatValue((Stat)21, 1));
            Stats.Add((Stat)22, new StatValue((Stat)22, 1));
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new NonPlayerEntityModel
            {
                CreatureId = CreatureId,
                Unknown0 = 0
            };
        }

        public override ServerEntityCreate BuildCreatePacket()
        {
            ServerEntityCreate entityCreate = base.BuildCreatePacket();
            entityCreate.Unknown60 = (byte)(VendorInfo != null ? 4 : 0); // show vendor icon above entity
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
