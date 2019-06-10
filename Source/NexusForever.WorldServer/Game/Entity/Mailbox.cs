using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.MailBox)]
    public class Mailbox : WorldEntity
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

        public override ServerEntityCreate BuildCreatePacket()
        {
            ServerEntityCreate entityCreate = base.BuildCreatePacket();
            entityCreate.CreateFlags = 0;
            return entityCreate;
        }
    }
}
