namespace NexusForever.Database.Configuration.Model
{
    public class DatabaseConnectionString : IConnectionString
    {
        public DatabaseProvider Provider { get; set; }
        public string ConnectionString { get; set; }
    }
}
