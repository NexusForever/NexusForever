using System.Linq;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity
{
    public class ClientActivateUnitCastHandler : IMessageHandler<IWorldSession, ClientActivateUnitCast>
    {
        #region Dependency Injection

        private readonly IAssetManager assetManager;

        public ClientActivateUnitCastHandler(
            IAssetManager assetManager)
        {
            this.assetManager = assetManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientActivateUnitCast activateUnitCast)
        {
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(activateUnitCast.ActivateUnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            // TODO: sanity check for range etc.

            session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, entity.CreatureId, 1u);
            foreach (uint targetGroupId in assetManager.GetTargetGroupsForCreatureId(entity.CreatureId) ?? Enumerable.Empty<uint>())
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateTargetGroup, targetGroupId, 1u); // Updates the objective, but seems to disable all the other targets. TODO: Investigate

            entity.OnActivateCast(session.Player);
        }
    }
}
