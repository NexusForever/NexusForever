using System;
using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Configuration;

namespace NexusForever.Database
{
    public static class Extensions
    {
        public static DbContextOptionsBuilder UseConfiguration(this DbContextOptionsBuilder optionsBuilder, IDatabaseConfiguration databaseConfiguration, DatabaseType databaseType)
        {
            IConnectionString connectionString = databaseConfiguration.GetConnectionString(databaseType);
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
    }
}
