using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NexusForever.Shared
{
    public static class Extensions
    {
        public static byte[] ToByteArray(this string value)
        {
            return Enumerable.Range(0, value.Length / 2)
                .Select(x => Convert.ToByte(value.Substring(x * 2, 2), 16))
                .ToArray();
        }

        public static string ToHexString(this byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", "");
        }

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
    }
}
