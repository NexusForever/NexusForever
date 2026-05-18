namespace NexusForever.Database.World.Model
{
    public class EntityEventModel
    {
        public uint Id { get; set; }
        public uint EventId { get; set; }
        public uint Phase { get; set; }

        public EntityModel Entity { get; set; }
    }
}
