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
            Faction1    = entity.Faction1;
            Faction2    = entity.Faction2;
            Rotation    = new Vector3(entity.Rx, entity.Ry, entity.Rz);

            if (EntityManager.VendorInfo.TryGetValue(entity.Id, out VendorInfo vendorInfo))
                VendorInfo = vendorInfo;

            CalculateProperties();

            // temp shit
            Stats.Add((Stat)15, new StatValue((Stat)15, 2));
            //Stats.Add((Stat)20, new StatValue((Stat)20, 3));
            Stats.Add((Stat)21, new StatValue((Stat)21, 4));
            Stats.Add((Stat)22, new StatValue((Stat)22, 5));
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
            Creature2Entry creature2Entry = GameTableManager.Creature2.GetEntry(CreatureId);
            UnitRaceEntry unitRaceEntry = GameTableManager.UnitRace.GetEntry(creature2Entry.UnitRaceId);
            Creature2DifficultyEntry creature2DifficultyEntry = GameTableManager.Creature2Difficulty.GetEntry(creature2Entry.Creature2DifficultyId);
            Creature2ArcheTypeEntry creature2ArcheTypeEntry = GameTableManager.Creature2ArcheType.GetEntry(creature2Entry.Creature2ArcheTypeId);
            Creature2TierEntry creature2TierEntry = GameTableManager.Creature2Tier.GetEntry(creature2Entry.Creature2TierId);
            Creature2ModelInfoEntry creature2ModelInfoEntry = GameTableManager.Creature2ModelInfo.GetEntry(creature2Entry.Creature2ModelInfoId);
            Creature2DisplayGroupEntryEntry creature2DisplayGroupEntry = GameTableManager.Creature2DisplayGroupEntry.GetEntry(creature2Entry.Creature2DisplayGroupId);
            Creature2OutfitGroupEntryEntry creature2OutfitGroupEntry = GameTableManager.Creature2OutfitGroupEntry.GetEntry(creature2Entry.Creature2OutfitGroupId);
            Faction2Entry faction2Entry = GameTableManager.Faction2.GetEntry(creature2Entry.FactionId);

            int level = new System.Random().Next((int)creature2Entry.MinLevel, (int)creature2Entry.MaxLevel);
            CreatureLevelEntry creatureLevelEntry = GameTableManager.CreatureLevel.GetEntry((ulong)level);

            float values;

            for (uint i = 0u; i < creatureLevelEntry.UnitPropertyValue.Length; i++)
            {
                values = creatureLevelEntry.UnitPropertyValue[i] * creature2ArcheTypeEntry.UnitPropertyMultiplier[i] * creature2DifficultyEntry.UnitPropertyMultiplier[i] * creature2TierEntry.UnitPropertyMultiplier[i];
                SetProperty((Property)i, values);
            }
            Stats.Add(Stat.Level, new StatValue(Stat.Level, (uint)level));
            Stats.Add(Stat.Health, new StatValue(Stat.Health, (uint)GetPropertyValue(Property.BaseHealth)));
            Stats.Add(Stat.Shield, new StatValue(Stat.Shield, (uint)GetPropertyValue(Property.ShieldCapacityMax)));

            // FIXME not done, more Stat's to add
        }
    }
}
