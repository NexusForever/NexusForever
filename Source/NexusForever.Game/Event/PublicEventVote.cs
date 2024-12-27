using NexusForever.Game.Abstract.Event;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Script.Template;
using NexusForever.Shared;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Event
{
    public class PublicEventVote : IPublicEventVote
    {
        public bool IsFinalised { get; private set; }
        public uint VoteId { get; private set; }

        private IPublicEventTeam publicEventTeam;
        private uint defaultChoice;

        private UpdateTimer expirationTimer;

        private readonly HashSet<uint> choices = [];
        private readonly Dictionary<ulong, IPublicEventVoteResponse> responses = [];

        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;
        private readonly IFactory<IPublicEventVoteResponse> responseFactory;

        public PublicEventVote(
            IGameTableManager gameTableManager,
            IFactory<IPublicEventVoteResponse> responseFactory)
        {
            this.gameTableManager = gameTableManager;
            this.responseFactory  = responseFactory;
        }

        #endregion

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (expirationTimer == null)
                return;

            expirationTimer.Update(lastTick);
            if (expirationTimer.HasElapsed)
            {
                expirationTimer = null;
                Finish();
            }
        }

        /// <summary>
        /// Initialise <see cref="PublicEventVote"/> with supplied voteId and defaultChoice.
        /// </summary>
        /// <remarks>
        /// Default choice will be selected if no response is received within the vote duration.
        /// </remarks>
        public void Initialise(IPublicEventTeam publicEventTeam, uint voteId, uint defaultChoice)
        {
            if (expirationTimer != null)
                throw new InvalidOperationException();

            PublicEventVoteEntry entry = gameTableManager.PublicEventVote.GetEntry(voteId);
            if (entry == null)
                throw new ArgumentOutOfRangeException();

            VoteId = voteId;

            this.publicEventTeam = publicEventTeam;
            this.defaultChoice   = defaultChoice;

            expirationTimer = new UpdateTimer(TimeSpan.FromMilliseconds(entry.DurationMS));

            // maybe move to template?
            for (uint i = 0; i < entry.LocalizedTextIdOption.Length; i++)
                if (entry.LocalizedTextIdOption[i] != 0)
                    choices.Add(i);

            foreach (IPublicEventTeamMember member in publicEventTeam.GetMembers())
            {
                IPublicEventVoteResponse response = responseFactory.Resolve();
                response.Initialise(member.CharacterId);
                responses.Add(member.CharacterId, response);
            }

            BroadcastVoteInitiate();
        }

        private void BroadcastVoteInitiate()
        {
            publicEventTeam.Broadcast(new ServerPublicEventVoteInitiate
            {
                EventId       = publicEventTeam.PublicEvent.Id,
                VoteId        = VoteId,
                TeamId        = publicEventTeam.Team,
                CanPlayerVote = true,
                ElapsedTimeMs = (uint)TimeSpan.FromSeconds(expirationTimer.Duration - expirationTimer.Time).TotalMilliseconds
            });
        }

        /// <summary>
        /// Respond to the vote with the supplied choice.
        /// </summary>
        public void Choice(ulong characterId, uint choice)
        {
            if (!responses.TryGetValue(characterId, out IPublicEventVoteResponse response))
                return;

            if (!choices.Contains(choice))
                return;

            response.SetChoice(choice);

            publicEventTeam.Broadcast(new ServerPublicEventVoteTally
            {
                EventId = publicEventTeam.PublicEvent.Id,
                VoteId  = VoteId,
                Choice  = choice
            });

            if (CanFinish())
                Finish();
        }

        private bool CanFinish()
        {
            return responses.Values.All(r => r.Choice.HasValue);
        }

        private void Finish()
        {
            if (IsFinalised)
                throw new InvalidOperationException();

            uint winningChoice = CalculateWinningChoice();

            publicEventTeam.Broadcast(new ServerPublicEventVoteEnd
            {
                EventId = publicEventTeam.PublicEvent.Id,
                VoteId  = VoteId,
                Winner  = winningChoice
            });

            publicEventTeam.PublicEvent.InvokeScriptCollection<IPublicEventScript>(s => s.OnVoteFinished(VoteId, winningChoice));

            IsFinalised = true;
        }

        private uint CalculateWinningChoice()
        {
            var tally = new Dictionary<uint, uint>();
            foreach (IPublicEventVoteResponse response in responses.Values)
            {
                if (!response.Choice.HasValue)
                    continue;

                if (tally.ContainsKey(response.Choice.Value))
                    tally[response.Choice.Value]++;
                else
                    tally.Add(response.Choice.Value, 1);
            }

            if (tally.Count == 0)
                return defaultChoice;

            // do we care about duplicates? this will return the first one
            return tally.MaxBy(p => p.Value).Key;
        }
    }
}
