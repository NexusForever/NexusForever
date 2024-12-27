using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventTemplate
    {
        PublicEventEntry Entry { get; }
        Dictionary<uint, PublicEventObjectiveEntry> Objectives { get; }
        List<PublicEventTeamEntry> Teams { get; }
        List<PublicEventCustomStatEntry> CustomStats { get; }

        /// <summary>
        /// Initialise <see cref="IPublicEventTemplate"/> with <see cref="PublicEventEntry"/>.
        /// </summary>
        void Initialise(PublicEventEntry entry);

        /// <summary>
        /// Returns if event has live stats.
        /// </summary>
        /// <remarks>
        /// This determines if live stats should be periodically sent to members of the event.
        /// </remarks>
        bool HasLiveStats();
    }
}
