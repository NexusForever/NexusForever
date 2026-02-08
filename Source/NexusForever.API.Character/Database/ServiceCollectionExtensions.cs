using Microsoft.EntityFrameworkCore;

namespace NexusForever.API.Character.Database
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbContextKeyed<T>(this IServiceCollection sc, ushort realmId, Action<DbContextOptionsBuilder> action) where T : DbContext
        {
            sc.AddKeyedScoped(realmId, (sp, s) =>
            {
                var builder = new DbContextOptionsBuilder<T>();
                action(builder);
                return ActivatorUtilities.CreateInstance<T>(sp, builder.Options);
            });
        }
    }
}
