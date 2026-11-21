using System;
using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Friendship;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Friendship;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Friendship
{
    public class FriendshipInviteListHandler : IHandleMessages<FriendshipInviteListMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public FriendshipInviteListHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(FriendshipInviteListMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerFriendshipInviteList
            {
                Invites = message.Invites.ConvertAll(i => new ServerFriendshipInviteList.InviteData
                {
                    InviteId       = i.Id,
                    PlayerIdentity = i.InviterCharacter.Identity.ToNetworkIdentity(),
                    Name           = i.InviterCharacter.IdentityName.Name,
                    Seen           = (byte)(i.Seen ? 0x01 : 0x00), // what about other flags?
                    ExpiryInDays   = (float)(i.Expiration - DateTime.UtcNow).TotalDays,
                    Note           = i.Note,
                    Class          = i.InviterCharacter.Class,
                    Path           = i.InviterCharacter.Path,
                    Level          = i.InviterCharacter.Level
                })
            });

            return Task.CompletedTask;
        }
    }
}
