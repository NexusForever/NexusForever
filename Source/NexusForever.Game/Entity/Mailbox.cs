using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    [DatabaseEntity(EntityType.MailBox)]
    public class Mailbox : WorldEntity, IMailbox
    {
        public Mailbox()
            : base(EntityType.MailBox)
        {
        }

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
