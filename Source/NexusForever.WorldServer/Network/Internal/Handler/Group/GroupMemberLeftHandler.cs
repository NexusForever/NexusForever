using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMemberLeftHandler : IHandleMessages<GroupMemberLeftMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;
        private readonly IMatchManager matchManager;

        public GroupMemberLeftHandler(
            IPlayerManager playerManager,
            IMatchManager matchManager)
        {
            this.playerManager = playerManager;
            this.matchManager  = matchManager;
        }

        #endregion

        public Task Handle(GroupMemberLeftMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.RemovedMember.Identity.ToGameIdentity());
            if (player == null)
                return Task.CompletedTask;

            player.Session.EnqueueMessageEncrypted(new ServerGroupLeave
            {
                GroupId = message.Group.Id,
                Reason  = message.Reason
            });

            // TODO: Rawaho: this is not thread safe, matches should really be moved to a seperate server...
            if (message.Group.Match != null)
            {
                IMatch match = matchManager.GetMatch(message.Group.Match.Value);
                if (match == null)
                    return Task.CompletedTask;

                match.MatchExit(player, true);
                match.MatchLeave(message.RemovedMember.Identity.ToGameIdentity());
            }
            
            return Task.CompletedTask;
        }
    }
}
