using System;
using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountInviteListHandler : IHandleMessages<FriendshipAccountInviteListMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountInviteListHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountInviteListMessage message)
        {
            IPlayer player = playerManager.GetPlayerByAccountId(message.Account.Id);
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipAccountInviteList
            {
                AccountId            = player.Account.Id,
                FriendAccountInvites = message.Invites.ConvertAll(i => new ServerFriendshipAccountInviteList.FriendAccountInviteInfo
                {
                    AccountFriendInviteId = i.Id,
                    DisplayName           = i.InviterAccount.Nickname ?? i.InviterAccount.Email,
                    Type                  = FriendshipType.Account,
                    Seen                  = (uint)(i.Seen ? 0x01 : 0x00), // what about other flags?
                    Note                  = i.Note,
                    DaysUntilExpired      = (float)(i.Expiration - DateTime.UtcNow).TotalDays
                })
            });

            return Task.CompletedTask;
        }
    }
}
