using System;
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
    }
}
