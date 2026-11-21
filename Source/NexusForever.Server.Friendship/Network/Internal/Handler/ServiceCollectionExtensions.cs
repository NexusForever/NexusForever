using Microsoft.Extensions.DependencyInjection;
using NexusForever.Server.Friendship.Network.Internal.Handler.Friendship;
using NexusForever.Server.Friendship.Network.Internal.Handler.Player;
using Rebus.Config;

namespace NexusForever.Server.Friendship.Network.Internal.Handler
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNetworkInternalHandlers(this IServiceCollection sc)
        {
            sc.AddRebusHandler<FriendshipAccountEmailInviteRequestHandler>();
            sc.AddRebusHandler<FriendshipAccountFriendInviteRequestHandler>();
            sc.AddRebusHandler<FriendshipAccountInviteResponseHandler>();
            sc.AddRebusHandler<FriendshipAccountInviteSeenHandler>();
            sc.AddRebusHandler<FriendshipAccountNicknameUpdateHandler>();
            sc.AddRebusHandler<FriendshipAccountNoteUpdateHandler>();
            sc.AddRebusHandler<FriendshipAccountPresenceUpdateHandler>();
            sc.AddRebusHandler<FriendshipAccountRemoveHandler>();
            sc.AddRebusHandler<FriendshipAccountStatusUpdateHandler>();
            sc.AddRebusHandler<FriendshipInviteMarkSeenHandler>();
            sc.AddRebusHandler<FriendshipNameInviteRequestHandler>();
            sc.AddRebusHandler<FriendshipInviteResponseHandler>();
            sc.AddRebusHandler<FriendshipLocationRequestHandler>();
            sc.AddRebusHandler<FriendshipNoteUpdateByIdentityHandler>();
            sc.AddRebusHandler<FriendshipRemoveIdentityHandler>();

            sc.AddRebusHandler<PlayerLoggedInHandler>();
            sc.AddRebusHandler<PlayerLoggedOutHandler>();
            sc.AddRebusHandler<PlayerStatUpdatedHandler>();
            sc.AddRebusHandler<PlayerWorldUpdatedHandler>();
            sc.AddRebusHandler<PlayerWorldZoneUpdatedHandler>();
        }
    }
}
