namespace NexusForever.Game.Abstract.PublicEvent
{
    public interface IPublicEventVoteResponse
    {
        ulong CharacterId { get; }
        uint? Choice { get; }

        /// <summary>
        /// Initialise <see cref="IPublicEventVoteResponse"/> for character.
        /// </summary>
        void Initialise(ulong characterId);

        /// <summary>
        /// Set response choice value.
        /// </summary>
        void SetChoice(uint choice);
    }
}
