using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventVote : IUpdate
    {
        bool IsFinalised { get; }
        uint VoteId { get; }

        /// <summary>
        /// Initialise <see cref="IPublicEventVote"/> with supplied voteId and defaultChoice.
        /// </summary>
        /// <remarks>
        /// Default choice will be selected if no response is received within the vote duration.
        /// </remarks>
        void Initialise(IPublicEventTeam publicEventTeam, uint voteId, uint defaultChoice);

        /// <summary>
        /// Respond to the vote with the supplied choice.
        /// </summary>
        void Choice(ulong characterId, uint choice);
    }
}
