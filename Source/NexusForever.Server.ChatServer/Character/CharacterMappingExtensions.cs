using NexusForever.Database.Chat.Model;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Group.Shared;
using InternalCharacter = NexusForever.Network.Internal.Message.Chat.Shared.ChatCharacter;

namespace NexusForever.Server.ChatServer.Character
{
    public static class CharacterMappingExtensions
    {
        public static CharacterModel ToDatabaseCharacter(this API.Model.Character.Character character)
        {
            return new CharacterModel
            {
                CharacterId = character.Identity.Id,
                RealmId     = character.Identity.RealmId,
                Name        = character.IdentityName.Name,
                RealmName   = character.IdentityName.RealmName,
                Faction     = character.Faction,
                IsOnline    = character.IsOnline
            };
        }

        public static CharacterModel ToDatabaseCharacter(this GroupMember groupMember)
        {
            return new CharacterModel
            {
                CharacterId = groupMember.Identity.Id,
                RealmId     = groupMember.Identity.RealmId,
                Name        = groupMember.Character.Name,
                RealmName   = groupMember.Character.RealmName,
                Faction     = groupMember.Character.Faction,
                IsOnline    = (groupMember.Flags & GroupMemberInfoFlags.Disconnected) == 0
            };
        }

        public static InternalCharacter ToInternalCharacter(this Character character)
        {
            return new InternalCharacter
            {
                Identity     = character.Identity.ToInternalIdentity(),
                IdentityName = character.IdentityName.ToInternalIdentity(),
                Faction      = character.Faction,
                IsOnline     = character.IsOnline
            };
        }
    }
}
