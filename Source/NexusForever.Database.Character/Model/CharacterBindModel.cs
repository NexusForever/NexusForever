namespace NexusForever.Database.Character.Model
{
    public class CharacterBindModel
    {
        public ulong Id { get; set; }
        public ushort BindPointId { get; set; }
        public ushort WorldId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public CharacterModel Character { get; set; }
    }
}
