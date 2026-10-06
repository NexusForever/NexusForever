using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Group;
using NexusForever.Shared;

namespace NexusForever.Game.Group
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameGroup(this IServiceCollection sc)
        {
            sc.AddSingleton<IGroupManager, GroupManager>();
            sc.AddTransientFactory<IGroup, Group>();
            sc.AddTransientFactory<IGroupMember, GroupMember>();
        }
    }
}
