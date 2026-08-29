using Microsoft.Extensions.Hosting;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Guild;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Network.Internal.Message.Server;
using Rebus.Bus;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler
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
            await _bus.Subscribe<ChatChannelJoinMessage>();
            await _bus.Subscribe<ChatChannelMemberKickMessage>();
            await _bus.Subscribe<ChatChannelMemberLeaveMessage>();
            await _bus.Subscribe<ChatChannelMemberModeratorMessage>();
            await _bus.Subscribe<ChatChannelMemberMuteMessage>();
            await _bus.Subscribe<ChatChannelMemberOwnerMessage>();
            await _bus.Subscribe<ChatChannelMembersRequestMessage>();
            await _bus.Subscribe<ChatChannelPasswordMessage>();
            await _bus.Subscribe<ChatChannelTextRequestMessage>();
            await _bus.Subscribe<ChatWhisperRequestMessage>();

            await _bus.Subscribe<GroupDisbandedMessage>();
            await _bus.Subscribe<GroupMemberAddedMessage>();
            await _bus.Subscribe<GroupMemberRemovedMessage>();

            await _bus.Subscribe<GuildCreatedMessage>();
            await _bus.Subscribe<GuildMemberAddedMessage>();
            await _bus.Subscribe<GuildMemberRankUpdatedMessage>();
            await _bus.Subscribe<GuildMemberRemovedMessage>();

            await _bus.Subscribe<PlayerLoggedInMessage>();
            await _bus.Subscribe<PlayerLoggedOutMessage>();

            await _bus.Subscribe<ServerWorldOfflineMessage>();
            await _bus.Subscribe<ServerWorldOnlineMessage>();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
