using System.Collections.Immutable;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NLog;

namespace NexusForever.Game.Quest
{
    /// <summary>
    /// A player's communicator calls: quest events arm the rows that list them, and a row is delivered once all of
    /// its conditions hold, as the client checks them.
    /// </summary>
    /// <remarks>
    /// Events are queued and handled on the next update: a quest's state is set before it is filed, a mention would
    /// re-enter the state change, and the quest packets must reach the client before the call it re-checks against them.
    /// Like the quest manager that feeds it, the tracker is not thread safe.
    /// A row that fails a gate other than the offer waits, re-checked once a second, while its trigger still holds, for
    /// at most <see cref="PendingLifetime"/>. Nothing persists: the client keeps no once-only list either.
    /// </remarks>
    public sealed class CommunicatorTracker : ICommunicatorTracker
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public const double PollInterval = 1d;
        public const double PendingLifetime = 300d;
        private const int MaxRounds = 4;

        private readonly record struct Event(ushort QuestId, CommunicatorTriggerKind Kind, byte Value);
        private readonly record struct Pending(ICommunicatorMessage Message, CommunicatorTrigger Trigger, double Expires);

        public ICommunicatorPlayerView View { get; }

        /// <summary>
        /// Number of rows waiting for a gate.
        /// </summary>
        public int PendingCount => pending.Count;

        private readonly CommunicatorIndex index;
        private readonly Action<ICommunicatorMessage, string> deliver;

        private readonly List<Event> events = new();
        private readonly Dictionary<uint, Pending> pending = new();
        private readonly HashSet<uint> deliveredThisDrain = new();

        private double time;
        private double sincePoll;
        private bool draining;

        public CommunicatorTracker(ICommunicatorPlayerView view, CommunicatorIndex index, Action<ICommunicatorMessage, string> deliver)
        {
            View         = view;
            this.index   = index ?? CommunicatorIndex.Empty;
            this.deliver = deliver;
        }

        public void OnQuestState(ushort questId, QuestState oldState, QuestState newState, bool created = false)
        {
            // A new quest object starts Accepted (Achieved without objectives), so its first set would read
            // Accepted -> Accepted and arm nothing.
            QuestState from = CommunicatorConditions.Effective(created ? QuestState.Unknown : oldState);
            QuestState to = CommunicatorConditions.Effective(newState);
            if (from == to)
                return;

            // State 0 (which includes Mentioned, Abandoned and Ignored) arms nothing, but still re-checks waiting rows.
            events.Add(new Event(questId, CommunicatorTriggerKind.State, (byte)to));
        }

        public void OnObjectiveComplete(ushort questId, byte objectiveIndex)
        {
            events.Add(new Event(questId, CommunicatorTriggerKind.Objective, objectiveIndex));
        }

        // World-armed rows: a zone row is checked when the player enters a world or a zone (or levels up in it), a
        // level row only when a level-up crosses its minimum, so a login never pushes the calls of every level the player
        // has passed. A quest record (the delivered call mentions its quest) keeps each one to a single delivery.
        public void OnZone(uint worldZoneId)
        {
            QueueWorldCheck("zone", 0u, 0u);
        }

        public void OnLevel(uint oldLevel, uint newLevel)
        {
            if (newLevel > oldLevel)
                QueueWorldCheck("level", oldLevel, newLevel);
        }

        public void OnEnteredWorld()
        {
            QueueWorldCheck("world", 0u, 0u);
        }

        private string worldCheck;
        private uint levelFrom, levelTo;

        private void QueueWorldCheck(string reason, uint fromLevel, uint toLevel)
        {
            if (index.WorldArmed.IsEmpty)
                return;
            worldCheck = reason;
            if (toLevel > fromLevel)
            {
                levelFrom = levelTo == 0u ? fromLevel : Math.Min(levelFrom, fromLevel);
                levelTo = Math.Max(levelTo, toLevel);
            }
        }

        private void CheckWorldArmed(string reason)
        {
            (uint from, uint to) = (levelFrom, levelTo);
            levelFrom = levelTo = 0u;
            foreach (ICommunicatorMessage message in index.WorldArmed)
            {
                bool zoneRow = message.Entry.WorldZoneId != 0u;
                bool levelCrossed = message.Entry.MinLevel > from && message.Entry.MinLevel <= to;
                if (!zoneRow && !levelCrossed)
                    continue;
                if (deliveredThisDrain.Contains(message.Id) || View.GetQuest(message.QuestId) != null)
                    continue;
                if (message.Evaluate(View) == CommunicatorResult.Eligible)
                    Deliver(message, $"world-armed-{reason}");
            }
        }

        /// <summary>
        /// Handle queued events, and re-check waiting rows once a second.
        /// </summary>
        public void Update(double lastTick)
        {
            time += lastTick;
            sincePoll += lastTick;
            if (events.Count == 0 && worldCheck == null && (pending.Count == 0 || sincePoll < PollInterval))
                return;

            Drain();
        }

        private void Drain()
        {
            // A delivery runs quest code that can raise further events; the outer drain handles those.
            if (draining)
                return;

            draining = true;
            try
            {
                sincePoll = 0d;
                deliveredThisDrain.Clear();

                int round = 0;
                do
                {
                    Event[] batch = events.ToArray();
                    events.Clear();
                    foreach (Event e in batch)
                        Dispatch(e);

                    EvaluatePending();
                    if (worldCheck != null)
                    {
                        string reason = worldCheck;
                        worldCheck = null;
                        CheckWorldArmed(reason);
                    }
                    round++;
                }
                while (events.Count > 0 && round < MaxRounds);

            }
            finally
            {
                draining = false;
            }
        }

        private void Dispatch(Event e)
        {
            ImmutableArray<CommunicatorArm> arms = index.GetTriggers(e.QuestId);
            foreach (CommunicatorArm arm in arms)
            {
                if (arm.Trigger.Kind != e.Kind || arm.Trigger.Value != e.Value)
                    continue;

                string reason = e.Kind == CommunicatorTriggerKind.State ? $"state-{e.Value}" : $"objective-{e.Value}";
                Arm(arm.Message, arm.Trigger, reason);
            }
        }

        private void Arm(ICommunicatorMessage message, CommunicatorTrigger trigger, string reason)
        {
            if (deliveredThisDrain.Contains(message.Id))
                return;

            Trace(message, "armed", reason);

            // A quest that already has a record can never be offered again, so don't keep the row waiting.
            if (message.Delivers && View.GetQuest(message.QuestId) != null)
            {
                RefuseOffer(message);
                return;
            }

            CommunicatorResult result = message.Evaluate(View);
            switch (result)
            {
                case CommunicatorResult.Eligible:
                    Deliver(message, reason);
                    break;
                case CommunicatorResult.Offer:
                    RefuseOffer(message);
                    break;
                default:
                    pending[message.Id] = new Pending(message, trigger, time + PendingLifetime);
                    Trace(message, "pending", result.ToString());
                    break;
            }
        }

        private void EvaluatePending()
        {
            if (pending.Count == 0)
                return;

            foreach (Pending entry in pending.Values.ToArray())
            {
                ICommunicatorMessage message = entry.Message;
                if (time >= entry.Expires)
                {
                    pending.Remove(message.Id);
                    Trace(message, "pending-expired");
                    continue;
                }

                if (!CommunicatorConditions.TriggerHolds(entry.Trigger, View.GetQuest(entry.Trigger.QuestId)))
                {
                    pending.Remove(message.Id);
                    Trace(message, "pending-dropped");
                    continue;
                }

                if (deliveredThisDrain.Contains(message.Id))
                {
                    pending.Remove(message.Id);
                    continue;
                }

                CommunicatorResult result = message.Evaluate(View);
                if (result == CommunicatorResult.Eligible)
                    Deliver(message, $"pending-{entry.Trigger.Kind.ToString().ToLowerInvariant()}-{entry.Trigger.Value}");
                else if (result == CommunicatorResult.Offer)
                {
                    pending.Remove(message.Id);
                    RefuseOffer(message);
                }
            }
        }

        // An offer failure is final: the quest has a record, can't be started or can't be called back.
        private void RefuseOffer(ICommunicatorMessage message)
        {
            Trace(message, "offer-skip", OfferReason(View.CheckOffer(message.QuestId)));
        }

        private void Deliver(ICommunicatorMessage message, string reason)
        {
            pending.Remove(message.Id);
            deliveredThisDrain.Add(message.Id);

            deliver(message, reason);
        }

        private void Trace(ICommunicatorMessage message, string @event, string detail = "")
        {
            log.Trace($"Communicator {message.Id} (quest {message.QuestId}) {@event} {detail}".TrimEnd());
        }

        public static string OfferReason(CommunicatorOfferBlock block)
        {
            return block switch
            {
                CommunicatorOfferBlock.RecordExists   => "record-exists",
                CommunicatorOfferBlock.Disabled       => "disabled",
                CommunicatorOfferBlock.CannotStart    => "cannot-start",
                CommunicatorOfferBlock.NoCallbackFlag => "no-callback-flag",
                _                                     => "none"
            };
        }
    }
}
