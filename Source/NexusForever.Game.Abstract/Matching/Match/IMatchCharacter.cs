namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchCharacter
    {
        ulong CharacterId { get; }
        IMatchProposal MatchProposal { get; }
        IMatch Match { get; }

        /// <summary>
        /// Initialise <see cref="IMatchCharacter"/> with supplied character id.
        /// </summary>
        void Initialise(ulong characterId);

        /// <summary>
        /// Add the <see cref="IMatchProposal"/> for the character.
        /// </summary>
        void AddMatchProposal(IMatchProposal matchProposal);

        /// <summary>
        /// Remove the <see cref="IMatchProposal"/> for the character.
        /// </summary>
        void RemoveMatchProposal();

        /// <summary>
        /// Add the <see cref="IMatch"/> for the character.
        /// </summary>
        void AddMatch(IMatch match);

        /// <summary>
        /// Remove the <see cref="IMatch"/> for the character."/>
        /// </summary>
        void RemoveMatch();
    }
}
