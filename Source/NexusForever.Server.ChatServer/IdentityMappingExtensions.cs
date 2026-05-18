using APIIdentity = NexusForever.API.Model.Identity;
using APIIdentityName = NexusForever.API.Model.IdentityName;
using InternalIdentity = NexusForever.Network.Internal.Message.Shared.Identity;
using InternalIdentityName = NexusForever.Network.Internal.Message.Shared.IdentityName;

namespace NexusForever.Server.ChatServer
{
    public static class IdentityMappingExtensions
    {
        public static Identity ToChatIdentity(this InternalIdentity identity)
        {
            return new Identity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId
            };
        }

        public static IdentityName ToChatIdentity(this InternalIdentityName identity)
        {
            return new IdentityName
            {
                Name      = identity.Name,
                RealmName = identity.RealmName
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

        public static InternalIdentityName ToInternalIdentity(this IdentityName identity)
        {
            return new InternalIdentityName
            {
                Name      = identity.Name,
                RealmName = identity.RealmName
            };
        }

        public static APIIdentity ToAPIdentity(this Identity identity)
        {
            return new APIIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }

        public static APIIdentityName ToAPIIdentity(this IdentityName identity)
        {
            return new APIIdentityName
            {
                Name      = identity.Name,
                RealmName = identity.RealmName
            };
        }
    }
}
