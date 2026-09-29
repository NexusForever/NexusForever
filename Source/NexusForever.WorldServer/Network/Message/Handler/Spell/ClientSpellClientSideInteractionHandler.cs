using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Quest;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientSpellClientSideInteractionHandler : IMessageHandler<IWorldSession, ClientSpellClientSideInteraction>
    {
        public void HandleMessage(IWorldSession session, ClientSpellClientSideInteraction clientSideInteraction)
        {
            // CSIResponse: 2 = success, 0 = fail (see packet model comment).
            if (clientSideInteraction.CSIResponse != 2)
            {
                session.Player.CancelSpellCast(clientSideInteraction.ServerUniqueId);
                return;
            }

            ISpell spell = session.Player.GetActiveSpell(s => s.CastingId == clientSideInteraction.ServerUniqueId);
            uint? targetGuid = spell?.Parameters.PrimaryTargetId;
            if (targetGuid is null or 0)
                targetGuid = session.Player.TargetGuid;

            if (targetGuid is null or 0)
                return;

            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(targetGuid.Value);
            if (entity == null)
                return;

            session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.SucceedCSI, entity.CreatureId, 1u);
        }
    }
}
