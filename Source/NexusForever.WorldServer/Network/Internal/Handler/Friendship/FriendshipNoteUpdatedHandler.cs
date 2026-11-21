using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipNoteUpdatedHandler : IHandleMessages<FriendshipNoteUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipNoteUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipNoteUpdatedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Friend.InviterCharacter.Identity.ToGameIdentity());
            player.Session.EnqueueMessageEncrypted(new ServerFriendshipSetNote
            {
                FriendshipId = message.Friend.Id,
                Note         = message.Friend.Note
            });

            return Task.CompletedTask;
        }
    }
}
