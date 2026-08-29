using Microsoft.Extensions.DependencyInjection;
using NexusForever.Server.GroupServer.Network.Internal.Handler.Group;
using NexusForever.Server.GroupServer.Network.Internal.Handler.Match;
using NexusForever.Server.GroupServer.Network.Internal.Handler.Player;
using Rebus.Config;

namespace NexusForever.Server.GroupServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetworkInternalHandlers(this IServiceCollection sc)
        {
            sc.AddRebusHandler<GroupDisbandHandler>();
            sc.AddRebusHandler<GroupFlagsUpdateHandler>();
            sc.AddRebusHandler<GroupLootRulesUpdateHandler>();
            sc.AddRebusHandler<GroupMarkerHandler>();
            sc.AddRebusHandler<GroupMemberFlagUpdateHandler>();
            sc.AddRebusHandler<GroupMemberKickHandler>();
            sc.AddRebusHandler<GroupMemberLeaveHandler>();
            sc.AddRebusHandler<GroupMemberRequestHandler>();
            sc.AddRebusHandler<GroupMemberRequestReponseHandler>();
            sc.AddRebusHandler<GroupMemberPromoteHandler>();
            sc.AddRebusHandler<GroupPlayerInviteHandler>();
            sc.AddRebusHandler<GroupPlayerInviteRespondedHandler>();
            sc.AddRebusHandler<GroupReadyCheckMessageHandler>();

            sc.AddRebusHandler<MatchCreatedHandler>();
            sc.AddRebusHandler<MatchMemberLeftHandler>();
            sc.AddRebusHandler<MatchRemovedHandler>();

            sc.AddRebusHandler<PlayerLoggedInHandler>();
            sc.AddRebusHandler<PlayerLoggedOutHandler>();
            sc.AddRebusHandler<PlayerPositionUpdatedHandler>();
            sc.AddRebusHandler<PlayerPropertyUpdatedHandler>();
            sc.AddRebusHandler<PlayerStatUpdatedHandler>();
            sc.AddRebusHandler<PlayerWorldUpdatedHandler>();
            sc.AddRebusHandler<PlayerWorldZoneUpdatedHandler>();
            return sc;
        }
    }
}
