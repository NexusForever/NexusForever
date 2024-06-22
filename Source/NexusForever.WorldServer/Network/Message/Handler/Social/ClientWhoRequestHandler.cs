using System.Collections.Generic;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Social
{
    public class ClientWhoRequestHandler : IMessageHandler<IWorldSession, ClientWhoRequest>
    {
        public void HandleMessage(IWorldSession session, ClientWhoRequest request)
        {
            var players = new List<ServerWhoResponse.WhoPlayer>
            {
                new()
                {
                    Name    = session.Player.Name,
                    Level   = session.Player.Level,
                    Race    = session.Player.Race,
                    Class   = session.Player.Class,
                    Path    = session.Player.Path,
                    Faction = session.Player.Faction1,
                    Sex     = session.Player.Sex,
                    Zone    = session.Player.Zone.Id
                }
            };

            session.EnqueueMessageEncrypted(new ServerWhoResponse
            {
                Players = players
            });
        }
    }
}
