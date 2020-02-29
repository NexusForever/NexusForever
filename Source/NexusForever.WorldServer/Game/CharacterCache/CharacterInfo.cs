using System;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.CharacterCache
{
    public class CharacterInfo : ICharacter
    {
        public ulong CharacterId { get; }
        public string Name { get; }
        public Sex Sex { get; }
        public Race Race { get;}
        public Class Class { get; }
        public Path Path { get; }
        public uint Level { get; }
        public Faction Faction1 { get; }
        public Faction Faction2 { get; }
        public DateTime LastOnline { get; } = DateTime.UtcNow;

        public CharacterInfo(CharacterModel model)
        {
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

        public CharacterInfo(ICharacter model)
        {
            CharacterId = model.CharacterId;
            Name        = model.Name;
            Sex         = model.Sex;
            Race        = model.Race;
            Class       = model.Class;
            Path        = model.Path;
            Level       = model.Level;
            Faction1    = model.Faction1;
            Faction2    = model.Faction1;
        }

        public float GetOnlineStatus()
        {
            return (float)DateTime.UtcNow.Subtract(LastOnline).TotalDays * -1f;
        }
    }
}
