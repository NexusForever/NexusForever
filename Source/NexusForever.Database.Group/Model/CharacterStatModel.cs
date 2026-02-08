using NexusForever.Game.Static.Entity;

namespace NexusForever.Database.Group.Model
{
    public class CharacterStatModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public Stat Stat { get; set; }
        public float Value { get; set; }

        public CharacterModel Character { get; set; }
    }
}
