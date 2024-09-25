using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Static.Event;
using NexusForever.Game.Static.Matching;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Instance.Arena.TheSlaughterdome
{
    [ScriptFilterOwnerId(209)]
    public class TheSlaughterdomeSubEventScript : IPublicEventScript, IOwnedScript<IPublicEvent>
    {
        private IPublicEvent publicEvent;

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
                    publicEvent.ActivateObjective(PublicEventObjective.ParticipateInArena);
                    break;
            }
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
                case PublicEventObjective.PrepareForBattle:
                    publicEvent.SetPhase(PublicEventPhase.Fight);
                    break;
            }
        }

        /// <summary>
        /// Invoked when a PvP match <see cref="PvpGameState"/> changes on the same map the public event is on. 
        /// </summary>
        public void OnMatchState(PvpGameState state)
        {
            switch (state)
            {
                case PvpGameState.InProgress:
                    publicEvent.UpdateObjective(PublicEventObjective.PrepareForBattle, 0);
                    break;
            }
        }
    }
}
