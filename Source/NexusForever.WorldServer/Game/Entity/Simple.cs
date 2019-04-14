using System.Numerics;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using EntityModel = NexusForever.WorldServer.Database.World.Model.Entity;

namespace NexusForever.WorldServer.Game.Entity
{
    [DatabaseEntity(EntityType.Simple)]
    public class Simple : WorldEntity
    {
        public uint CreatureId { get; private set; }

        public Simple()
            : base(EntityType.Simple)
        {
        }

        public override void Initialise(EntityModel model)
        {
            CreatureId = model.Creature;
            Rotation   = new Vector3(model.Rx, model.Ry, model.Rz);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new SimpleEntityModel
            {
                CreatureId = CreatureId
            };
        }
    }
}
