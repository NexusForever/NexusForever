using Microsoft.Extensions.DependencyInjection;
using NexusForever.Server.ChatServer.Network.Internal.Handler.Chat;
using NexusForever.Server.ChatServer.Network.Internal.Handler.Group;
using NexusForever.Server.ChatServer.Network.Internal.Handler.Guild;
using NexusForever.Server.ChatServer.Network.Internal.Handler.Player;
using NexusForever.Server.ChatServer.Network.Internal.Handler.Server;
using Rebus.Config;

namespace NexusForever.Server.ChatServer.Network.Internal.Handler
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetworkInternalHandlers(this IServiceCollection sc)
        {
            sc.AddRebusHandler<ChatChannelJoinHandler>();
            sc.AddRebusHandler<ChatChannelMemberKickHandler>();
            sc.AddRebusHandler<ChatChannelMemberLeaveHandler>();
            sc.AddRebusHandler<ChatChannelMemberModeratorHandler>();
            sc.AddRebusHandler<ChatChannelMemberMuteHandler>();
            sc.AddRebusHandler<ChatChannelMemberOwnerHandler>();
            sc.AddRebusHandler<ChatChannelMembersRequestHandler>();
            sc.AddRebusHandler<ChatChannelPasswordHandler>();
            sc.AddRebusHandler<ChatChannelTextRequestHandler>();
            sc.AddRebusHandler<ChatWhisperRequestHandler>();

            sc.AddRebusHandler<GroupDisbandedHandler>();
            sc.AddRebusHandler<GroupMemberAddedHandler>();
            sc.AddRebusHandler<GroupMemberRemovedHandler>();

            sc.AddSingleton<GuildChannelDataManager>();
            sc.AddRebusHandler<GuildCreatedHandler>();
            sc.AddRebusHandler<GuildMemberAddedHandler>();
            sc.AddRebusHandler<GuildMemberRankUpdatedHandler>();
            sc.AddRebusHandler<GuildMemberRemovedHandler>();

            sc.AddRebusHandler<PlayerLoggedInHandler>();
            sc.AddRebusHandler<PlayerLoggedOutHandler>();

            sc.AddRebusHandler<ServerWorldOfflineHandler>();
            sc.AddRebusHandler<ServerWorldOnlineHandler>();

            return sc;
        }
    }
}
