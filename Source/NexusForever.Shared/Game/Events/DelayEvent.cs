using System;
using System.Threading.Tasks;

namespace NexusForever.Shared.Game.Events
{
    /// <summary>
    /// An <see cref="IEvent"/> that will be executed after a <see cref="TimeSpan"/>.
    /// </summary>
    public class DelayEvent : TaskEvent
    {
        public DelayEvent(TimeSpan timeSpan, Action callback)
            : base(Task.Delay(timeSpan), callback)
        {
        }
    }
}
