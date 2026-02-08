using Microsoft.EntityFrameworkCore;

namespace NexusForever.API.Character.Database
{
    public class ContextFactory<T> where T : DbContext
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;

        public ContextFactory(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        public T GetContext(ushort realmId)
        {
            return _serviceProvider.GetKeyedService<T>(realmId);
        }
    }
}
