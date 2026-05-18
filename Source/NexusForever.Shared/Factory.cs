using System;
using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Shared
{
    public class Factory<T> : IFactory<T> where T : class
    {
        private readonly IServiceProvider sp;

        public Factory(
            IServiceProvider sp)
        {
            this.sp = sp;
        }

        public T Resolve()
        {
            return sp.GetService<T>();
        }
    }
}
