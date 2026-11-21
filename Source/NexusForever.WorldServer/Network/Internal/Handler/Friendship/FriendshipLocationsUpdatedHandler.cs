using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipLocationsUpdatedHandler : IHandleMessages<FriendshipLocationsUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipLocationsUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipLocationsUpdatedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Character.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipLocationUpdate
            {
                FriendLocations = message.Friends.ConvertAll(f => new ServerFriendshipLocationUpdate.FriendLocation
                {
                    FriendshipId = f.Id,
                    WorldZoneId  = f.InviteeCharacter.WorldZoneId
                })
            });

            return Task.CompletedTask;
        }
    }
}
