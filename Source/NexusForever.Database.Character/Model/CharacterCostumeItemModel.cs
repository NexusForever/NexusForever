namespace NexusForever.Database.Character.Model
{
    public class CharacterCostumeItemModel
    {
        public ulong Id { get; set; }
        public byte Index { get; set; }
        public byte Slot { get; set; }
        public uint ItemId { get; set; }
        public int DyeData { get; set; }

        public virtual CharacterCostumeModel Costume { get; set; }
    }
}
