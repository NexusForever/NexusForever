using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class MailboxEntity : WorldEntity, IMailboxEntity
    {
        public override EntityType Type => EntityType.MailBox;

        #region Dependency Injection

        public MailboxEntity(IMovementManager movementManager)
            : base(movementManager)
        {
        }

        #endregion

        protected override IEntityModel BuildEntityModel()
        {
            return new MailboxEntityModel
            {
                CreatureId = CreatureId
            };
        }

        public override ServerEntityCreate BuildCreatePacket(bool initialCommands)
        {
            ServerEntityCreate entityCreate = base.BuildCreatePacket(initialCommands);
            entityCreate.CreateFlags = 0;
            return entityCreate;
        }
    }
}
