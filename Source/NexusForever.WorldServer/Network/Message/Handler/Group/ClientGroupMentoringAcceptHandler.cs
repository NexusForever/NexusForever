using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupMentoringAcceptHandler : IMessageHandler<IWorldSession, ClientGroupMentoringAccept>
    {
        public void HandleMessage(IWorldSession session, ClientGroupMentoringAccept mentoringAccept)
        {
            IPlayer mentor = session.Player;
            IPlayer mentee = PlayerManager.Instance.GetPlayer(mentoringAccept.MentorIdentity.ToGame());

            if(mentee != null)
            {
                session.EnqueueMessageEncrypted(new ServerGroupMentoringResult
                {
                    Cancelled = false,
                    GroupId = mentoringAccept.GroupId,
                    Mentee = mentoringAccept.MentorIdentity,
                    Mentor = mentor.Identity.ToNetwork()
                });
                return;
            }    
        }
    }
}