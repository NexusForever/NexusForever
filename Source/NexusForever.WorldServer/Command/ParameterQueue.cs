using System.Collections.Generic;
using System.Linq;

namespace NexusForever.WorldServer.Command
{
    public class ParameterQueue
    {
        /// <summary>
        /// Returns the amount of parameters left in the queue.
        /// </summary>
        public uint Count => (uint)queue.Count;

        /// <summary>
        /// Returns the parameter at the front of the queue.
        /// </summary>
        public string Front => queue.Count != 0 ? queue.Peek() : null;

        /// <summary>
        /// Return a string representing a breadcrumb trail of previously dequeued parameters.
        /// </summary>
        public string BreadcrumbTrail => string.Join(' ', breadcrumbs.Reverse());

        private readonly Queue<string> queue;
        private readonly Stack<string> breadcrumbs = new();

        /// <summary>
        /// Create a new <see cref="ParameterQueue"/> from an array of parameters.
        /// </summary>
        public ParameterQueue(string[] commands)
        {
            queue = new Queue<string>(commands);
        }

        /// <summary>
        /// Returns the next parameter and removes it from the queue.
        /// </summary>
        public string Dequeue()
        {
            string value = queue.Dequeue();
            breadcrumbs.Push(value);
            return value;
        }
    }
}
