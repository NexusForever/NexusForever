using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Server.Friendship.Game.Account
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAccount(this IServiceCollection sc)
        {
            sc.AddTransient<Account>();
            sc.AddTransient<AccountFriend>();
            sc.AddTransient<AccountFriendInverse>();
            sc.AddTransient<AccountFriendInvite>();
            sc.AddTransient<AccountFriendInvitePending>();

            sc.AddScoped<AccountManager>();

            return sc;
        }
    }
}
