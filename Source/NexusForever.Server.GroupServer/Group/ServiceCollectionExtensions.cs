using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Server.GroupServer.Group
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGroup(this IServiceCollection sc)
        {
            sc.AddScoped<GroupManager>();
            sc.AddTransient<Group>();
            sc.AddTransient<GroupInvite>();
            sc.AddTransient<GroupMarker>();
            sc.AddTransient<GroupMember>();
            return sc;
        }
    }
}
