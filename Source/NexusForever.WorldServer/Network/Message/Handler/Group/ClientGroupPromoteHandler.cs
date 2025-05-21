using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupPromoteHandler : IMessageHandler<IWorldSession, ClientGroupPromote>
    {
        /// <summary>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGroupPromote clientGroupPromote)
        {
            GroupHelper.AssertGroupId(session, clientGroupPromote.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(clientGroupPromote.GroupId);
            group.Promote(clientGroupPromote.TargetedPlayer);
        }
    }
}