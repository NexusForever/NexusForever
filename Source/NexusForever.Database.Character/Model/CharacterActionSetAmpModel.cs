namespace NexusForever.Database.Character.Model
{
    public class CharacterActionSetAmpModel
    {
        public ulong Id { get; set; }
        public byte SpecIndex { get; set; }
        public ushort AmpId { get; set; }

        public virtual CharacterModel Character { get; set; }
    }
}
