using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Shared
{
    public abstract class Singleton<T> where T : class
    {
        public static T Instance => LegacyServiceProvider.Provider.GetRequiredService<T>();
    }
}
