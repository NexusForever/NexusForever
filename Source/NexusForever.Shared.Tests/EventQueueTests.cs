using NexusForever.Shared.Game.Events;
using NexusForever.Shared.Game.Events.Static;
using Xunit;

namespace NexusForever.Shared.Tests
{
    public class EventQueueTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void BlockingEventPreservesDeferredEvents(int deferredCount)
        {
            var queue = new EventQueue();
            var executed = new List<int>();
            bool ready = false;
            for (int i = 0; i < deferredCount; i++)
            {
                int id = i;
                queue.EnqueueEvent(new PredicateEvent(() => ready, () => executed.Add(id)));
            }
            queue.EnqueueEvent(new PredicateEvent(() => ready, () => executed.Add(-1)), ConditionalEventType.Blocking);
            queue.EnqueueEvent(new PredicateEvent(() => true, () => executed.Add(-2)));

            queue.Update(0);
            queue.Update(0);
            Assert.Empty(executed);
            Assert.True(queue.PendingEvents);

            ready = true;
            queue.Update(0);
            Assert.Equal(new[] { -1, -2 }.Concat(Enumerable.Range(0, deferredCount)), executed);
            Assert.False(queue.PendingEvents);

            queue.Update(0);
            Assert.Equal(deferredCount + 2, executed.Count);
        }

        [Fact]
        public void DeferredEventCanBecomeReadyAfterBlockerHasCompleted()
        {
            var queue = new EventQueue();
            bool deferredReady = false;
            bool blockerReady = false;
            int executions = 0;
            queue.EnqueueEvent(new PredicateEvent(() => deferredReady, () => executions++));
            queue.EnqueueEvent(new PredicateEvent(() => blockerReady, () => { }), ConditionalEventType.Blocking);

            queue.Update(0);
            blockerReady = true;
            queue.Update(0);
            Assert.True(queue.PendingEvents);
            Assert.Equal(0, executions);

            deferredReady = true;
            queue.Update(0);
            Assert.Equal(1, executions);
            Assert.False(queue.PendingEvents);
        }

        [Fact]
        public void StandardEventsDoNotBlockReadyEvents()
        {
            var queue = new EventQueue();
            bool ready = false;
            var executed = new List<int>();
            queue.EnqueueEvent(new PredicateEvent(() => ready, () => executed.Add(1)));
            queue.EnqueueEvent(new PredicateEvent(() => true, () => executed.Add(2)));

            queue.Update(0);
            Assert.Equal(new[] { 2 }, executed);
            ready = true;
            queue.Update(0);
            Assert.Equal(new[] { 2, 1 }, executed);
            Assert.False(queue.PendingEvents);
        }
    }
}
