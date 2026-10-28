namespace NexusForever.Database.Character.Model
{
    public class CharacterEldanAugmentationUnlockModel
    {
        public ulong Id { get; set; }
        public ushort EldanAugmentationId { get; set; }

        public virtual CharacterModel Character { get; set; }
    }
}
