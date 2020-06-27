namespace NexusForever.Database.Character.Model
{
    public class CharacterAppearanceModel
    {
        public ulong Id { get; set; }
        public byte Slot { get; set; }
        public ushort DisplayId { get; set; }

        public virtual CharacterModel Character { get; set; }
    }
}
