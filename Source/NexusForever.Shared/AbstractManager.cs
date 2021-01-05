using NLog;
using System;

namespace NexusForever.Shared
{
    public abstract class AbstractManager<T> : Singleton<T>, IShutdownAble where T : class
    {
        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public virtual void Shutdown()
        {
            // By default do nothing
        }

        public virtual T Initialise()
        {
            throw new NotImplementedException();
        }
    }
}
