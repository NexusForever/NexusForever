using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.Configuration;

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

        public static DbContextOptionsBuilder UseConfiguration(this DbContextOptionsBuilder optionsBuilder, IDatabaseConfiguration databaseConfiguration, DatabaseType databaseType)
        {
            var connectionString = databaseConfiguration.GetConnectionString(databaseType);
            switch (connectionString.Provider)
            {
                case DatabaseProvider.MySql:
                    optionsBuilder.UseMySql(connectionString.ConnectionString);
                    break;
                default:
                    throw new NotSupportedException($"The requested database provider: {connectionString.Provider:G} is not supported.");
            }
            return optionsBuilder;
        }

        public static IEnumerable<T> Dequeue<T>(this ConcurrentQueue<T> queue, uint count)
        {
            for (uint i = 0u; i < count && queue.Count > 0; i++)
            {
                queue.TryDequeue(out T result);
                yield return result;
            }
        }
    }
}
