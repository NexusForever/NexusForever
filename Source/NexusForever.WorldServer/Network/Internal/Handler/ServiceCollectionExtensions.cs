using Microsoft.Extensions.DependencyInjection;
using NexusForever.WorldServer.Network.Internal.Handler.Chat;
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

            return sc;
        }
    }
}
