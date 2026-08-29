using NexusForever.Database.Auth.Model;

namespace NexusForever.API.Character.Server
{
    public static class ServerMappingExtensions
    {
        public static Server ToServer(this ServerModel server)
        {
            return new Server
            {
                Id   = server.Id,
                Name = server.Name,
            };
        }
    }
}
