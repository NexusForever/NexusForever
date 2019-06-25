namespace NexusForever.WorldServer.Game
{
    public abstract class Manager<T> where T : class, new()
    {
        public static T Instance
        {
            get
            {
                // NOT thread safe, this should be fine as all managers should be initialised during startup
                if (instance == null)
                    instance = new T();

                return instance;
            }
        }

        private static T instance;
    }
}
