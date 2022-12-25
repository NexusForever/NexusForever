namespace NexusForever.Database.Character.Model
{
    public class ItemRuneModel
    {
        public ulong Id { get; set; }
        public uint Index { get; set; }
        public byte RuneType { get; set; }
        public uint? RuneItemId { get; set; }

        public ItemModel Item { get; set; }
    }
}
