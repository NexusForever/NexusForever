using NexusForever.Database.Friendship.Model;
using InternalCharacter = NexusForever.Network.Internal.Message.Friendship.Shared.Character;

namespace NexusForever.Server.Friendship.Game.Character
{
    public static class CharacterMappingExtensions
    {
        /// <summary>
        /// Convert an <see cref="Character"/> to an internal message model.
        /// </summary>
        /// <param name="character">The <see cref="Character"/> to convert to the internal message model.</param>
        /// <returns>The internal message model of the <see cref="Character"/>.</returns>
        public static InternalCharacter ToInternalCharacter(this Character character)
        {
            return new InternalCharacter
            {
                Identity     = character.Identity.ToInternalIdentity(),
                IdentityName = character.IdentityName.ToInternalIdentity(),
                Race         = character.Race,
                Class        = character.Class,
                Path         = character.Path,
                Faction      = character.Faction,
                Level        = character.Level,
                WorldZoneId  = character.WorldZoneId,
                WorldId      = character.WorldId,
                LastOnline   = character.LastOnline
            };
        }

        public static CharacterModel ToDatabaseCharacter(this API.Model.Character.Character character)
        {
            return new CharacterModel
            {
                CharacterId  = character.Identity.Id,
                RealmId      = character.Identity.RealmId,
                RealmName    = character.IdentityName.RealmName,
                Name         = character.IdentityName.Name,
                AccountId    = character.AccountId,
                Race         = character.Race,
                Class        = character.Class,
                Faction      = character.Faction,
                WorldZoneId  = character.WorldZoneId,
                WorldId      = character.WorldId,
                LastOnline   = !character.IsOnline ? character.LastOnline : null,
                Stats        = character.Stats.ConvertAll(s => new CharacterStatModel
                {
                    Stat  = s.Stat,
                    Value = s.Value
                })
            };
        }
    }
}
