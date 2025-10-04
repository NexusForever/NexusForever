using InternalGroupCharacter = NexusForever.Network.Internal.Message.Group.Shared.GroupCharacter;
using NetworkGroupCharacter = NexusForever.Network.World.Message.Model.Shared.GroupCharacter;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public static class GroupCharacterMappingExtensions
    {
        public static NetworkGroupCharacter ToNetworkGroupCharacter(this InternalGroupCharacter character)
        {
            return new NetworkGroupCharacter
            {
                Name           = character.Name,
                Faction        = character.Faction,
                Race           = character.Race,
                Class          = character.Class,
                Sex            = character.Sex,
                Level          = character.Level,
                EffectiveLevel = character.EffectiveLevel,
                Path           = character.Path,
                Realm          = character.RealmId,
                WorldZoneId    = (ushort)character.WorldZoneId,
                MapId          = character.WorldId,
                SyncedToGroup  = true,
                PhaseId        = 1,
            };
        }
    }
}
