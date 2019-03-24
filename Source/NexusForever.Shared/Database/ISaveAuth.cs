using NexusForever.Shared.Database.Auth.Model;

namespace NexusForever.Shared.Database
{
    public interface ISaveAuth
    {
        void Save(AuthContext context);
    }
}
