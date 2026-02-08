using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Abilities;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientSetSpecHandler : IMessageHandler<IWorldSession, ClientSetSpec>
    {
        public void HandleMessage(IWorldSession session, ClientSetSpec changeActiveActionSet)
        {
            session.EnqueueMessageEncrypted(new ServerSpecChanged
            {
                SpecError      = session.Player.SpellManager.SetActiveActionSet(changeActiveActionSet.SpecIndex),
                ActionSetIndex = session.Player.SpellManager.ActiveActionSet
            });

            session.Player.SpellManager.SendServerAbilityPoints();
        }
    }
}
