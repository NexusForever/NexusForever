namespace NexusForever.Game.Abstract.Map
{
    public interface IQueuedCounter
    {
        uint Dequeue();
        void Enqueue(uint value);
    }
}