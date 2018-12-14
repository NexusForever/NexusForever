namespace NexusForever.Shared.Configuration
{
    public class DatabaseConfig
    {
        public MySqlConfig Auth { get; set; }
        public MySqlConfig Character { get; set; }
        public MySqlConfig World { get; set; }
    }
}
