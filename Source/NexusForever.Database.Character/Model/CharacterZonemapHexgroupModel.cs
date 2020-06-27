namespace NexusForever.Database.Character.Model
{
    public class CharacterZonemapHexgroupModel
    {
        public ulong Id { get; set; }
        public ushort ZoneMap { get; set; }
        public ushort HexGroup { get; set; }

        public virtual CharacterModel Character { get; set; }
    }
}
