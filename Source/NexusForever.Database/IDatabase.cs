using NexusForever.Database.Configuration.Model;

namespace NexusForever.Database;

public interface IDatabase
{
    void Initialise(IConnectionString connectionString);

    void Migrate();
}
