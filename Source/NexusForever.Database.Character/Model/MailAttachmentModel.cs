namespace NexusForever.Database.Character.Model
{
    public class MailAttachmentModel
    {
        public ulong Id { get; set; }
        public uint Index { get; set; }
        public ulong ItemGuid { get; set; }

        public MailModel Mail { get; set; }
        public ItemModel Item { get; set; }
    }
}
