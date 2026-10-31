using System.Collections.Immutable;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Quest
{
    /// <summary>
    /// CommunicatorMessages classified once at startup, with the quest events that arm each scheduled row.
    /// </summary>
    /// <remarks>
    /// Pure, so the harness builds it straight from the tables. The trigger store holds only the rows a quest event can
    /// deliver: <see cref="CommunicatorKind.PushedScheduled"/> (pushed) and <see cref="CommunicatorKind.ClientScheduled"/>
    /// (mentioned; the client shows them itself).
    /// </remarks>
    public sealed class CommunicatorIndex
    {
        public ImmutableDictionary<uint, ICommunicatorMessage> Messages { get; }
        public ImmutableHashSet<uint> ChainTargets { get; }
        public ImmutableDictionary<ushort, ImmutableArray<CommunicatorArm>> Triggers { get; }
        public static readonly CommunicatorIndex Empty = new(ImmutableDictionary<uint, ICommunicatorMessage>.Empty,
            ImmutableHashSet<uint>.Empty, ImmutableDictionary<ushort, ImmutableArray<CommunicatorArm>>.Empty);

        private CommunicatorIndex(ImmutableDictionary<uint, ICommunicatorMessage> messages, ImmutableHashSet<uint> chainTargets,
            ImmutableDictionary<ushort, ImmutableArray<CommunicatorArm>> triggers)
        {
            Messages     = messages;
            ChainTargets = chainTargets;
            Triggers     = triggers;
        }

        /// <summary>
        /// Return the rows a quest event on <paramref name="questId"/> can arm.
        /// </summary>
        public ImmutableArray<CommunicatorArm> GetTriggers(ushort questId)
        {
            return Triggers.TryGetValue(questId, out ImmutableArray<CommunicatorArm> arms) ? arms : ImmutableArray<CommunicatorArm>.Empty;
        }

        /// <summary>
        /// Build the index from CommunicatorMessages rows, with <paramref name="quest"/> giving Quest2 data (null for a
        /// quest missing from Quest2).
        /// </summary>
        public static CommunicatorIndex Build(IEnumerable<CommunicatorMessagesEntry> rows, Func<ushort, CommunicatorQuestInfo?> quest)
        {
            CommunicatorMessagesEntry[] entries = rows.ToArray();
            ImmutableHashSet<uint> chainTargets = entries
                .Where(e => e.CommunicatorMessagesIdNext != 0u)
                .Select(e => e.CommunicatorMessagesIdNext)
                .ToImmutableHashSet();

            var messages = ImmutableDictionary.CreateBuilder<uint, ICommunicatorMessage>();
            var triggers = new Dictionary<ushort, List<CommunicatorArm>>();
            foreach (CommunicatorMessagesEntry entry in entries)
            {
                ImmutableArray<CommunicatorCondition> conditions = GetConditions(entry, quest);
                ImmutableArray<CommunicatorTrigger> rowTriggers = GetTriggers(entry, conditions);
                CommunicatorKind kind = Classify(entry, chainTargets.Contains(entry.Id), !rowTriggers.IsEmpty);

                var message = new CommunicatorMessage(entry, kind, conditions, rowTriggers);
                messages[entry.Id] = message;

                if (kind is not (CommunicatorKind.PushedScheduled or CommunicatorKind.ClientScheduled))
                    continue;

                foreach (CommunicatorTrigger trigger in rowTriggers)
                {
                    if (!triggers.TryGetValue(trigger.QuestId, out List<CommunicatorArm> arms))
                        triggers.Add(trigger.QuestId, arms = new List<CommunicatorArm>());
                    arms.Add(new CommunicatorArm(trigger, message));
                }
            }

            return new CommunicatorIndex(messages.ToImmutable(), chainTargets,
                triggers.ToImmutableDictionary(p => p.Key, p => p.Value.ToImmutableArray()));
        }

        /// <summary>
        /// Classify a row by flags bit 0, whether it is the next message of another row, whether it has triggers, and what
        /// it delivers.
        /// </summary>
        public static CommunicatorKind Classify(CommunicatorMessagesEntry entry, bool isChainTarget, bool hasTriggers)
        {
            bool delivers = entry.QuestIdDelivered != 0u;
            if ((entry.Flags & 0x1u) != 0u)
            {
                if (isChainTarget)
                    return CommunicatorKind.ChainTarget;
                if (hasTriggers)
                    return CommunicatorKind.PushedScheduled;
                return delivers ? CommunicatorKind.PushedUnconditioned : CommunicatorKind.PushedScript;
            }

            if (!delivers)
                return CommunicatorKind.ClientOnly;
            return hasTriggers ? CommunicatorKind.ClientScheduled : CommunicatorKind.ClientUnscheduled;
        }

        private static ImmutableArray<CommunicatorCondition> GetConditions(CommunicatorMessagesEntry entry, Func<ushort, CommunicatorQuestInfo?> quest)
        {
            var builder = ImmutableArray.CreateBuilder<CommunicatorCondition>();
            for (int i = 0; i < entry.Quests.Length && i < entry.States.Length; i++)
            {
                // Quest 0 always passes.
                ushort questId = (ushort)entry.Quests[i];
                if (questId != 0)
                    builder.Add(new CommunicatorCondition(questId, entry.States[i], quest(questId)));
            }

            return builder.ToImmutable();
        }

        private static ImmutableArray<CommunicatorTrigger> GetTriggers(CommunicatorMessagesEntry entry, ImmutableArray<CommunicatorCondition> conditions)
        {
            var builder = ImmutableArray.CreateBuilder<CommunicatorTrigger>();
            foreach (CommunicatorCondition condition in conditions)
            {
                // A pair on the delivered quest can't arm a call: the quest already has a record when it fires, so the
                // offer check fails. A quest missing from Quest2 never changes state.
                if (condition.Quest is not CommunicatorQuestInfo info || condition.QuestId == entry.QuestIdDelivered)
                    continue;

                if (condition.TableState is >= 1u and <= 4u)
                    builder.Add(new CommunicatorTrigger(condition.QuestId, CommunicatorTriggerKind.State, (byte)condition.TableState));
                else if (condition.TableState is >= 5u and <= 10u && condition.TableState - 5u < info.ObjectiveCount)
                    builder.Add(new CommunicatorTrigger(condition.QuestId, CommunicatorTriggerKind.Objective, (byte)(condition.TableState - 5u)));
            }

            return builder.ToImmutable();
        }
    }
}
