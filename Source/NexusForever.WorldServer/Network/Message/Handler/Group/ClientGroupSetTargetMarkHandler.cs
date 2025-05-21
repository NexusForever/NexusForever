using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupSetTargetMarkHandler : IMessageHandler<IWorldSession, ClientGroupSetTargetMark>
    {
        /// <summary>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGroupSetTargetMark clientMark)
        {
            // Players can only mark for their Active group.
            IGroup group = session.Player.GroupMembershipForeground.Group;
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, group.Id, session.Player.Name);
                return;
            }

            GroupHelper.AssertPermission(session, group.Id, GroupMemberInfoFlags.CanMark);
            group.MarkUnit(clientMark.UnitId, clientMark.TargetMarkerId);
        }
    }
}