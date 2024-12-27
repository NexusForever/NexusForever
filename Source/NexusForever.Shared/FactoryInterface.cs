using System;
using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Shared
{
    public class FactoryInterface<T> : IFactoryInterface<T> where T : class
    {
        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public FactoryInterface(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public T2 Resolve<T2>() where T2 : T
        {
            return serviceProvider.GetService<T2>();
        }
    }
}
