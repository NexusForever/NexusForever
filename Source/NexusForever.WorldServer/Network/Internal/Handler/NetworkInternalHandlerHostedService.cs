using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Network.Internal.Message.Who;
using Rebus.Bus;

namespace NexusForever.WorldServer.Network.Internal.Handler
{
    public class NetworkInternalHandlerHostedService : IHostedService
    {
        #region Dependency Injection

        private readonly IBus bus;

        public NetworkInternalHandlerHostedService(
            IBus bus)
        {
            this.bus = bus;
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await bus.Subscribe<ChatChannelActionMessage>();
            await bus.Subscribe<ChatChannelJoinResultMessage>();
            await bus.Subscribe<ChatChannelMemberAddedMessage>();
            await bus.Subscribe<ChatChannelMembersMessage>();
            await bus.Subscribe<ChatChannelResultMessage>();
            await bus.Subscribe<ChatTextAcceptedMessage>();
            await bus.Subscribe<ChatChannelTextMessage>();
            await bus.Subscribe<ChatChannelTextResultMessage>();
            await bus.Subscribe<ChatWhisperFailedMessage>();
            await bus.Subscribe<ChatWhisperTextMessage>();

            await bus.Subscribe<FriendshipAccountInviteListMessage>();
            await bus.Subscribe<FriendshipAccountInviteRemovedMessage>();
            await bus.Subscribe<FriendshipAccountLastOnlineUpdatedMessage>();
            await bus.Subscribe<FriendshipAccountLevelUpdatedMessage>();
            await bus.Subscribe<FriendshipAccountListMessage>();
            await bus.Subscribe<FriendshipAccountLocationsUpdatedMessage>();
            await bus.Subscribe<FriendshipAccountNicknameUpdatedMessage>();
            await bus.Subscribe<FriendshipAccountNoteUpdatedMessage>();
            await bus.Subscribe<FriendshipAccountPersonalStatusUpdatedMessage>();
            await bus.Subscribe<FriendshipAccountPresenceUpdatedMessage>();
            await bus.Subscribe<FriendshipAccountRemovedMessage>();
            await bus.Subscribe<FriendshipAccountStatusUpdatedMessage>();
            await bus.Subscribe<FriendshipAccountUpdatedMessage>();
            await bus.Subscribe<FriendshipAddedMessage>();
            await bus.Subscribe<FriendshipInviteListMessage>();
            await bus.Subscribe<FriendshipInviteRemovedMessage>();
            await bus.Subscribe<FriendshipLastOnlineUpdatedMessage>();
            await bus.Subscribe<FriendshipLevelUpdatedMessage>();
            await bus.Subscribe<FriendshipListMessage>();
            await bus.Subscribe<FriendshipLocationsUpdatedMessage>();
            await bus.Subscribe<FriendshipNoteUpdatedMessage>();
            await bus.Subscribe<FriendshipRemovedMessage>();
            await bus.Subscribe<FriendshipResultMessage>();
            await bus.Subscribe<FriendshipTypeUpdatedMessage>();

            await bus.Subscribe<GroupActionResultMessage>();
            await bus.Subscribe<GroupFlagsUpdatedMessage>();
            await bus.Subscribe<GroupLootRulesUpdatedMessage>();
            await bus.Subscribe<GroupMarkerUpdatedMessage>();
            await bus.Subscribe<GroupMaxSizeUpdatedMessage>();
            await bus.Subscribe<GroupMemberAddedMessage>();
            await bus.Subscribe<GroupMemberFlagsUpdatedMessage>();
            await bus.Subscribe<GroupMemberJoinedMessage>();
            await bus.Subscribe<GroupMemberLeftMessage>();
            await bus.Subscribe<GroupMemberPositionUpdatedMessage>();
            await bus.Subscribe<GroupMemberPromotedMessage>();
            await bus.Subscribe<GroupMemberRealmUpdatedMessage>();
            await bus.Subscribe<GroupMemberRemovedMessage>();
            await bus.Subscribe<GroupMemberRequestedMessage>();
            await bus.Subscribe<GroupMemberRequestResultMessage>();
            await bus.Subscribe<GroupMemberStatsUpdatedMessage>();
            await bus.Subscribe<GroupPlayerInvitedMessage>();
            await bus.Subscribe<GroupPlayerInviteResultMessage>();
            await bus.Subscribe<GroupReadyCheckStartedMessage>();

            await bus.Subscribe<PlayerGroupAssociationUpdatedMessage>();
            await bus.Subscribe<PlayerInfoResponseMessage>();

            await bus.Subscribe<WhoResponseMessage>();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
