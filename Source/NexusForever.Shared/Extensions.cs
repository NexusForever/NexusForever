using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace NexusForever.Shared
{
    public static class Extensions
    {
        public static IEnumerable<T> Dequeue<T>(this ConcurrentQueue<T> queue, uint count)
        {
            for (uint i = 0u; i < count && !queue.IsEmpty; i++)
            {
                queue.TryDequeue(out T result);
                yield return result;
            }
        }

        /// <summary>
        /// Round <see cref="TimeSpan"/> down to supplied <see cref="TimeSpan"/> interval.
        /// </summary>
        public static TimeSpan RoundDown(this TimeSpan span, TimeSpan intervalSpan)
        {
            return new TimeSpan((long)Math.Round(span.Ticks / (decimal)intervalSpan.Ticks, MidpointRounding.ToZero) * intervalSpan.Ticks);
        }

        public static uint Remaining(this Stream stream)
        {
            if (stream.Length < stream.Position)
                throw new InvalidOperationException();

            return (uint)(stream.Length - stream.Position);
        }
    }
}
