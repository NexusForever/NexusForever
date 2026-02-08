using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Matching.Match;

namespace NexusForever.Game.Matching.Match
{
    public class MatchCharacter : IMatchCharacter
    {
        public Identity Identity { get; private set; }
        public IMatchProposal MatchProposal { get; private set; }
        public IMatch Match { get; private set; }

        #region Dependency Injection

        private readonly ILogger<MatchCharacter> log;

        public MatchCharacter(
            ILogger<MatchCharacter> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchCharacter"/> with supplied character id.
        /// </summary>
        public void Initialise(Identity identity)
        {
            if (Identity != null)
                throw new InvalidOperationException();

            Identity = identity;

            log.LogTrace($"Matching information initialised for character {identity}.");
        }

        /// <summary>
        /// Add the <see cref="IMatchProposal"/> for the character.
        /// </summary>
        public void AddMatchProposal(IMatchProposal matchProposal)
        {
            if (MatchProposal != null)
                throw new InvalidOperationException();

            MatchProposal = matchProposal;

            log.LogTrace($"Match proposal added for character {Identity}.");
        }

        /// <summary>
        /// Remove the <see cref="IMatchProposal"/> for the character.
        /// </summary>
        public void RemoveMatchProposal()
        {
            if (MatchProposal == null)
                throw new InvalidOperationException();

            MatchProposal = null;

            log.LogTrace($"Match proposal removed for character {Identity}.");
        }

        /// <summary>
        /// Add the <see cref="IMatch"/> for the character.
        /// </summary>
        public void AddMatch(IMatch match)
        {
            if (Match != null)
                throw new InvalidOperationException();

            Match = match;

            log.LogTrace($"Match added for character {Identity}.");
        }

        /// <summary>
        /// Remove the <see cref="IMatch"/> for the character."/>
        /// </summary>
        public void RemoveMatch()
        {
            if (Match == null)
                throw new InvalidOperationException();

            Match = null;

            log.LogTrace($"Match removed for character {Identity}.");
        }
    }
}
