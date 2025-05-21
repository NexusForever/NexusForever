using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupFlagsChangedHandler : IMessageHandler<IWorldSession, ClientGroupFlagsChanged>
    {
        /// <summary>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGroupFlagsChanged clientGroupFlagsChanged)
        {
            GroupHelper.AssertGroupId(session, clientGroupFlagsChanged.GroupId);
            GroupHelper.AssertGroupLeader(session, clientGroupFlagsChanged.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(clientGroupFlagsChanged.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, clientGroupFlagsChanged.GroupId, session.Player.Name);
                return;
            }

            group.SetGroupFlags(clientGroupFlagsChanged.NewFlags);
        }
    }
}