using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipListHandler : IHandleMessages<FriendshipListMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipListHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipListMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Character.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipList
            {
                Friends = message.Friends.ConvertAll(f => new FriendData
                {
                    FriendshipId   = f.Id,
                    PlayerIdentity = f.InviteeCharacter.Identity.ToNetworkIdentity(),
                    Note           = f.Note,
                    Type           = f.Type
                })
            });

            return Task.CompletedTask;
        }
    }
}
