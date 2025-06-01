using APIIdentity = NexusForever.API.Model.Identity;
using APIIdentityName = NexusForever.API.Model.IdentityName;
using InternalIdentity = NexusForever.Network.Internal.Message.Shared.Identity;
using InternalIdentityName = NexusForever.Network.Internal.Message.Shared.IdentityName;

namespace NexusForever.Server.GroupServer
{
    public static class IdentityMappingExtensions
    {
        public static Identity ToGroupIdentity(this InternalIdentity identity)
        {
            return new Identity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }

        public static IdentityName ToGroupIdentity(this InternalIdentityName identity)
        {
            return new IdentityName
            {
                Name      = identity.Name,
                RealmName = identity.RealmName,
            };
        }

        public static InternalIdentity ToInternalIdentity(this Identity identity)
        {
            return new InternalIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }

        public static InternalIdentityName ToInternalIdentity(this IdentityName identity)
        {
            return new InternalIdentityName
            {
                Name      = identity.Name,
                RealmName = identity.RealmName,
            };
        }

        public static APIIdentity ToAPIIdentity(this Identity identity)
        {
            return new APIIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }

        public static APIIdentityName ToAPIIdentityName(this IdentityName identity)
        {
            return new APIIdentityName
            {
                Name      = identity.Name,
                RealmName = identity.RealmName,
            };
        }
    }
}
