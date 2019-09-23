using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;
using System.Numerics;

namespace NexusForever.WorldServer.Game.Entity
{
    public class SimpleCollidable : WorldEntity
    {
        public byte QuestChecklistIdx { get; private set; }

        public SimpleCollidable(uint creatureId, byte questChecklistIdx = 255)
            : base(EntityType.SimpleCollidable)
        {
            CreatureId = creatureId;
            QuestChecklistIdx = questChecklistIdx;
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new SimpleCollidableEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = QuestChecklistIdx
            };
        }

        public override void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);

            EnqueueToVisible(new Server08B3
            {
                MountGuid = Guid,
                Unknown0 = 0,
                Unknown1 = true
            }, true);
        }
    }
}
