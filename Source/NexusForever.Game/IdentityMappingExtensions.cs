using NexusForever.Game.Abstract;
using InternalIdentity = NexusForever.Network.Internal.Message.Shared.Identity;
using NetworkIdentity = NexusForever.Network.World.Message.Model.Shared.Identity;

namespace NexusForever.Game
{
    public static class IdentityMappingExtensions
    {
        public static NetworkIdentity ToNetworkIdentity(this Identity identity)
        {
            return new NetworkIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }

        public static Identity ToGameIdentity(this NetworkIdentity identity)
        {
            return new Identity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
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

        public static Identity ToGameIdentity(this InternalIdentity identity)
        {
            return new Identity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }
    }
}
