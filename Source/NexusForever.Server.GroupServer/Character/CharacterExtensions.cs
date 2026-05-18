using NexusForever.Database.Group.Model;

namespace NexusForever.Server.GroupServer.Character
{
    public static class CharacterExtensions
    {
        public static CharacterModel ToDatabaseCharacter(this API.Model.Character.Character character)
        {
            return new CharacterModel
            {
                CharacterId  = character.Identity.Id,
                RealmId      = character.Identity.RealmId,
                RealmName    = character.IdentityName.RealmName,
                Name         = character.IdentityName.Name,
                Sex          = character.Sex,
                Race         = character.Race,
                Class        = character.Class,
                Path         = character.Path,
                Faction      = character.Faction,
                CurrentRealm = character.RealmId,
                MapId        = character.WorldId,
                WorldZoneId  = character.WorldZoneId,
                PositionX    = character.Position.X,
                PositionY    = character.Position.Y,
                PositionZ    = character.Position.Z,
                Stats        = character.Stats.Select(s => new CharacterStatModel
                {
                    Stat  = s.Stat,
                    Value = s.Value
                }).ToList(),
            };
        }
    }
}
