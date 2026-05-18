using NexusForever.Game.Static.Quest;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Quest
{
    public interface IGlobalQuestManager : IUpdate
    {
        /// <summary>
        /// <see cref="DateTime"/> representing the next daily reset.
        /// </summary>
        DateTime NextDailyReset { get; }

        /// <summary>
        /// <see cref="DateTime"/> representing the next weekly reset.
        /// </summary>
        DateTime NextWeeklyReset { get; }

        void Initialise();

        /// <summary>
        /// Return <see cref="IQuestInfo"/> for supplied quest.
        /// </summary>
        IQuestInfo GetQuestInfo(ushort questId);

        /// <summary>
        /// Return a collection of creatures that start the supplied quest.
        /// </summary>
        IEnumerable<uint> GetQuestGivers(ushort questId);

        /// <summary>
        /// Return a collection of creatures that finish the supplied quest.
        /// </summary>
        IEnumerable<uint> GetQuestReceivers(ushort questId);

        /// <summary>
        /// Return a collection of <see cref="ICommunicatorMessage"/>'s that start the supplied quest.
        /// </summary>
        IEnumerable<ICommunicatorMessage> GetQuestCommunicatorMessages(ushort questId);

        /// <summary>
        /// Return a collection of <see cref="ICommunicatorMessage"/>'s that are triggered when a quest hits a certain state.
        /// </summary>
        IEnumerable<ICommunicatorMessage> GetQuestCommunicatorQuestStateTriggers(ushort questId, QuestState state);
    }
}