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

        private uint medbayDoorGuid;
        private uint securityChiefKondovichDoorGuid;
        private uint primaryPowerPlantDoorGuid;

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
                case PublicEventCreature.Door:
                    OnAddToMapDoor(worldEntity);
                    break;
            }
        }

        private void OnAddToMapDoor(IWorldEntity entity)
        {
            // multiple doors with the same creature id are used, so we need to differentiate them by their active prop id
            switch (entity.ActivePropId)
            {
                case 7024518:
                    primaryPowerPlantDoorGuid = entity.Guid;
                    break;
                case 7059788:
                    securityChiefKondovichDoorGuid = entity.Guid;
                    break;
                case 7059789:
                    medbayDoorGuid = entity.Guid;
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
                case PublicEventPhase.ActivateMedbayGenerator:
                    OnPhaseActivateMedbayGenerator();
                    break;
                case PublicEventPhase.GoToPrimaryPowerPlant:
                    OnPhaseGoToPrimaryPowerPlant();
                    break;
                case PublicEventPhase.KillSecurityChiefKondovich:
                    OnPhaseKillSecurityChiefKondovich();
                    break;
                case PublicEventPhase.GoToPrimaryPowerPlant2:
                    OnPhaseGoToPrimaryPowerPlant2();
                    break;
                case PublicEventPhase.RestartMainGenerators:
                    OnPhaseRestartMainGenerators();
                    break;
                case PublicEventPhase.EnterCrewQuarters:
                    OnPhaseEnterCrewQuarters();
                    break;
                case PublicEventPhase.DefeatEthericOrganisms:
                    OnPhaseDefeatEthericOrganisms();
                    break;
            }
        }

        private void OnPhaseGoToAirlock()
        {
            publicEvent.ActivateObjective(PublicEventObjective.GoToAirlock, mapInstance.PlayerCount);

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
            medbayDoorControl?.RemoveFromMap();
        }

        private void OnPhaseActivateMedbayGenerator()
        {
            publicEvent.ActivateObjective(PublicEventObjective.ActivateMedbayGenerator);

            IDoorEntity door = mapInstance.GetEntity<IDoorEntity>(medbayDoorGuid);
            door?.OpenDoor();
        }

        private void OnPhaseGoToPrimaryPowerPlant()
        {
            publicEvent.ActivateObjective(PublicEventObjective.GoToPrimaryPowerPlant, mapInstance.PlayerCount);
        }

        private void OnPhaseKillSecurityChiefKondovich()
        {
            publicEvent.ActivateObjective(PublicEventObjective.KillSecurityChiefKondovich);

            IDoorEntity door = mapInstance.GetEntity<IDoorEntity>(securityChiefKondovichDoorGuid);
            door?.OpenDoor();
        }

        private void OnPhaseGoToPrimaryPowerPlant2()
        {
            publicEvent.ResetObjective(PublicEventObjective.GoToPrimaryPowerPlant);
            publicEvent.ActivateObjective(PublicEventObjective.GoToPrimaryPowerPlant);
        }

        private void OnPhaseRestartMainGenerators()
        {
            publicEvent.ActivateObjective(PublicEventObjective.RestartMainGenerators);

            IDoorEntity door = mapInstance.GetEntity<IDoorEntity>(primaryPowerPlantDoorGuid);
            door?.OpenDoor();
        }

        private void OnPhaseEnterCrewQuarters()
        {
            publicEvent.ActivateObjective(PublicEventObjective.EnterCrewQuarters);
        }

        private void OnPhaseDefeatEthericOrganisms()
        {
            publicEvent.ActivateObjective(PublicEventObjective.DefeatEthericOrganisms);

            foreach (IPlayer player in mapInstance.GetPlayers())
                player.CinematicManager.QueueCinematic(cinematicFactory.CreateCinematic<IEvilFromTheEtherOnEthericOrganisms>());
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
                case PublicEventObjective.RepairDoor:
                    publicEvent.SetPhase(PublicEventPhase.ActivateMedbayGenerator);
                    break;
                case PublicEventObjective.ActivateMedbayGenerator:
                    publicEvent.SetPhase(PublicEventPhase.GoToPrimaryPowerPlant);
                    break;
                case PublicEventObjective.GoToPrimaryPowerPlant:
                {
                    // special case, seems this objective is used twice
                    // once to open the door to Security Chief Kondovich and once to open the door to the primary Power Plant
                    if (publicEvent.Phase == (uint)PublicEventPhase.GoToPrimaryPowerPlant)
                        publicEvent.SetPhase(PublicEventPhase.KillSecurityChiefKondovich);
                    else
                        publicEvent.SetPhase(PublicEventPhase.RestartMainGenerators);

                    break;
                }
                case PublicEventObjective.KillSecurityChiefKondovich:
                    publicEvent.SetPhase(PublicEventPhase.GoToPrimaryPowerPlant2);
                    break;
                case PublicEventObjective.RestartMainGenerators:
                    publicEvent.SetPhase(PublicEventPhase.EnterCrewQuarters);
                    break;
                case PublicEventObjective.EnterCrewQuarters:
                    publicEvent.SetPhase(PublicEventPhase.DefeatEthericOrganisms);
                    break;
            }
        }
    }
}
