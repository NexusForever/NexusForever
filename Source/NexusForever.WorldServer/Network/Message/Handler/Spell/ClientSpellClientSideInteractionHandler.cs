using NexusForever.Game.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientSpellClientSideInteractionHandler : IMessageHandler<IWorldSession, ClientSpellClientSideInteraction>
    {
        public void HandleMessage(IWorldSession session, ClientSpellClientSideInteraction result)
        {
            QuestCsiInteraction.Resolve(session.Player, result.ServerUniqueId, result.CSIResponse);
        }
    }
}
