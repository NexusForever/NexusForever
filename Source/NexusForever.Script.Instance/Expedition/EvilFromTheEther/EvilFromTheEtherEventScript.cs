using System.Numerics;
using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Game.Abstract.Cinematic.Cinematics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Trigger;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Static.Event;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Expedition.EvilFromTheEther
{
    [ScriptFilterOwnerId(781)]
    public class EvilFromTheEtherEventScript : IPublicEventScript, IOwnedScript<IPublicEvent>
    {
        private IPublicEvent publicEvent;
        private IMapInstance mapInstance;

        private uint medbayDoorControlGuid;

        #region Dependency Injection

        private readonly ICinematicFactory cinematicFactory;

        public EvilFromTheEtherEventScript(
            ICinematicFactory cinematicFactory)
        {
            this.cinematicFactory = cinematicFactory;
        }

        #endregion

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IPublicEvent owner)
        {
            publicEvent = owner;
            publicEvent.SetPhase(PublicEventPhase.Initial);

            mapInstance = publicEvent.Map as IMapInstance;
        }

        /// <summary>
        /// Invoked when a <see cref="IGridEntity"/> is added to the map the public event is on.
        /// </summary>
        public void OnAddToMap(IGridEntity entity)
        {
            if (entity is not IWorldEntity worldEntity)
                return;

            switch ((PublicEventCreature)worldEntity.CreatureId)
            {
                case PublicEventCreature.MedbayDoorControl:
                    medbayDoorControlGuid = worldEntity.Guid;
                    break;
            }
        }

        /// <summary>
        /// Invoked when the public event phase changes.
        /// </summary>
        public void OnPublicEventPhase(uint phase)
        {
            switch ((PublicEventPhase)phase)
            {
                case PublicEventPhase.GoToAirlock:
                    OnPhaseGoToAirlock();
                    break;
                case PublicEventPhase.OpenMedbay:
                    OnPhaseOpenMedbay();
                    break;
                case PublicEventPhase.ScavengeSpareParts:
                    OnPhaseScavengeSpareParts();
                    break;
                case PublicEventPhase.RepairDoor:
                    OnPhaseRepairDoor();
                    break;
            }
        }

        private void OnPhaseGoToAirlock()
        {
            uint playerCount = mapInstance.PlayerCount;
            publicEvent.ActivateObjective(PublicEventObjective.GoToAirlock, playerCount);

            var triggerEntity = publicEvent.CreateEntity<IWorldLocationTriggerEntity>();
            triggerEntity.Initialise(50278);
            mapInstance.EnqueueAdd(triggerEntity, new Vector3(-396.43555f, -840.7188f, 119.74138f));
        }

        private void OnPhaseOpenMedbay()
        {
            publicEvent.ActivateObjective(PublicEventObjective.OpenMedbay);
            publicEvent.ActivateObjective(PublicEventObjective.DownloadCrewLogs);

            foreach (IPlayer player in mapInstance.GetPlayers())
                player.TeleportToLocal(new Vector3(53.180374f, -852.86273f, -91.41684f));
        }

        private void OnPhaseScavengeSpareParts()
        {
            publicEvent.ActivateObjective(PublicEventObjective.ScavengeSpareParts);

            foreach (IPlayer player in mapInstance.GetPlayers())
                player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<IEvilFromTheEtherOnOpenMedbay>());
        }

        private void OnPhaseRepairDoor()
        {
            publicEvent.ActivateObjective(PublicEventObjective.RepairDoor);

            IWorldEntity medbayDoorControl = mapInstance.GetEntity<IWorldEntity>(medbayDoorControlGuid);
            medbayDoorControl.RemoveFromMap();
        }

        /// <summary>
        /// Invoked when the <see cref="IPublicEventObjective"/> status changes.
        /// </summary>
        public void OnPublicEventObjectiveStatus(IPublicEventObjective objective)
        {
            if (objective.Status != PublicEventStatus.Succeeded)
                return;

            switch ((PublicEventObjective)objective.Entry.Id)
            {
                case PublicEventObjective.TalkToCaptainWeir:
                    publicEvent.SetPhase(PublicEventPhase.GoToAirlock);
                    break;
                case PublicEventObjective.GoToAirlock:
                    publicEvent.SetPhase(PublicEventPhase.OpenMedbay);
                    break;
                case PublicEventObjective.OpenMedbay:
                    publicEvent.SetPhase(PublicEventPhase.ScavengeSpareParts);
                    break;
                case PublicEventObjective.ScavengeSpareParts:
                    publicEvent.SetPhase(PublicEventPhase.RepairDoor);
                    break;
            }
        }
    }
}
