using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Cinematic;

namespace NexusForever.Game.Cinematic
{
    public class CinematicFactory : ICinematicFactory
    {
        private readonly IServiceProvider serviceProvider;

        public CinematicFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ICinematicBase CreateCinematic<T>() where T : ICinematicBase
        {
            return serviceProvider.GetRequiredService<T>();
        }
    }
}
