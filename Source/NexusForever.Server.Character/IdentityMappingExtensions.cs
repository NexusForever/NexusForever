using APIIdentity = NexusForever.API.Model.Identity;
using APINameIdentity = NexusForever.API.Model.IdentityName;
using InternalIdentity = NexusForever.Network.Internal.Message.Shared.Identity;
using InternalNameIdentity = NexusForever.Network.Internal.Message.Shared.IdentityName;

namespace NexusForever.Server.Character
{
    public static class IdentityMappingExtensions
    {
        public static APIIdentity ToAPIdentity(this InternalIdentity identity)
        {
            return new APIIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId
            };
        }

        public static InternalNameIdentity ToInternalIdentity(this APINameIdentity identity)
        {
            return new InternalNameIdentity
            {
                Name      = identity.Name,
                RealmName = identity.RealmName
            };
        }
    }
}
