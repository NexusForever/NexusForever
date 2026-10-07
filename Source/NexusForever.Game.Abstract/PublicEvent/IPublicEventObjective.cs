using NexusForever.Game.Static.PublicEvent;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.PublicEvent
{
    public interface IPublicEventObjective : IUpdate, INetworkBuildable<PublicEventObjective>
    {
        IPublicEventTeam Team { get; }
        PublicEventObjectiveEntry Entry { get; }
        PublicEventStatus Status { get; }
        uint Count { get; }
        uint DynamicMax { get; }
        uint Checklist { get; }

        bool IsBusy { get; }

        /// <summary>
        /// Initialise <see cref="IPublicEventObjective"/> with suppled <see cref="IPublicEventTeam"/> and <see cref="PublicEventObjectiveEntry"/>.
        /// </summary>
        void Initialise(IPublicEventTeam team, PublicEventObjectiveEntry entry);

        /// <summary>
        /// Set busy state for the objective.
        /// </summary>
        /// <remarks>
        /// This will pause the objective preventing updates.
        /// </remarks>
        void SetBusy(bool busy);

        /// <summary>
        /// Update objective with the supplied count.
        /// </summary>
        void UpdateObjective(int count);

        /// <summary>
        /// Activate the objective.
        /// </summary>
        /// <remarks>
        /// This shows the objective to members and allows it to be updated.
        /// </remarks>
        void ActivateObjective(uint max);

        /// <summary>
        /// Set the dynamic max of an active objective, for example when participants join or leave.
        /// </summary>
        /// <remarks>
        /// The objective is completed immediately if the current count already meets the new max.
        /// </remarks>
        void SetDynamicMax(uint max);

        /// <summary>
        /// Reset the objective.
        /// </summary>
        /// <remarks>
        /// This will reset the objective to its initial state allowing it to be activated again.
        /// </remarks>
        void ResetObjective();
    }
}
