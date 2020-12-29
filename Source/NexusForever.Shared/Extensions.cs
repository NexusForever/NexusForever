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
            for (uint i = 0u; i < count && queue.Count > 0; i++)
            {
                queue.TryDequeue(out T result);
                yield return result;
            }
        }

        public static int RoundOff(this int i)
        {
            return (int)Math.Round(i / 10.0) * 10;
        }
    }
}
