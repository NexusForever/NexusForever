using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Map
{
    public class QueuedCounter : IQueuedCounter
    {
        private uint counter = 1;
        private readonly Queue<uint> queue = new();

        public uint Dequeue()
        {
            return queue.TryDequeue(out uint value) ? value : counter++;
        }

        public void Enqueue(uint value)
        {
            queue.Enqueue(value);
        }
    }
}
