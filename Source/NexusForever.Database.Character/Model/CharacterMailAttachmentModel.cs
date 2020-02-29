namespace NexusForever.Database.Character.Model
{
    public class CharacterMailAttachmentModel
    {
        public ulong Id { get; set; }
        public uint Index { get; set; }
        public ulong ItemGuid { get; set; }

        public CharacterMailModel Mail { get; set; }
        public ItemModel Item { get; set; }
    }
}
