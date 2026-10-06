using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using InternalGroupMember = NexusForever.Network.Internal.Message.Group.Shared.GroupMember;

namespace NexusForever.Game.Group
{
    public class GroupMember : IGroupMember
    {
        public Identity Identity { get; private set; }

        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMember(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public void Initialise(InternalGroupMember internalGroupMember)
        {
            Identity = internalGroupMember.Identity.ToGameIdentity();
        }

        public IPlayer GetPlayer()
        {
            return playerManager.GetPlayer(Identity);
        }
    }
}
