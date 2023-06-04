using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Customisation;

namespace NexusForever.Game.Customisation
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameCustomisation(this IServiceCollection sc)
        {
            sc.AddSingleton<ICustomisationManager, CustomisationManager>();
        }
    }
}
