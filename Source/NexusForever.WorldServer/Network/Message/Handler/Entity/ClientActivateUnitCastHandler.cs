using System.Linq;
using System.Numerics;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
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
            IWorldEntity entity = session.Player.GetVisible<IWorldEntity>(activateUnitCast.UnitId);
            if (entity == null)
                throw new InvalidPacketValueException();

            // TODO: sanity check for range etc.

            // Apply activate spells/effects first so a failure does not leave quest objectives advanced
            // without the world side-effect (e.g. NPE hoverboard summon).
            entity.OnActivateCast(session.Player);

            session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, entity.CreatureId, 1u);
            // SucceedCSI is often driven by activate-cast completion; many NPE activates have no CSI minigame.
            session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.SucceedCSI, entity.CreatureId, 1u);

            // NPE Part 2 sim-mines: quest Data does not match creature ids for medium/hard pads.
            // Medium → ActivateEntity Data=0 (obj 21340); Hard → Data=5968 (obj 21318, mis-tagged in tbl).
            switch (entity.CreatureId)
            {
                case 73667: // Explosive Mine - Medium
                    session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, 0u, 1u);
                    entity.RemoveFromMap();
                    break;
                case 73668: // Explosive Mine - Hard
                    session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateEntity, 5968u, 1u);
                    entity.RemoveFromMap();
                    break;
                case 73463: // Explosive Mine - Easy
                    entity.RemoveFromMap();
                    break;
                case 73741: // Housing Hologram Projector (Part 2 → housing plot)
                    TryTeleportToHousingPlot(session.Player);
                    break;
            }

            foreach (uint targetGroupId in assetManager.GetTargetGroupsForCreatureId(entity.CreatureId) ?? Enumerable.Empty<uint>())
            {
                session.Player.QuestManager.ObjectiveUpdate(QuestObjectiveType.ActivateTargetGroup, targetGroupId, 1u); // Updates the objective, but seems to disable all the other targets. TODO: Investigate
                if (entity is ISimpleEntity simple)
                    session.Player.QuestManager.ObjectiveChecklistBit(targetGroupId, simple.QuestChecklistIdx);
            }
        }

        // Housing plot pad near Protostar Sky/Music specialists.
        private static readonly Vector3 HousingPlotPosition = new(1227.4f, -494.2f, 125f);

        /// <summary>
        /// NPE housing hologram projector (73741) — spell 87061 is Activate + fullscreen only.
        /// Teleport into the housing plot once Face of the Enemy is done / Claim to Stake is active.
        /// </summary>
        private static void TryTeleportToHousingPlot(IPlayer player)
        {
            QuestState? exileCombat    = player.QuestManager.GetQuestState(10518);
            QuestState? dominionCombat = player.QuestManager.GetQuestState(10524);
            QuestState? exileHousing   = player.QuestManager.GetQuestState(10525);
            QuestState? dominionHousing = player.QuestManager.GetQuestState(10526);

            bool combatDone = exileCombat is QuestState.Completed or QuestState.Achieved
                || dominionCombat is QuestState.Completed or QuestState.Achieved;
            bool housingActive = exileHousing is QuestState.Accepted or QuestState.Achieved
                || dominionHousing is QuestState.Accepted or QuestState.Achieved;

            if (!combatDone && !housingActive)
                return;

            player.Dismount();

            float x = HousingPlotPosition.X;
            float y = HousingPlotPosition.Y;
            float z = HousingPlotPosition.Z;
            float? terrainY = player.Map?.GetTerrainHeight(x, z);
            if (terrainY.HasValue)
                y = terrainY.Value;

            player.TeleportTo(3460, x, y, z);
        }
    }
}
