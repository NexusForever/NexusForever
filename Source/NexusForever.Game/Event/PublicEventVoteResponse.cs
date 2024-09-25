using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Event;

namespace NexusForever.Game.Event
{
    public class PublicEventVoteResponse : IPublicEventVoteResponse
    {
        public ulong CharacterId { get; private set; }
        public uint? Choice { get; private set; }

        #region Dependency Injection

        private readonly ILogger<PublicEventVoteResponse> log;

        public PublicEventVoteResponse(
            ILogger<PublicEventVoteResponse> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="PublicEventVoteResponse"/> for character.
        /// </summary>
        public void Initialise(ulong characterId)
        {
            if (CharacterId != 0)
                throw new InvalidOperationException();

            CharacterId = characterId;
        }

        /// <summary>
        /// Set response choice value.
        /// </summary>
        public void SetChoice(uint choice)
        {
            if (Choice.HasValue)
                throw new InvalidOperationException();

            Choice = choice;
        }
    }
}
