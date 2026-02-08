using NexusForever.Game.Static.Entity;

namespace NexusForever.Database.Group.Model
{
    public class CharacterPropertyModel
    {
        public ulong CharacterId { get; set; }
        public ushort RealmId { get; set; }
        public Property Property { get; set; }
        public float Value { get; set; }

        public CharacterModel Character { get; set; }
    }
}
