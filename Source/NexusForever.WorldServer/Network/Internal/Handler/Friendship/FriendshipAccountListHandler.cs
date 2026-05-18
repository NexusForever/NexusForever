using System;
using System.Threading.Tasks;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipAccountListHandler : IHandleMessages<FriendshipAccountListMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipAccountListHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipAccountListMessage message)
        {
            IPlayer player = playerManager.GetPlayerByAccountId(message.Account.Id);
            if (player == null)
                return Task.CompletedTask;

            var friendshipAccountList = new ServerFriendshipAccountList();
            foreach (FriendAccount friend in message.Friends)
            {
                var accountFriend = new ServerFriendshipAccountList.AccountFriend
                {
                    AccountId       = friend.InviteeAccount.Id,
                    AccountFriendId = friend.Id,
                    PublicNote      = friend.InviteeAccount.Status,
                    PrivateNote     = friend.Note,
                    DisplayName     = friend.InviteeAccount.Nickname ?? friend.InviteeAccount.Email,
                    Presence        = friend.InviteeAccount.Presence
                };

                if (friend.InviteeAccount.LastOnline != null)
                    accountFriend.DaysSinceLastLogin = (float)(friend.InviteeAccount.LastOnline.Value - DateTime.UtcNow).TotalDays;

                // retail sent data for all characters although the UI only ever references the active one
                // any implications not sending every character?
                if (friend.InviteeAccount.ActiveCharacter != null)
                {
                    accountFriend.CharacterList.Add(new CharacterData
                    {
                        PlayerIdentity = friend.InviteeAccount.ActiveCharacter.Identity.ToNetworkIdentity(),
                        Name           = friend.InviteeAccount.ActiveCharacter.IdentityName.Name,
                        Class          = friend.InviteeAccount.ActiveCharacter.Class,
                        Race           = friend.InviteeAccount.ActiveCharacter.Race,
                        Path           = friend.InviteeAccount.ActiveCharacter.Path,
                        Level          = friend.InviteeAccount.ActiveCharacter.Level,
                        WorldZoneId    = friend.InviteeAccount.ActiveCharacter.WorldZoneId,
                        Faction        = friend.InviteeAccount.ActiveCharacter.Faction,
                    });
                }

                friendshipAccountList.FriendListData.Add(accountFriend);
            }

            player.Session.EnqueueMessageEncrypted(friendshipAccountList);

            return Task.CompletedTask;
        }
    }
}
