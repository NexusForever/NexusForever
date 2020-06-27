namespace NexusForever.Database.Character.Model
{
    public class CharacterActionSetShortcutModel
    {
        public ulong Id { get; set; }
        public byte SpecIndex { get; set; }
        public ushort Location { get; set; }
        public byte ShortcutType { get; set; }
        public uint ObjectId { get; set; }
        public byte Tier { get; set; }

        public CharacterModel Character { get; set; }
    }
}
