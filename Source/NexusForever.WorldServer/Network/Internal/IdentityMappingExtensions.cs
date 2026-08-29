using InternalIdentity = NexusForever.Network.Internal.Message.Shared.Identity;
using NetworkIdentity = NexusForever.Network.World.Message.Model.Shared.Identity;

namespace NexusForever.WorldServer.Network.Internal
{
    public static class IdentityMappingExtensions
    {
        public static NetworkIdentity ToNetworkIdentity(this InternalIdentity identity)
        {
            return new NetworkIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }

        public static InternalIdentity ToInternalIdentity(this NetworkIdentity identity)
        {
            return new InternalIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }
    }
}
