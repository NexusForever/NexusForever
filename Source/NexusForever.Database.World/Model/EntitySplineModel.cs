namespace NexusForever.Database.World.Model
{
    public class EntitySplineModel
    {
        public uint Id { get; set; }
        public ushort SplineId { get; set; }
        public byte Mode { get; set; }
        public float Speed { get; set; }
        public float Fx { get; set; }
        public float Fy { get; set; }
        public float Fz { get; set; }

        public EntityModel Entity { get; set; }
    }
}
