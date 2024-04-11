using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Entity
{
    public class PlatformEntity : WorldEntity, IPlatformEntity
    {
        public override EntityType Type => EntityType.Platform;

        #region Dependency Injection

        public PlatformEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new PlatformEntityModel
            {
                CreatureId = CreatureId,
                QuestChecklistIdx = 0
            };
        }
    }
}
