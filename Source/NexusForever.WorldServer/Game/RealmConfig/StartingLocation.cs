using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.RealmConfig
{
    public class StartingLocation
    {
        public byte Id { get; }
        public Race Race { get;}
        public Faction FactionId { get; }
        public uint LocationId { get; }
        public CharacterCreationStart CharacterCreationStart { get; }

        public StartingLocation(RealmConfigStartingLocation model)
        {
            Id                      = model.Id;
            Race                    = (Race)model.Race;
            FactionId               = (Faction)model.FactionId;
            LocationId              = model.LocationId;
            CharacterCreationStart  = (CharacterCreationStart)model.CharacterCreationStart;
        }
    }
}
