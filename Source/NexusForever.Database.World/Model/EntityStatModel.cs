namespace NexusForever.Database.World.Model
{
    public class EntityStatModel
    {
        public uint Id { get; set; }
        public byte Stat { get; set; }
        public float Value { get; set; }

        public EntityModel Entity { get; set; }
    }
}
