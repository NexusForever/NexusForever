using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Game.Character
{
    public class Character : ICharacter
    {
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
            CharacterId = model.Id;
            Name        = model.Name;
            Sex         = (Sex)model.Sex;
            Race        = (Race)model.Race;
            Class       = (Class)model.Class;
            Path        = (Path)model.ActivePath;
            Level       = (uint)model.Stat.SingleOrDefault(e => e.Stat == 10).Value;
            Faction1    = (Faction)model.FactionId;
            Faction2    = (Faction)model.FactionId;
            LastOnline  = model.LastOnline;
        }

        public Character(IPlayer player)
        {
            CharacterId = player.CharacterId;
            Name        = player.Name;
            Sex         = player.Sex;
            Race        = player.Race;
            Class       = player.Class;
            Path        = player.Path;
            Level       = player.Level;
            Faction1    = player.Faction1;
            Faction2    = player.Faction2;
            LastOnline  = DateTime.UtcNow;
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
