namespace NexusForever.Database.World.Model
{
    public class EntityEmoteModel
    {
        public uint Id { get; set; }
        public ushort EmoteId { get; set; }

        public EntityModel Entity { get; set; }
    }
}
