namespace NexusForever.Database.Configuration
{
    public interface IDatabaseConfig
    {
        IConnectionString GetConnectionString(DatabaseType type);
    }
}
