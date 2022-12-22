using System;
using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Configuration.Model;

namespace NexusForever.Database
{
    public static class Extensions
    {
        public static DbContextOptionsBuilder UseConfiguration(this DbContextOptionsBuilder optionsBuilder, IConnectionString connectionString)
        {
            switch (connectionString.Provider)
            {
                case DatabaseProvider.MySql:
                    optionsBuilder.UseMySql(connectionString.ConnectionString, ServerVersion.AutoDetect(connectionString.ConnectionString), b =>
                    {
                        b.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });
                    break;
                default:
                    throw new NotSupportedException($"The requested database provider: {connectionString.Provider:G} is not supported.");
            }
            return optionsBuilder;
        }
    }
}
