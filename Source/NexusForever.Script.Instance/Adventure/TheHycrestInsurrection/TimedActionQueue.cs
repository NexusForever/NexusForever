namespace NexusForever.Script.Instance.Adventure.TheHycrestInsurrection
{
    /// <summary>
    /// Runs actions after a delay, driven by <see cref="Update(double)"/> from a script tick.
    /// </summary>
    public class TimedActionQueue
    {
        private readonly List<(double Remaining, Action Action)> actions = [];

        public bool IsEmpty => actions.Count == 0;

        /// <summary>
        /// Run <paramref name="action"/> after <paramref name="delay"/>.
        /// </summary>
        public void Enqueue(TimeSpan delay, Action action)
        {
            actions.Add((delay.TotalSeconds, action));
        }

        /// <summary>
        /// Remove all pending actions.
        /// </summary>
        public void Clear()
        {
            actions.Clear();
        }

        /// <summary>
        /// Advance time by <paramref name="lastTick"/> seconds and run every action that is due.
        /// </summary>
        public void Update(double lastTick)
        {
            if (actions.Count == 0)
                return;

            var due = new List<Action>();
            for (int i = actions.Count - 1; i >= 0; i--)
            {
                double remaining = actions[i].Remaining - lastTick;
                if (remaining <= 0d)
                {
                    due.Add(actions[i].Action);
                    actions.RemoveAt(i);
                }
                else
                    actions[i] = (remaining, actions[i].Action);
            }

            // run in the order they became due; actions may enqueue new ones
            due.Reverse();
            foreach (Action action in due)
                action();
        }
    }
}
