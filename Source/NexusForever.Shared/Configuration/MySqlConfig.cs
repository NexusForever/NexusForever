namespace NexusForever.Shared.Configuration
{
    public class MySqlConfig : IConnectionString
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        DatabaseProvider IConnectionString.Provider => DatabaseProvider.MySql;
        string IConnectionString.ConnectionString => $"server={Host};port={Port};user={Username};password={Password};database={Database}";
    }
}
