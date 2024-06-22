using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientChangeActiveActionSetHandler : IMessageHandler<IWorldSession, ClientChangeActiveActionSet>
    {
        public void HandleMessage(IWorldSession session, ClientChangeActiveActionSet changeActiveActionSet)
        {
            session.EnqueueMessageEncrypted(new ServerChangeActiveActionSet
            {
                SpecError      = session.Player.SpellManager.SetActiveActionSet(changeActiveActionSet.ActionSetIndex),
                ActionSetIndex = session.Player.SpellManager.ActiveActionSet
            });

            session.Player.SpellManager.SendServerAbilityPoints();
        }
    }
}
