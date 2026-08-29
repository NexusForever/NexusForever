using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountNoteUpdatedHandler : IHandleMessages<FriendshipAccountNoteUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountNoteUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountNoteUpdatedMessage message)
        {
            IPlayer player = playerManager.GetPlayerByAccountId(message.FriendAccount.InviterAccount.Id);
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipAccountPrivateNote
            {
                AccountId   = message.FriendAccount.InviteeAccount.Id,
                PrivateNote = message.FriendAccount.Note
            });

            return Task.CompletedTask;
        }
    }
}
