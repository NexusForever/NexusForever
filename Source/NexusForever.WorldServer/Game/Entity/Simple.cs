using System.Numerics;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Simple : UnitEntity
    {
        public uint CreatureId { get; }
        public byte QuestChecklistIdx { get; }

        public Simple(Database.World.Model.Entity entity)
            : base(EntityType.Simple)
        {
            CreatureId  = entity.Creature;
            DisplayInfo = entity.DisplayInfo;
            OutfitInfo  = entity.OutfitInfo;
            Faction1    = (Faction)entity.Faction1;
            Faction2    = (Faction)entity.Faction2;
            Rotation    = new Vector3(entity.Rx, entity.Ry, entity.Rz);
            QuestChecklistIdx = entity.QuestChecklistIdx;

            SetStat(Stat.Health, 101u);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new SimpleEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = QuestChecklistIdx
            };
        }
    }
}
