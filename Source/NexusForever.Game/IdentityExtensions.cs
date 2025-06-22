using NexusForever.Game.Abstract;
using NetworkIdentity = NexusForever.Network.World.Message.Model.Shared.Identity;

namespace NexusForever.Game
{
    public static class IdentityExtensions
    {
        public static NetworkIdentity ToNetwork(this IIdentity identity)
        {
            return new NetworkIdentity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }

        public static IIdentity ToGame(this NetworkIdentity identity)
        {
            return new Identity
            {
                Id      = identity.Id,
                RealmId = identity.RealmId,
            };
        }
    }
}
