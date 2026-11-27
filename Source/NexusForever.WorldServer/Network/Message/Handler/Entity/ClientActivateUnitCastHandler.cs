using System.Linq;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Quest;
using NexusForever.Game.Spell;
using NexusForever.Game.Static.Quest;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

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
            IPlayer player = session.Player;
            IWorldEntity entity = player.GetVisible<IWorldEntity>(activateUnitCast.UnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            if (EverstarPassage.TryBegin(player, entity))
                return;

            if (!QuestCsiInteraction.TryBegin(player, entity, CompleteActivation))
                CompleteActivation(player, entity);
        }

        private void CompleteActivation(IPlayer player, IWorldEntity entity)
        {
            // TODO: sanity check for range etc.

            player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, entity.CreatureId, 1u);
            foreach (uint targetGroupId in assetManager.GetTargetGroupsForCreatureId(entity.CreatureId) ?? Enumerable.Empty<uint>())
                player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateTargetGroup, targetGroupId, 1u); // Updates the objective, but seems to disable all the other targets. TODO: Investigate

            entity.OnActivateCast(player);
        }
    }
}
