using System.Numerics;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using EntityModel = NexusForever.WorldServer.Database.World.Model.Entity;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.MailBox)]
    public class Mailbox : WorldEntity, IDatabaseEntity
    {
        public uint CreatureId { get; private set; }

        public Mailbox()
            : base(EntityType.MailBox)
        {
        }

        public void Initialise(EntityModel model)
        {
            CreatureId = model.Creature;
            DisplayInfo = model.DisplayInfo;
            OutfitInfo = model.OutfitInfo;
            Faction1 = (Faction)model.Faction1;
            Faction2 = (Faction)model.Faction2;
            Rotation = new Vector3(model.Rx, model.Ry, model.Rz);
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
