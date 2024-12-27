using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Static.Matching;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Arena.TheSlaughterdome
{
    [ScriptFilterOwnerId(205)]
    public class TheSlaughterdomeEventScript : IPublicEventScript, IOwnedScript<IPublicEvent>
    {
        private IPublicEvent publicEvent;

        private readonly List<uint> doorEntities = [];

        /// <summary>
        /// Invoked when <see cref="IScript"/> is loaded.
        /// </summary>
        public void OnLoad(IPublicEvent owner)
        {
            publicEvent = owner;
            publicEvent.SetPhase(PublicEventPhase.Preperation);
        }

        /// <summary>
        /// Invoked when the public event phase changes.
        /// </summary>
        public void OnPublicEventPhase(uint phase)
        {
            switch ((PublicEventPhase)phase)
            {
                case PublicEventPhase.Fight:
                    PhaseFight();
                    break;
                case PublicEventPhase.Finished:
                    PhaseFinished();
                    break;
            }
        }

        private void PhaseFight()
        {
            foreach (uint doorGuid in doorEntities)
            {
                IDoorEntity door = publicEvent?.Map.GetEntity<IDoorEntity>(doorGuid);
                door?.OpenDoor();
            }
        }

        private void PhaseFinished()
        {
        }

        /// <summary>
        /// Invoked when a PvP match <see cref="PvpGameState"/> changes on the same map the public event is on. 
        /// </summary>
        public void OnMatchState(PvpGameState state)
        {
            switch (state)
            {
                case PvpGameState.InProgress:
                    publicEvent.SetPhase(PublicEventPhase.Fight);
                    break;
                case PvpGameState.Finished:
                    publicEvent.SetPhase(PublicEventPhase.Finished);
                    break;
            }
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
                case PublicEventCreature.PvpForcefieldDoor:
                case PublicEventCreature.PvpForcefieldDoor2:
                    doorEntities.Add(worldEntity.Guid);
                    break;
            }
        }

        public void OnDeath(IUnitEntity entity)
        {
            // TODO: resurrection timer and auto release
            // TODO: spawn flag
        }

        public void OnResurrection(IPlayer player)
        {
            // TODO: move to spawn position
        }
    }
}
