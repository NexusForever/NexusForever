using NexusForever.Game.Abstract.Entity.Movement;

namespace NexusForever.Game.Entity.Movement.Key
{
    public abstract class Keys<T>
    {
        /// <summary>
        /// Returns if the keys have been finalised.
        /// </summary>
        public bool IsFinalised => movementManager.GetTime() >= Times[^1];

        public List<uint> Times { get; private set; }
        public List<T> Values { get; private set; }

        private IMovementManager movementManager;

        /// <summary>
        /// Initialise with the supplied times and values.
        /// </summary>
        public void Initialise(IMovementManager movementManager, List<uint> times, List<T> values)
        {
            if (times.Count < 2)
                throw new ArgumentException();

            if (times.Count != values.Count)
                throw new ArgumentException();

            this.movementManager = movementManager;

            Times  = times;
            Values = values;

            // adjust local times to be relative to the current time
            uint time = movementManager.GetTime();
            for (int i = 0; i < times.Count; i++)
                times[i] += time;
        }

        /// <summary>
        /// Return the interpolated value at the current time.
        /// </summary>
        public T GetInterpolated()
        {
            uint time = movementManager.GetTime();
            if (time < Times[0])
                return Values[0];

            for (int i = 0; i < Times.Count - 1; i++)
            {
                if (time < Times[i + 1])
                {
                    float t = (time - Times[i]) / (float)(Times[i + 1] - Times[i]);
                    return CalculateInterpolated(Values[i], Values[i + 1], t);
                }
            }

            return Values[^1];
        }

        /// <summary>
        /// Return the index of the current time.
        /// </summary>
        public int GetIndex()
        {
            uint time = movementManager.GetTime();
            for (int i = 0; i < Times.Count - 1; i++)
                if (time < Times[i + 1])
                    return i;

            return Times.Count - 1;
        }

        /// <summary>
        /// Calculate the interpolcated value at the current time.
        /// </summary>
        public abstract T CalculateInterpolated(T v1, T v2, float t);
    }
}
