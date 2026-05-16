using Microsoft.Extensions.Hosting;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Player;
using Rebus.Bus;

namespace NexusForever.Server.Friendship.Network.Internal.Handler
{
    public class NetworkInternalHandlerHostedService : IHostedService
    {
        private readonly IBus _bus;

        public NetworkInternalHandlerHostedService(
            IBus bus)
        {
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.Subscribe<FriendshipAccountEmailInviteRequestMessage>();
            await _bus.Subscribe<FriendshipAccountFriendInviteRequestMessage>();
            await _bus.Subscribe<FriendshipAccountInviteResponseMessage>();
            await _bus.Subscribe<FriendshipAccountInviteMarkSeenMessage>();
            await _bus.Subscribe<FriendshipAccountNicknameUpdateMessage>();
            await _bus.Subscribe<FriendshipAccountNoteUpdateMessage>();
            await _bus.Subscribe<FriendshipAccountPresenceUpdateMessage>();
            await _bus.Subscribe<FriendshipAccountRemoveMessage>();
            await _bus.Subscribe<FriendshipAccountStatusUpdateMessage>();
            await _bus.Subscribe<FriendshipInviteMarkSeenMessage>();
            await _bus.Subscribe<FriendshipNameInviteRequestMessage>();
            await _bus.Subscribe<FriendshipInviteResponseMessage>();
            await _bus.Subscribe<FriendshipLocationRequestMessage>();
            await _bus.Subscribe<FriendshipNoteUpdateByIdentityMessage>();
            await _bus.Subscribe<FriendshipRemoveIdentityMessage>();

            await _bus.Subscribe<PlayerLoggedInMessage>();
            await _bus.Subscribe<PlayerLoggedOutMessage>();
            await _bus.Subscribe<PlayerStatUpdatedMessage>();
            await _bus.Subscribe<PlayerWorldUpdatedMessage>();
            await _bus.Subscribe<PlayerWorldZoneUpdatedMessage>();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
