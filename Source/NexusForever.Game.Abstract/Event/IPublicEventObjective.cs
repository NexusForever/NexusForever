using NexusForever.Game.Static.Event;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventObjective : IUpdate, INetworkBuildable<PublicEventObjective>
    {
        IPublicEventTeam Team { get; }
        PublicEventObjectiveEntry Entry { get; }
        PublicEventStatus Status { get; }
        uint Count { get; }
        uint DynamicMax { get; set; }

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
        void ActivateObjective();
    }
}
