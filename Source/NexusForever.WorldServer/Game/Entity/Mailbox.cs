using System.Numerics;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using EntityModel = NexusForever.WorldServer.Database.World.Model.Entity;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.MailBox)]
    public class Mailbox : WorldEntity
    {
        public uint CreatureId { get; private set; }

        public Mailbox()
            : base(EntityType.MailBox)
        {
        }

        public override void Initialise(EntityModel model)
        {
            base.Initialise(model);
            CreatureId = model.Creature;
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
