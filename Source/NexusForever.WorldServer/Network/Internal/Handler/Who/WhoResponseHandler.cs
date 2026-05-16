using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Who;
using NexusForever.Network.World.Message.Model.Who;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Who
{
    public class WhoResponseHandler : IHandleMessages<WhoResponseMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public WhoResponseHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(WhoResponseMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerWhoResponse
            {
                Result  = message.Result,
                Players = message.Characters.ConvertAll(c => new ServerWhoResponse.WhoPlayer
                {
                    Name    = c.IdentityName.Name,
                    Realm   = c.IdentityName.RealmName,
                    Race    = c.Race,
                    Class   = c.Class,
                    Path    = c.Path,
                    Faction = c.Faction,
                    Sex     = c.Sex,
                    Zone    = c.WorldZoneId,
                    Level   = c.Level
                })
            });

            return Task.CompletedTask;
        }
    }
}
