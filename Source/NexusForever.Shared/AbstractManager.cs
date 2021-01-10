using NLog;
using System;

namespace NexusForever.Shared
{
    public abstract class AbstractManager<T> : Singleton<T>, IShutdown where T : class
    {
        protected readonly ILogger Log;

        protected AbstractManager()
        {
            Log = LogManager.GetLogger(GetType().FullName);
        }

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
