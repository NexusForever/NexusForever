using NexusForever.Database.Query.Model;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Internal.Message.Who;

namespace NexusForever.Server.Character.Game.Character
{
    public static class CharacterMappingExtensions
    {
        public static CharacterModel ToDatabaseCharacter(this API.Model.Character.Character character)
        {
            return new CharacterModel
            {
                CharacterId    = character.Identity.Id,
                RealmId        = character.RealmId,
                Name           = character.IdentityName.Name,
                RealmName      = character.IdentityName.RealmName,
                Race           = character.Race,
                Class          = character.Class,
                Path           = character.Path,
                Faction        = character.Faction,
                Sex            = character.Sex,
                CurrentRealmId = character.RealmId,
                WorldZoneId    = character.WorldZoneId,
                Level          = (uint)(character.Stats.SingleOrDefault(s => s.Stat == Stat.Level)?.Value ?? 0u),
                LastOnline     = character.LastOnline
            };
        }

        public static WhoCharacter ToInternalCharacter(this Character character)
        {
            return new WhoCharacter
            {
                Identity     = character.Identity.ToInternalIdentity(),
                IdentityName = character.IdentityName.ToInternalIdentity(),
                Race         = character.Race,
                Class        = character.Class,
                Path         = character.Path,
                Faction      = character.Faction,
                Sex          = character.Sex,
                WorldZoneId  = character.WorldZoneId,
                Level        = character.Level
            };
        }
    }
}