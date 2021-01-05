using System;

namespace NexusForever.Shared
{
    public abstract class Singleton<T> where T : class
    {
        public static T Instance => instance.Value;

        private static readonly Lazy<T> instance = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
    }
}
