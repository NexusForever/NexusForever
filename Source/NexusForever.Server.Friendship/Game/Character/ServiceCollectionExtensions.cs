using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Server.Friendship.Game.Character
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCharacter(this IServiceCollection sc)
        {
            sc.AddTransient<Character>();
            sc.AddTransient<CharacterFriend>();
            sc.AddTransient<CharacterFriendInverse>();
            sc.AddTransient<CharacterFriendInvite>();
            sc.AddTransient<CharacterFriendInvitePending>();
            sc.AddTransient<CharacterStat>();

            sc.AddScoped<CharacterManager>();

            return sc;
        }
    }
}
