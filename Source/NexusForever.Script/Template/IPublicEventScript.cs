using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Static.Event;
using NexusForever.Game.Static.Matching;
using NexusForever.Shared;

namespace NexusForever.Script.Template
{
    public interface IPublicEventScript : IUpdate
    {
        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        void IUpdate.Update(double lastTick)
        {
        }

        /// <summary>
        /// Invoked when the public event <see cref="PublicEventStatus"/> changes.
        /// </summary>
        void OnPublicEventStatus(PublicEventStatus status)
        {
        }

        /// <summary>
        /// Invoked when the public event phase changes.
        /// </summary>
        void OnPublicEventPhase(uint phase)
        {
        }

        /// <summary>
        /// Invoked when the <see cref="IPublicEventObjective"/> status changes.
        /// </summary>
        void OnPublicEventObjectiveStatus(IPublicEventObjective objective)
        {
        }

        /// <summary>
        /// Invoked when a <see cref="IGridEntity"/> is added to the map the public event is on.
        /// </summary>
        void OnAddToMap(IGridEntity entity)
        {
        }

        /// <summary>
        /// Invoked when a <see cref="IGridEntity"/> is removed from the map the public event is on.
        /// </summary>
        void OnRemoveFromMap(IGridEntity entity)
        {
        }

        /// <summary>
        /// Invoked when a PvP match <see cref="PvpGameState"/> changes on the same map the public event is on. 
        /// </summary>
        /// <remarks>
        /// This is useful to sync state between matches and their respective public events.
        /// </remarks>
        void OnMatchState(PvpGameState state)
        {

        }

        /// <summary>
        /// Invoked when a vote on the public event has finished.
        /// </summary>
        void OnVoteFinished(uint voteId, uint winner)
        {
        }
    }
}
