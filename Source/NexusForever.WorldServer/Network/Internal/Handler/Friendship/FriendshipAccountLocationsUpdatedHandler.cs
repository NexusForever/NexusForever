using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountLocationsUpdatedHandler : IHandleMessages<FriendshipAccountLocationsUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountLocationsUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountLocationsUpdatedMessage message)
        {
            IPlayer player = playerManager.GetPlayerByAccountId(message.Account.Id);
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipAccountCharacterZoneChanged
            {
                ZoneInfos = message.Friends.ConvertAll(f => new ServerFriendshipAccountCharacterZoneChanged.FriendCharacterZoneInfo
                {
                    AccountFriendId    = f.Id,
                    CharacterInNewZone = f.InviteeAccount.ActiveCharacter.Identity.ToNetworkIdentity(),
                    WorldZoneId        = f.InviteeAccount.ActiveCharacter.WorldZoneId
                })
            });

            return Task.CompletedTask;
        }
    }
}
