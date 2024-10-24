using NexusForever.Game.Static.Entity;

namespace NexusForever.Database.World.Model
{
    public class EntityPropertyModel
    {
        public uint Id { get; set; }
        public Property Property { get; set; }
        public float Value { get; set; }

        public EntityModel Entity { get; set; }
    }
}
