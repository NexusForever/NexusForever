using Microsoft.Extensions.DependencyInjection;
using NexusForever.WorldServer.Network.Internal.Handler.Chat;
using NexusForever.WorldServer.Network.Internal.Handler.Friendship;
using NexusForever.WorldServer.Network.Internal.Handler.Group;
using NexusForever.WorldServer.Network.Internal.Handler.Player;
using Rebus.Config;

namespace NexusForever.WorldServer.Network.Internal.Handler
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetworkInternalHandlers(this IServiceCollection sc)
        {
            sc.AddRebusHandler<ChatChannelActionHandler>();
            sc.AddRebusHandler<ChatChannelJoinResultHandler>();
            sc.AddRebusHandler<ChatChannelMemberAddedHandler>();
            sc.AddRebusHandler<ChatChannelMembersHandler>();
            sc.AddRebusHandler<ChatChannelResultHandler>();
            sc.AddRebusHandler<ChatChannelTextAcceptedHandler>();
            sc.AddRebusHandler<ChatChannelTextHandler>();
            sc.AddRebusHandler<ChatChannelTextResultHandler>();
            sc.AddRebusHandler<ChatWhisperFailedHandler>();
            sc.AddRebusHandler<ChatWhisperTextHandler>();

            sc.AddRebusHandler<FriendshipAccountInviteListHandler>();
            sc.AddRebusHandler<FriendshipAccountInviteRemovedHandler>();
            sc.AddRebusHandler<FriendshipAccountLastOnlineUpdatedHandler>();
            sc.AddRebusHandler<FriendshipAccountLevelUpdatedHandler>();
            sc.AddRebusHandler<FriendshipAccountListHandler>();
            sc.AddRebusHandler<FriendshipAccountLocationsUpdatedHandler>();
            sc.AddRebusHandler<FriendshipAccountNicknameUpdatedHandler>();
            sc.AddRebusHandler<FriendshipAccountNoteUpdatedHandler>();
            sc.AddRebusHandler<FriendshipAccountPersonalStatusUpdatedHandler>();
            sc.AddRebusHandler<FriendshipAccountPresenceUpdatedHandler>();
            sc.AddRebusHandler<FriendshipAccountStatusUpdatedHandler>();
            sc.AddRebusHandler<FriendshipAccountRemovedHandler>();
            sc.AddRebusHandler<FriendshipAccountUpdatedHandler>();
            sc.AddRebusHandler<FriendshipAddedHandler>();
            sc.AddRebusHandler<FriendshipInviteListHandler>();
            sc.AddRebusHandler<FriendshipInviteRemovedHandler>();
            sc.AddRebusHandler<FriendshipLastOnlineUpdatedHandler>();
            sc.AddRebusHandler<FriendshipLevelUpdatedHandler>();
            sc.AddRebusHandler<FriendshipListHandler>();
            sc.AddRebusHandler<FriendshipLocationsUpdatedHandler>();
            sc.AddRebusHandler<FriendshipNoteUpdatedHandler>();
            sc.AddRebusHandler<FriendshipRemovedHandler>();
            sc.AddRebusHandler<FriendshipResultHandler>();
            sc.AddRebusHandler<FriendshipTypeUpdatedHandler>();

            sc.AddRebusHandler<GroupActionResultHandler>();
            sc.AddRebusHandler<GroupFlagsUpdatedHandler>();
            sc.AddRebusHandler<GroupLootRulesUpdatedHandler>();
            sc.AddRebusHandler<GroupMarkerUpdatedHandler>();
            sc.AddRebusHandler<GroupMaxSizeUpdatedHandler>();
            sc.AddRebusHandler<GroupMemberAddedHandler>();
            sc.AddRebusHandler<GroupMemberFlagsUpdatedHandler>();
            sc.AddRebusHandler<GroupMemberJoinedHandler>();
            sc.AddRebusHandler<GroupMemberLeftHandler>();
            sc.AddRebusHandler<GroupMemberPositionUpdatedHandler>();
            sc.AddRebusHandler<GroupMemberPromotedHandler>();
            sc.AddRebusHandler<GroupMemberRealmUpdatedHandler>();
            sc.AddRebusHandler<GroupMemberRemovedHandler>();
            sc.AddRebusHandler<GroupMemberRequestedHandler>();
            sc.AddRebusHandler<GroupMemberRequestResultHandler>();
            sc.AddRebusHandler<GroupMemberStatsUpdatedHandler>();
            sc.AddRebusHandler<GroupPlayerInvitedHandler>();
            sc.AddRebusHandler<GroupPlayerInviteResultHandler>();
            sc.AddRebusHandler<GroupReadyCheckStartedHandler>();

            sc.AddRebusHandler<PlayerGroupAssociationUpdatedHandler>();
            sc.AddRebusHandler<PlayerInfoResponseHandler>();

            return sc;
        }
    }
}
