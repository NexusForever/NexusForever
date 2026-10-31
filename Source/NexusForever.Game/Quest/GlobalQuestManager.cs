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

        /// <summary>
        /// Communicator messages classified with the quest events that arm them; each player's tracker reads it.
        /// </summary>
        public CommunicatorIndex CommunicatorIndex { get; private set; } = CommunicatorIndex.Empty;

        private ImmutableDictionary<ushort, ImmutableList<ICommunicatorMessage>> communicatorQuestStore;

        public void Initialise()
        {
            Stopwatch sw = Stopwatch.StartNew();

            CalculateResetTimes(); 
            InitialiseQuestInfo();
            InitialiseQuestRelations();

            InitialiseCommunicatorEntries();
            InitialiseCommunicatorQuests();

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
            CommunicatorIndex = CommunicatorIndex.Build(GameTableManager.Instance.CommunicatorMessages.Entries, GetCommunicatorQuestInfo);
            log.Info($"Cached {CommunicatorIndex.Messages.Count} communicator messages, armed by events on {CommunicatorIndex.Triggers.Count} quests.");
        }

        private CommunicatorQuestInfo? GetCommunicatorQuestInfo(ushort questId)
        {
            IQuestInfo info = GetQuestInfo(questId);
            if (info == null)
                return null;

            bool repeatable = (info.Entry.Flags & 0x20u) != 0u || info.Entry.QuestRepeatPeriodEnum != 0u;
            return new CommunicatorQuestInfo((byte)info.Objectives.Count, repeatable, info.Entry.Flags);
        }

        private void InitialiseCommunicatorQuests()
        {
            var builder = new Dictionary<ushort, List<ICommunicatorMessage>>();
            foreach (CommunicatorMessagesEntry entry in GameTableManager.Instance.CommunicatorMessages.Entries
                .Where(e => e.QuestIdDelivered != 0u))
            {
                ICommunicatorMessage communicator = CommunicatorIndex.Messages[entry.Id];
                if (!builder.ContainsKey(communicator.QuestId))
                    builder.Add(communicator.QuestId, new List<ICommunicatorMessage>());

                builder[communicator.QuestId].Add(communicator);
            }

            communicatorQuestStore = builder.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
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
        /// Return a collection of <see cref="ICommunicatorMessage"/>'s that start the supplied quest.
        /// </summary>
        public IEnumerable<ICommunicatorMessage> GetQuestCommunicatorMessages(ushort questId)
        {
            return communicatorQuestStore.TryGetValue(questId, out ImmutableList<ICommunicatorMessage> creatureIds)
                ? creatureIds : Enumerable.Empty<ICommunicatorMessage>();
        }

        /// <summary>
        /// Returns if any communicator message starts the supplied quest.
        /// </summary>
        public bool HasQuestCommunicatorMessages(ushort questId)
        {
            return communicatorQuestStore.ContainsKey(questId);
        }
    }
}
