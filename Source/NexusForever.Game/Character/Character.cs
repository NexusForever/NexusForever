using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Game.Character
{
    public class Character : ICharacter
    {
        public uint AccountId { get; }
        public ulong CharacterId { get; }
        public string Name { get; }
        public Sex Sex { get; }
        public Race Race { get; }
        public Class Class { get; }
        public Path Path { get; }
        public uint Level { get; }
        public Faction Faction1 { get; }
        public Faction Faction2 { get; }
        public DateTime? LastOnline { get; }

        public Character(CharacterModel model)
        {
            AccountId   = model.AccountId;
            CharacterId = model.Id;
            Name        = model.Name;
            Sex         = (Sex)model.Sex;
            Race        = (Race)model.Race;
            Class       = (Class)model.Class;
            Path        = (Path)model.ActivePath;
            Level       = model.Level;
            Faction1    = (Faction)model.FactionId;
            Faction2    = (Faction)model.FactionId;
            LastOnline  = model.LastOnline;
        }

        /// <summary>
        /// Returns a <see cref="float"/> representing decimal value, in days, since the character was last online.
        /// </summary>
        public float? GetOnlineStatus()
        {
            if (!LastOnline.HasValue)
                return null;

            return (float)DateTime.UtcNow.Subtract(LastOnline.Value).TotalDays * -1f;
        }
    }
}
