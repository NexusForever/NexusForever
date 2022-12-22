namespace NexusForever.Database.Configuration.Model
{
    public interface IDatabaseConfig
    {
        IConnectionString GetConnectionString(DatabaseType type);
    }
}
