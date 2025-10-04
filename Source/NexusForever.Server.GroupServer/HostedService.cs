using Microsoft.Extensions.Hosting;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Match;
using NexusForever.Network.Internal.Message.Player;
using Rebus.Bus;

namespace NexusForever.Server.GroupServer
{
    public class HostedService : IHostedService
    {
        #region Dependency Injection

        private readonly IBus _bus;

        public HostedService(
            IBus bus)
        {
            _bus = bus;
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.Subscribe<GroupDisbandMessage>();
            await _bus.Subscribe<GroupFlagsUpdateMessage>();
            await _bus.Subscribe<GroupLootRulesUpdateMessage>();
            await _bus.Subscribe<GroupMarkerMessage>();
            await _bus.Subscribe<GroupMemberFlagUpdateMessage>();
            await _bus.Subscribe<GroupMemberKickMessage>();
            await _bus.Subscribe<GroupMemberLeaveMessage>();
            await _bus.Subscribe<GroupMemberPromoteMessage>();
            await _bus.Subscribe<GroupMemberRequestMessage>();
            await _bus.Subscribe<GroupMemberRequestReponseMessage>();
            await _bus.Subscribe<GroupPlayerInviteMessage>();
            await _bus.Subscribe<GroupPlayerInviteRespondedMessage>();
            await _bus.Subscribe<GroupReadyCheckMessage>();

            await _bus.Subscribe<MatchCreatedMessage>();
            await _bus.Subscribe<MatchMemberLeftMessage>();
            await _bus.Subscribe<MatchRemovedMessage>();

            await _bus.Subscribe<PlayerLoggedInMessage>();
            await _bus.Subscribe<PlayerLoggedOutMessage>();
            await _bus.Subscribe<PlayerPositionUpdatedMessage>();
            await _bus.Subscribe<PlayerPropertyUpdatedMessage>();
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
