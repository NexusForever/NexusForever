namespace NexusForever.Database.Character.Model
{
    public class CharacterPathModel
    {
        public ulong Id { get; set; }
        public byte Path { get; set; }
        public byte Unlocked { get; set; }
        public uint TotalXp { get; set; }
        public byte LevelRewarded { get; set; }

        public CharacterModel Character { get; set; }
    }
}
