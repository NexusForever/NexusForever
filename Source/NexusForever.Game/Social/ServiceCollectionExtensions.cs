using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Social;
using NexusForever.Shared;

namespace NexusForever.Game.Social
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameSocial(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalChatManager, GlobalChatManager>();
        }
    }
}
