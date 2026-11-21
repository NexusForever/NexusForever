using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFriend(this IServiceCollection sc)
        {
            sc.AddTransient<FriendAccount>();
            sc.AddTransient<FriendAccountFactory>();
            sc.AddTransient<FriendAccountInvite>();
            sc.AddTransient<FriendAccountInviteValidator>();
            sc.AddTransient<FriendAccountInviteFactory>();
            sc.AddScoped<FriendAccountManager>();

            sc.AddTransient<Friend>();
            sc.AddTransient<FriendFactory>();
            sc.AddTransient<FriendInvite>();
            sc.AddTransient<FriendInviteValidator>();
            sc.AddTransient<FriendInviteFactory>();
            sc.AddTransient<FriendRequestHandler>();
            sc.AddScoped<FriendManager>();

            sc.AddTransient<FriendshipResultPublisher>();
        }
    }
}
