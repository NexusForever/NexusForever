using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;

namespace NexusForever.Game.Quest
{
    /// <summary>
    /// The client's communicator quest conditions (0x14043d870, 0x1405510d0), free of game state so the harness can run them.
    /// </summary>
    public static class CommunicatorConditions
    {
        /// <summary>
        /// Quest2 flag the client requires before a call may offer its quest ("can be called back"). Flag 0x10000 is
        /// the other branch of the same check (0x140552f50), but it governs turn-in calls for a quest the player holds,
        /// never a new offer, so it doesn't count here.
        /// </summary>
        public const uint CallbackFlag = 0x4u;

        /// <summary>
        /// The state the client compares: Mentioned, Abandoned and Ignored records read as no record.
        /// </summary>
        public static QuestState Effective(QuestState? raw)
        {
            return raw is null or QuestState.Unknown or QuestState.Mentioned or QuestState.Abandoned or QuestState.Ignored
                ? QuestState.Unknown
                : raw.Value;
        }

        /// <summary>
        /// Returns if a quest/state pair holds. States 0-4 compare the effective state, 5-10 need objective (state - 5)
        /// complete and 14-19 need objective (state - 14) not complete on an active record, 11 is "can be taken", 12 "in
        /// progress" and 13 "objectives done".
        /// </summary>
        public static bool Meets(uint tableState, CommunicatorQuestInfo? info, CommunicatorQuestSnapshot? record)
        {
            // A quest missing from Quest2 fails every state (0x14043d8a2).
            if (info is not CommunicatorQuestInfo quest)
                return false;

            bool native = record is { Native: true };
            byte completed = native ? record.Value.CompletedObjectives : (byte)0;
            bool Done(uint index) => index < quest.ObjectiveCount && (completed & (1u << (int)index)) != 0u;

            if (native && tableState is >= 5u and <= 10u)
                return Done(tableState - 5u);
            // An index past the objective count is never complete, so the pair holds.
            if (native && tableState is >= 14u and <= 19u)
                return !Done(tableState - 14u);

            QuestState effective = Effective(record?.Raw);
            return tableState switch
            {
                11u => effective is QuestState.Unknown or QuestState.Botched
                    || effective == QuestState.Completed && quest.RepeatableAfterComplete,
                12u => effective is QuestState.Accepted or QuestState.Achieved,
                13u => effective is QuestState.Achieved or QuestState.Completed,
                // 5-10 and 14-19 without an active record fail, as does anything from 20.
                _   => tableState <= 4u && (uint)effective == tableState
            };
        }

        /// <summary>
        /// Return why a call can't offer a quest, as the client's offer check sees it, or <see cref="CommunicatorOfferBlock.None"/>.
        /// </summary>
        public static CommunicatorOfferBlock Offer(QuestState? record, bool canStart, uint questFlags)
        {
            if (record != null)
                return CommunicatorOfferBlock.RecordExists;
            if (!canStart)
                return CommunicatorOfferBlock.CannotStart;
            if ((questFlags & CallbackFlag) == 0u)
                return CommunicatorOfferBlock.NoCallbackFlag;
            return CommunicatorOfferBlock.None;
        }

        public static bool CanOffer(QuestState? record, bool canStart, uint questFlags)
        {
            return Offer(record, canStart, questFlags) == CommunicatorOfferBlock.None;
        }

        /// <summary>
        /// Returns if one of <paramref name="rows"/> could be offering its quest to <paramref name="player"/> now: a row the
        /// client can receive whose conditions hold, skipping <paramref name="skip"/>.
        /// </summary>
        public static bool OfferedByCall(IEnumerable<ICommunicatorMessage> rows, ICommunicatorPlayerView player, CommunicatorGates skip)
        {
            return rows.Any(m => m.Reachable && m.Evaluate(player, skip) == CommunicatorResult.Eligible);
        }

        /// <summary>
        /// Returns if <paramref name="trigger"/> still holds for <paramref name="record"/>: the quest is still in the state,
        /// or the objective is still complete on an active record.
        /// </summary>
        public static bool TriggerHolds(CommunicatorTrigger trigger, CommunicatorQuestSnapshot? record)
        {
            if (trigger.Kind == CommunicatorTriggerKind.State)
                return (byte)Effective(record?.Raw) == trigger.Value;

            return record is { Native: true } snapshot && (snapshot.CompletedObjectives & (1 << trigger.Value)) != 0;
        }
    }
}
