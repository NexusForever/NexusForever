using APIIdentity = NexusForever.API.Model.Identity;
using InternalIdentity = NexusForever.Network.Internal.Message.Shared.Identity;
using InternalNameIdentity = NexusForever.Network.Internal.Message.Shared.IdentityName;

namespace NexusForever.Server.Character
{
    public static class IdentityMappingExtensions
    {
        public static Identity ToQueryIdentity(this InternalIdentity identity)
        {
            return new Identity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId
            };
        }

        public static APIIdentity ToAPIIdentity(this Identity identity)
        {
            return new APIIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId
            };
        }

        public static InternalIdentity ToInternalIdentity(this Identity identity)
        {
            return new InternalIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId
            };
        }

        public static InternalNameIdentity ToInternalIdentity(this IdentityName identity)
        {
            return new InternalNameIdentity
            {
                Name      = identity.Name,
                RealmName = identity.RealmName
            };
        }
    }
}
