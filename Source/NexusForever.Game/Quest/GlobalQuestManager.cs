using System.Collections.Immutable;
using System.Diagnostics;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Quest
{
    public sealed class GlobalQuestManager : Singleton<GlobalQuestManager>, IGlobalQuestManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// <see cref="DateTime"/> representing the next daily reset.
        /// </summary>
        public DateTime NextDailyReset { get; private set; }

        /// <summary>
        /// <see cref="DateTime"/> representing the next weekly reset.
        /// </summary>
        public DateTime NextWeeklyReset { get; private set; }

        private ImmutableDictionary<ushort, IQuestInfo> questInfoStore;
        private ImmutableDictionary<ushort, ImmutableList<uint>> questGiverStore;
        private ImmutableDictionary<ushort, ImmutableList<uint>> questReceiverStore;

        private ImmutableDictionary<uint, ICommunicatorMessage> communicatorStore;
        private ImmutableDictionary<ushort, ImmutableList<ICommunicatorMessage>> communicatorQuestStore;
        private ImmutableDictionary<(ushort /*questId*/, QuestState), ImmutableList<ICommunicatorMessage>> communicatorQuestStateTriggerStore;

        public void Initialise()
        {
            Stopwatch sw = Stopwatch.StartNew();

            CalculateResetTimes(); 
            InitialiseQuestInfo();
            InitialiseQuestRelations();

            InitialiseCommunicatorEntries();
            InitialiseCommunicatorQuests();
            InitialiseCommunicatorQuestStateTriggers();

            log.Info($"Cached {questInfoStore.Count} quests in {sw.ElapsedMilliseconds}ms.");
        }

        private void CalculateResetTimes()
        {
            DateTime now = DateTime.UtcNow;
            var resetTime = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0);

            // calculate daily reset (every day 10AM UTC)
            NextDailyReset = resetTime.AddDays(1);

            // calculate weekly reset (every tuesday 10AM UTC)
            NextWeeklyReset = resetTime.AddDays((DayOfWeek.Tuesday - now.DayOfWeek + 7) % 7);
        }

        private void InitialiseQuestInfo()
        {
            var builder = ImmutableDictionary.CreateBuilder<ushort, IQuestInfo>();
            foreach (Quest2Entry entry in GameTableManager.Instance.Quest2.Entries)
                builder.Add((ushort)entry.Id, new QuestInfo(entry));

            questInfoStore = builder.ToImmutable();
        }

        private void InitialiseQuestRelations()
        {
            var questGivers = new Dictionary<ushort, List<uint>>();
            var questReceivers = new Dictionary<ushort, List<uint>>();

            foreach (Creature2Entry entry in GameTableManager.Instance.Creature2.Entries)
            {
                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (ushort questId in entry.QuestIdGiven.Where(q => q != 0u))
                {
                    if (!questGivers.ContainsKey(questId))
                        questGivers.Add(questId, new List<uint>());

                    questGivers[questId].Add(entry.Id);
                }

                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (ushort questId in entry.QuestIdReceive.Where(q => q != 0u))
                {
                    if (!questReceivers.ContainsKey(questId))
                        questReceivers.Add(questId, new List<uint>());

                    questReceivers[questId].Add(entry.Id);
                }
            }

            questGiverStore = questGivers.ToImmutableDictionary(k => k.Key, v => v.Value.ToImmutableList());
            questReceiverStore = questReceivers.ToImmutableDictionary(k => k.Key, v => v.Value.ToImmutableList());
        }

        private void InitialiseCommunicatorEntries()
        {
            var builder = ImmutableDictionary.CreateBuilder<uint, ICommunicatorMessage>();
            foreach (CommunicatorMessagesEntry entry in GameTableManager.Instance.CommunicatorMessages.Entries)
            {
                var communicator = new CommunicatorMessage(entry);
                builder.Add(communicator.Id, communicator);
            }

            communicatorStore = builder.ToImmutable();
        }

        private void InitialiseCommunicatorQuests()
        {
            var builder = new Dictionary<ushort, List<ICommunicatorMessage>>();
            foreach (CommunicatorMessagesEntry entry in GameTableManager.Instance.CommunicatorMessages.Entries
                .Where(e => e.QuestIdDelivered != 0u))
            {
                ICommunicatorMessage communicator = communicatorStore[entry.Id];
                if (!builder.ContainsKey(communicator.QuestId))
                    builder.Add(communicator.QuestId, new List<ICommunicatorMessage>());

                builder[communicator.QuestId].Add(communicator);
            }

            communicatorQuestStore = builder.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
        }

        private void InitialiseCommunicatorQuestStateTriggers()
        {
            var builder = new Dictionary<(ushort, QuestState), List<ICommunicatorMessage>>();
            foreach (CommunicatorMessagesEntry entry in GameTableManager.Instance.CommunicatorMessages.Entries
                .Where(e => e.QuestIdDelivered != 0u))
            {
                foreach ((ushort QuestId, QuestState QuestState) p in
                    entry.Quests.Zip(entry.States, (a, b) => ((ushort)a, (QuestState)b)))
                {
                    if (p.QuestId == 0)
                        continue;

                    if (p.QuestId == entry.QuestIdDelivered)
                        continue;

                    if (!builder.ContainsKey(p))
                        builder.Add(p, new List<ICommunicatorMessage>());

                    ICommunicatorMessage communicator = communicatorStore[entry.Id];
                    builder[p].Add(communicator);
                }
            }

            communicatorQuestStateTriggerStore = builder.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
        }

        public void Update(double lastTick)
        {
            DateTime now = DateTime.UtcNow;
            if (NextDailyReset <= now)
                NextDailyReset = NextDailyReset.AddDays(1);

            if (NextWeeklyReset <= now)
                NextWeeklyReset = NextWeeklyReset.AddDays(7);
        }

        /// <summary>
        /// Return <see cref="IQuestInfo"/> for supplied quest.
        /// </summary>
        public IQuestInfo GetQuestInfo(ushort questId)
        {
            return questInfoStore.TryGetValue(questId, out IQuestInfo questInfo) ? questInfo : null;
        }

        /// <summary>
        /// Return a collection of creatures that start the supplied quest.
        /// </summary>
        public IEnumerable<uint> GetQuestGivers(ushort questId)
        {
            return questGiverStore.TryGetValue(questId, out ImmutableList<uint> creatureIds) ? creatureIds : Enumerable.Empty<uint>();
        }

        /// <summary>
        /// Return a collection of creatures that finish the supplied quest.
        /// </summary>
        public IEnumerable<uint> GetQuestReceivers(ushort questId)
        {
            return questReceiverStore.TryGetValue(questId, out ImmutableList<uint> creatureIds) ? creatureIds : Enumerable.Empty<uint>();
        }

        /// <summary>
        /// Return <see cref="ICommunicatorMessage"/> for supplied id.
        /// </summary>
        public ICommunicatorMessage GetCommunicatorMessage(uint id)
        {
            return communicatorStore.TryGetValue(id, out ICommunicatorMessage communicator) ? communicator : null;
        }

        /// <summary>
        /// Return <see cref="ICommunicatorMessage"/> for supplied id.
        /// </summary>
        public ICommunicatorMessage GetCommunicatorMessage<T>(T id) where T : Enum
        {
            return GetCommunicatorMessage(id.As<T, uint>());
        }

        /// <summary>
        /// Return a collection of <see cref="ICommunicatorMessage"/>'s that start the supplied quest.
        /// </summary>
        public IEnumerable<ICommunicatorMessage> GetQuestCommunicatorMessages(ushort questId)
        {
            return communicatorQuestStore.TryGetValue(questId, out ImmutableList<ICommunicatorMessage> creatureIds)
                ? creatureIds : Enumerable.Empty<ICommunicatorMessage>();
        }

        /// <summary>
        /// Return a collection of <see cref="ICommunicatorMessage"/>'s that are triggered when a quest hits a certain state.
        /// </summary>
        public IEnumerable<ICommunicatorMessage> GetQuestCommunicatorQuestStateTriggers(ushort questId, QuestState state)
        {
            return communicatorQuestStateTriggerStore.TryGetValue((questId, state), out ImmutableList<ICommunicatorMessage> triggers)
                ? triggers : Enumerable.Empty<ICommunicatorMessage>();
        }
    }
}
