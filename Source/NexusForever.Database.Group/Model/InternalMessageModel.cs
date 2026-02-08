namespace NexusForever.Database.Group.Model
{
    public class InternalMessageModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}
