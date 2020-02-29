using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Achievement.Static;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Prerequisite;
using NexusForever.WorldServer.Game.Quest;
using NexusForever.WorldServer.Game.Quest.Static;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class QuestManager : ISaveCharacter, IUpdate
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        [Flags]
        private enum GetQuestFlags
        {
            Completed = 0x01,
            Inactive  = 0x02,
            Active    = 0x04,
            All       = Completed | Inactive | Active
        }

        private readonly Player player;

        private readonly Dictionary<ushort, Quest.Quest> completedQuests = new Dictionary<ushort, Quest.Quest>();
        private readonly Dictionary<ushort, Quest.Quest> inactiveQuests = new Dictionary<ushort, Quest.Quest>();
        private readonly Dictionary<ushort, Quest.Quest> activeQuests = new Dictionary<ushort, Quest.Quest>();

        /// <summary>
        /// Create a new <see cref="QuestManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public QuestManager(Player owner, CharacterModel model)
        {
            player = owner;

            foreach (CharacterQuestModel questModel in model.Quest)
            {
                QuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questModel.QuestId);
                if (info == null)
                {
                    log.Error($"Player {player.CharacterId} has an invalid quest {questModel.QuestId}!");
                    continue;
                }

                var quest = new Quest.Quest(player, info, questModel);
                switch (quest.State)
                {
                    case QuestState.Completed:
                        completedQuests.Add(quest.Id, quest);
                        break;
                    case QuestState.Botched:
                    case QuestState.Ignored:
                        inactiveQuests.Add(quest.Id, quest);
                        break;
                    case QuestState.Accepted:
                    case QuestState.Achieved:
                        activeQuests.Add(quest.Id, quest);
                        break;
                }
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (Quest.Quest quest in completedQuests.Values)
                quest.Save(context);

            foreach (Quest.Quest quest in inactiveQuests.Values.ToList())
            {
                if (quest.PendingDelete)
                    inactiveQuests.Remove(quest.Id);

                quest.Save(context);
            }

            foreach (Quest.Quest quest in activeQuests.Values.ToList())
            {
                if (quest.PendingDelete)
                    activeQuests.Remove(quest.Id);

                quest.Save(context);
            }
        }

        public void Update(double lastTick)
        {
            var botchedQuests = new List<Quest.Quest>();
            foreach (Quest.Quest quest in activeQuests.Values)
            {
                quest.Update(lastTick);
                if (quest.State == QuestState.Botched)
                    botchedQuests.Add(quest);
            }

            foreach (Quest.Quest quest in botchedQuests)
            {
                activeQuests.Remove(quest.Id);
                inactiveQuests.Add(quest.Id, quest);

                log.Trace($"Failed to complete quest {quest.Id} before the timer expired!");
            }
        }

        public void SendInitialPackets()
        {
            DateTime now = DateTime.UtcNow;
            player.Session.EnqueueMessageEncrypted(new ServerQuestInit
            {
                Completed = completedQuests.Values
                    .Select(q => new ServerQuestInit.QuestComplete
                    {
                        QuestId        = q.Id,
                        CompletedToday = now < q.Reset
                    }).ToList(),
                Inactive = inactiveQuests.Values
                    .Select(q => new ServerQuestInit.QuestInactive
                    {
                        QuestId = q.Id,
                        State   = q.State
                    }).ToList(),
                Active = activeQuests.Values
                    .Select(q => new ServerQuestInit.QuestActive
                    {
                        QuestId    = q.Id,
                        State      = q.State,
                        Flags      = q.Flags,
                        Timer      = q.Timer ?? 0u,
                        Objectives = q.Select(o => new ServerQuestInit.QuestActive.Objective
                        {
                            Progress = o.Progress,
                            Timer    = 0u
                        }).ToList()
                    }).ToList()
            });
        }

        /// <summary>
        /// Return <see cref="QuestState"/> for supplied quest.
        /// </summary>
        public QuestState? GetQuestState(ushort questId)
        {
            return GetQuest(questId)?.State;
        }

        private Quest.Quest GetQuest(ushort questId, GetQuestFlags flags = GetQuestFlags.All)
        {
            if ((flags & GetQuestFlags.Active) != 0
                && activeQuests.TryGetValue(questId, out Quest.Quest quest))
                return quest;

            if ((flags & GetQuestFlags.Inactive) != 0
                && inactiveQuests.TryGetValue(questId, out quest))
                return quest;

            if ((flags & GetQuestFlags.Completed) != 0
                && completedQuests.TryGetValue(questId, out quest))
                return quest;

            return null;
        }

        /// <summary>
        /// Add a <see cref="Quest"/> from supplied id, optionally supplying <see cref="Item"/> which was used to start the quest.
        /// </summary>
        public void QuestAdd(ushort questId, Item item)
        {
            QuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            if (DisableManager.Instance.IsDisabled(DisableType.Quest, questId))
            {
                player.SendSystemMessage($"Unable to add quest {questId} because it is disabled.");
                return;
            }

            Quest.Quest quest = GetQuest(questId);
            QuestAdd(info, quest, item);
        }

        private void QuestAdd(QuestInfo info, Quest.Quest quest, Item item)
        {
            if (quest?.State == QuestState.Accepted || quest?.State == QuestState.Achieved)
                throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} which is already in progress!");

            // if quest has already been completed make sure it's repeatable and the reset period has elapsed
            if (quest?.State == QuestState.Completed)
            {
                if (info.Entry.QuestRepeatPeriodEnum == 0u)
                    throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} which they have already completed!");

                DateTime? resetTime = GetQuest((ushort)info.Entry.Id, GetQuestFlags.Completed).Reset;
                if (DateTime.UtcNow < resetTime)
                    throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} which hasn't reset yet!");
            }

            if (item != null)
            {
                if (info.Entry.Id != item.Entry.Quest2IdActivation)
                    throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} from invalid item {item.Entry.Id}!");

                // TODO: consume charge
            }
            else
            {
                // make sure the player is in range of a quest giver or they are eligible for a communicator message that starts the quest
                if (!GlobalQuestManager.Instance.GetQuestGivers((ushort)info.Entry.Id)
                    .Any(c => player.GetVisibleCreature<WorldEntity>(c).Any()))
                {
                    if (!info.IsCommunicatorReceived())
                        throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} without quest giver!");

                    if (!GlobalQuestManager.Instance.GetQuestCommunicatorMessages((ushort)info.Entry.Id)
                        .Any(m => m.Meets(player)))
                        throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} without communicator message!");
                }
            }

            // server doesn't send an error message for prerequisites since the client does the exact same checks
            // it's assumed that a player could never get here without cheating in some way
            if (!MeetsPrerequisites(info))
                throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} without meeting the prerequisites!");

            QuestAdd(info);
        }

        private bool MeetsPrerequisites(QuestInfo info)
        {
            if (info.Entry.QuestPlayerFactionEnum == 0u && player.Faction1 != Faction.Exile)
                return false;
            if (info.Entry.QuestPlayerFactionEnum == 1u && player.Faction1 != Faction.Dominion)
                return false;
            if (info.Entry.PrerequisiteRace != 0u && player.Race != (Race)info.Entry.PrerequisiteRace)
                return false;
            if (info.Entry.PrerequisiteClass != 0u && player.Class != (Class)info.Entry.PrerequisiteClass)
                return false;
            if (player.Level < info.Entry.PrerequisiteLevel)
                return false;

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (ushort questId in info.Entry.PrerequisiteQuests.Where(q => q != 0u))
                if (GetQuestState(questId) != QuestState.Completed)
                    return false;

            if (info.Entry.PrerequisiteId != 0u && !PrerequisiteManager.Instance.Meets(player, info.Entry.PrerequisiteId))
                return false;

            if (!info.IsContract())
            {
                GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(655);
                // client also hard codes 40 if entry doesn't exist
                if (completedQuests.Count > (entry?.Dataint0 ?? 40u))
                    return false;
            }
            else
            {
                // TODO: contracts use reward property for max slots, RewardProperty.ActiveContractSlots
            }

            return true;
        }

        /// <summary>
        /// Add a <see cref="Quest"/> from supplied <see cref="QuestInfo"/>, skipping any prerequisites checks.
        /// </summary>
        public void QuestAdd(QuestInfo info)
        {
            // make sure player has room for all pushed items
            if (player.Inventory.GetInventoryFreeBagIndexCount()
                < info.Entry.PushedItemIds.Count(i => i != 0u))
            {
                player.SendGenericError(GenericError.ItemInventoryFull);
                return;
            }

            for (int i = 0; i < info.Entry.PushedItemIds.Length; i++)
            {
                uint itemId = info.Entry.PushedItemIds[i];
                if (itemId != 0u)
                    player.Inventory.ItemCreate(itemId, info.Entry.PushedItemCounts[i]);
            }

            // TODO: virtual items

            Quest.Quest quest = GetQuest((ushort)info.Entry.Id);
            if (quest == null)
                quest = new Quest.Quest(player, info);
            else
            {
                // remove existing quest from its current home before
                switch (quest.State)
                {
                    case QuestState.Abandoned:
                        activeQuests.Remove(quest.Id);
                        break;
                    case QuestState.Completed:
                        completedQuests.Remove(quest.Id);
                        break;
                    case QuestState.Botched:
                    case QuestState.Ignored:
                        inactiveQuests.Remove(quest.Id);
                        break;
                }

                if (quest.PendingDelete)
                    quest.EnqueueDelete(false);

                // reset previous objective progress
                foreach (QuestObjective objective in quest)
                    objective.Progress = 0u;
            }

            quest.State = QuestState.Accepted;
            activeQuests.Add((ushort)info.Entry.Id, quest);

            quest.InitialiseTimer();

            log.Trace($"Accepted new quest {info.Entry.Id}.");
        }

        /// <summary>
        /// Retry an inactive <see cref="Quest"/> that was previously failed.
        /// </summary>
        public void QuestRetry(ushort questId)
        {
            QuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            Quest.Quest quest = GetQuest(questId, GetQuestFlags.Inactive);
            if (quest == null)
                throw new QuestException($"Player {player.CharacterId} tried to restart quest {questId} which they don't have!");

            if (quest.State != QuestState.Botched)
                throw new QuestException($"Player {player.CharacterId} tried to restart quest {questId} which hasn't been failed!");

            QuestAdd(info, quest, null);
        }

        /// <summary>
        /// Abandon an active <see cref="Quest"/>.
        /// </summary>
        public void QuestAbandon(ushort questId)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            Quest.Quest quest = GetQuest(questId, GetQuestFlags.Active | GetQuestFlags.Inactive);
            if (quest == null || quest.PendingDelete)
                throw new QuestException($"Player {player.CharacterId} tried to abandon quest {questId} which they don't have!");

            if (!quest.CanAbandon())
                throw new QuestException($"Player {player.CharacterId} tried to abandon quest {questId} which can't be abandoned!");

            if (!quest.PendingCreate)
                quest.EnqueueDelete(true);
            else
            {
                switch (quest.State)
                {
                    case QuestState.Accepted:
                    case QuestState.Achieved:
                        activeQuests.Remove(questId);
                        break;
                    case QuestState.Botched:
                        inactiveQuests.Remove(quest.Id);
                        break;
                }
            }

            quest.State = QuestState.Abandoned;

            log.Trace($"Abandoned quest {questId}.");
        }

        /// <summary>
        /// Complete all <see cref="QuestObjective"/>'s for supplied active <see cref="Quest"/>.
        /// </summary>
        public void QuestAchieve(ushort questId)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            Quest.Quest quest = GetQuest(questId);
            if (quest == null || quest.PendingDelete)
                throw new QuestException($"Player {player.CharacterId} tried to achieve quest {questId} which they don't have!");

            if (quest.State != QuestState.Accepted)
                throw new QuestException($"Player {player.CharacterId} tried to achieve quest {questId} with invalid state!");

            foreach (QuestObjectiveEntry entry in quest.Info.Objectives)
                quest.ObjectiveUpdate((QuestObjectiveType)entry.Type, entry.Data, entry.Count);
        }

        /// <summary>
        /// Complete single <see cref="QuestObjective"/> for supplied active <see cref="Quest"/>.
        /// </summary>
        public void QuestAchieveObjective(ushort questId, byte index)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            Quest.Quest quest = GetQuest(questId);
            if (quest == null || quest.PendingDelete)
                throw new QuestException();

            if (quest.State != QuestState.Accepted)
                throw new QuestException();

            QuestObjective objective = quest.SingleOrDefault(o => o.Index == index);
            if (objective == null)
                throw new QuestException();

            quest.ObjectiveUpdate((QuestObjectiveType)objective.Entry.Type, objective.Entry.Data, objective.Entry.Count);
        }

        /// <summary>
        /// Complete an achieved <see cref="Quest"/> supplying an optional reward and whether the quest was completed from the communicator.
        /// </summary>
        public void QuestComplete(ushort questId, ushort reward, bool communicator)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            if (DisableManager.Instance.IsDisabled(DisableType.Quest, questId))
            {
                player.SendSystemMessage($"Unable to complete quest {questId} because it is disabled.");
                return;
            }

            Quest.Quest quest = GetQuest(questId, GetQuestFlags.Active);
            if (quest == null)
                throw new QuestException($"Player {player.CharacterId} tried to complete quest {questId} which they don't have!");

            if (quest.State != QuestState.Achieved)
                throw new QuestException($"Player {player.CharacterId} tried to complete quest {questId} which wasn't complete!");

            if (communicator)
            {
                if (!quest.Info.IsCommunicatorReceived())
                    throw new QuestException($"Player {player.CharacterId} tried to complete quest {questId} without communicator message!");
            }
            else
            {
                if (!GlobalQuestManager.Instance.GetQuestReceivers(questId).Any(c => player.GetVisibleCreature<WorldEntity>(c).Any()))
                    throw new QuestException($"Player {player.CharacterId} tried to complete quest {questId} without any quest receiver!");
            }

            // reclaim any quest specific items
            for (int i = 0; i < quest.Info.Entry.PushedItemIds.Length; i++)
            {
                uint itemId = quest.Info.Entry.PushedItemIds[i];
                if (itemId != 0u)
                    player.Inventory.ItemDelete(itemId, quest.Info.Entry.PushedItemCounts[i]);
            }

            RewardQuest(quest.Info, reward);
            quest.State = QuestState.Completed;

            // mark repeatable quests for reset
            switch ((QuestRepeatPeriod)quest.Info.Entry.QuestRepeatPeriodEnum)
            {
                case QuestRepeatPeriod.Daily:
                    quest.Reset = GlobalQuestManager.Instance.NextDailyReset;
                    break;
                case QuestRepeatPeriod.Weekly:
                    quest.Reset = GlobalQuestManager.Instance.NextWeeklyReset;
                    break;
            }

            activeQuests.Remove(questId);
            completedQuests.Add(questId, quest);

            player.AchievementManager.CheckAchievements(player, AchievementType.QuestComplete, questId);
        }

        private void RewardQuest(QuestInfo info, ushort reward)
        {
            if (reward != 0)
            {
                if (!info.Rewards.TryGetValue(reward, out Quest2RewardEntry entry))
                    throw new QuestException($"Player {player.CharacterId} tried to complete quest {info.Entry.Id} with invalid reward!");

                // TODO: make sure reward is valid for player, some rewards are conditional

                RewardQuest(entry);
            }

            // TODO: fixed rewards

            uint experience = info.GetRewardExperience();
            if (experience != 0u)
            {
                // TODO: reward experience
            }

            uint money = info.GetRewardMoney();
            if (money != 0u)
                player.CurrencyManager.CurrencyAddAmount(CurrencyType.Credits, money);
        }

        private void RewardQuest(Quest2RewardEntry entry)
        {
            switch ((QuestRewardType)entry.Quest2RewardTypeId)
            {
                case QuestRewardType.Item:
                    player.Inventory.ItemCreate(entry.ObjectId, entry.ObjectAmount);
                    break;
                case QuestRewardType.Money:
                    player.CurrencyManager.CurrencyAddAmount((CurrencyType)entry.ObjectId, entry.ObjectAmount);
                    break;
                default:
                {
                    log.Warn($"Unhandled quest reward type {entry.Quest2RewardTypeId}!");
                    break;
                }
            }

            log.Trace($"Recieved quest reward, type: {(QuestRewardType)entry.Quest2RewardTypeId}, objectId: {entry.ObjectId}, amount: {entry.ObjectAmount}.");
        }

        /// <summary>
        /// Ignore or acknowledge an inactive <see cref="Quest"/>.
        /// </summary>
        public void QuestIgnore(ushort questId, bool ignored)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            // TODO:
        }

        /// <summary>
        /// Track or hide an active <see cref="Quest"/>.
        /// </summary>
        public void QuestTrack(ushort questId, bool tracked)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            Quest.Quest quest = GetQuest(questId, GetQuestFlags.Active);
            if (quest == null)
                throw new QuestException($"Player {player.CharacterId} tried to track quest {questId} which they don't have!");

            if (quest.State != QuestState.Accepted && quest.State != QuestState.Achieved)
                throw new QuestException($"Player {player.CharacterId} tried to track quest {questId} with invalid state!");

            if (tracked)
                quest.Flags |= QuestFlags.Tracked;
            else
                quest.Flags &= ~QuestFlags.Tracked;

            log.Trace($"Updated tracked state of quest {questId} to {tracked}.");
        }

        /// <summary>
        /// Share supplied <see cref="Quest"/> with another <see cref="Player"/>.
        /// </summary>
        public void QuestShare(ushort questId)
        {
            QuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            Quest.Quest quest = GetQuest(questId);
            if (quest == null)
                throw new QuestException($"Player {player.CharacterId} tried to share quest {questId} which they don't have!");

            if (!quest.CanShare())
                throw new QuestException($"Player {player.CharacterId} tried to share quest {questId} which can't be shared!");

            Player recipient = player.GetVisible<Player>(player.TargetGuid);
            if (recipient == null)
                throw new QuestException($"Player {player.CharacterId} tried to share quest {questId} to an invalid player!");

            // TODO

            log.Trace($"Shared quest {questId} with player {recipient.Name}.");
        }

        /// <summary>
        /// Accept or deny a shared <see cref="Quest"/> from another <see cref="Player"/>.
        /// </summary>
        public void QuestShareResult(ushort questId, bool result)
        {
            // TODO
        }

        /// <summary>
        /// Update any active <see cref="Quest"/> <see cref="QuestObjective"/>'s with supplied <see cref="QuestObjectiveType"/> and data with progress.
        /// </summary>
        public void ObjectiveUpdate(QuestObjectiveType type, uint data, uint progress)
        {
            foreach (Quest.Quest quest in activeQuests.Values)
                quest.ObjectiveUpdate(type, data, progress);
        }
    }
}
