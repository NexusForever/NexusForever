using NexusForever.Database.Character;
using System.Numerics;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Quest;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Achievement;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.Reputation;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Entity
{
    public class QuestManager : IQuestManager
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

        private readonly IPlayer player;
        private readonly TimeProvider timeProvider;
        private (IBaseMap Map, QuestState? State, bool Available)? shadePlantVisibility;

        private readonly Dictionary<ushort, IQuest> completedQuests = new();
        private readonly Dictionary<ushort, IQuest> inactiveQuests = new();
        private readonly Dictionary<ushort, IQuest> activeQuests = new();

        /// <summary>
        /// Create a new <see cref="IQuestManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public QuestManager(IPlayer owner, CharacterModel model)
            : this(owner, model, TimeProvider.System)
        {
        }

        internal QuestManager(IPlayer owner, CharacterModel model, TimeProvider timeProvider)
        {
            player = owner;
            this.timeProvider = timeProvider;

            foreach (CharacterQuestModel questModel in model.Quest)
            {
                IQuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questModel.QuestId);
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
                    case QuestState.Mentioned:
                        inactiveQuests.Add(quest.Id, quest);
                        break;
                    case QuestState.Accepted:
                    case QuestState.Achieved:
                        activeQuests.Add(quest.Id, quest);
                        break;
                }
            }
        }

        public void Dispose()
        {
            foreach (IQuest quest in completedQuests.Values
                .Concat(inactiveQuests.Values)
                .Concat(activeQuests.Values))
                quest.Dispose();
        }

        public void Save(CharacterContext context)
        {
            foreach (IQuest quest in completedQuests.Values)
                quest.Save(context);

            foreach (IQuest quest in inactiveQuests.Values.ToList())
            {
                if (quest.PendingDelete)
                    inactiveQuests.Remove(quest.Id);

                quest.Save(context);
            }

            foreach (IQuest quest in activeQuests.Values.ToList())
            {
                if (quest.PendingDelete)
                    activeQuests.Remove(quest.Id);

                quest.Save(context);
            }
        }

        public void Update(double lastTick)
        {
            RefreshShadePlantActors();
            RefreshBorellianTavernActors();
            BorellianTavern.Update(player, lastTick);
            ShadeEvePlants.Gather(player, activeQuests);
            var botchedQuests = new List<IQuest>();
            foreach (IQuest quest in activeQuests.Values)
            {
                quest.Update(lastTick);
                if (quest.State == QuestState.Botched)
                    botchedQuests.Add(quest);
            }

            foreach (IQuest quest in botchedQuests)
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
                        QuestTimeElapsed      = q.Timer ?? 0u,
                        Objectives = q.Select(o => new ServerQuestInit.QuestActive.Objective
                        {
                            Progress = o.Progress,
                            TimeElapsed    = 0u
                        }).ToList()
                    }).ToList()
            });
        }

        /// <summary>
        /// Return <see cref="QuestState"/> for supplied quest.
        /// </summary>
        public QuestState? GetQuestState<T>(T questId) where T : Enum
        {
            return GetQuestState(questId.As<T, ushort>());
        }

        /// <summary>
        /// Return <see cref="QuestState"/> for supplied quest.
        /// </summary>
        public QuestState? GetQuestState(ushort questId)
        {
            return GetQuest(questId)?.State;
        }

        private IQuest GetQuest(ushort questId, GetQuestFlags flags = GetQuestFlags.All)
        {
            if ((flags & GetQuestFlags.Active) != 0
                && activeQuests.TryGetValue(questId, out IQuest quest))
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
        /// Mention a quest from supplied quest id, skipping any prerequisites checks.
        /// </summary>
        public void QuestMention(ushort questId)
        {
            IQuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            if (DisableManager.Instance.IsDisabled(DisableType.Quest, questId))
            {
                player.SendSystemMessage($"Unable to add quest {questId} because it is disabled.");
                return;
            }

            if (GetQuest(questId) != null)
                return;

            QuestMention(info);
        }

        /// <summary>
        /// Mention a quest from supplied <see cref="IQuestInfo"/>, skipping any prerequisites checks.
        /// </summary>
        public void QuestMention(IQuestInfo info)
        {
            IQuest quest = GetQuest((ushort)info.Entry.Id);
            if (quest == null)
                quest = new Quest.Quest(player, info);
            else
                QuestRemove(quest);

            quest.State = QuestState.Mentioned;
            inactiveQuests.Add((ushort)info.Entry.Id, quest);

            log.Trace($"Mentioned new quest {info.Entry.Id}.");
        }

        /// <summary>
        /// Add a quest from supplied id, optionally supplying <see cref="IItem"/> which was used to start the quest.
        /// </summary>
        public void QuestAdd(ushort questId, IItem item)
        {
            IQuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            if (DisableManager.Instance.IsDisabled(DisableType.Quest, questId))
            {
                player.SendSystemMessage($"Unable to add quest {questId} because it is disabled.");
                return;
            }

            IQuest quest = GetQuest(questId);
            QuestAdd(info, quest, item);
        }

        private void QuestAdd(IQuestInfo info, IQuest quest, IItem item)
        {
            if (quest?.State is QuestState.Accepted or QuestState.Achieved)
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
                if (info.Entry.Id != item.Info.Entry.Quest2IdActivation)
                    throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} from invalid item {item.Info.Entry.Id}!");

                // TODO: consume charge
            }
            else
            {
                // make sure the player is in range of a quest giver or they are eligible for a communicator message that starts the quest
                if (!GlobalQuestManager.Instance.GetQuestGivers((ushort)info.Entry.Id)
                        .Any(c => player.GetVisibleCreature<WorldEntity>(c).Any())
                    && !GlobalQuestManager.Instance.GetQuestCommunicatorMessages((ushort)info.Entry.Id)
                        .Any(m => m.Meets(player)))
                    throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} without quest giver!");
            }

            if (ShadeEvePlants.HandlesQuest(info.Entry.Id) &&
                (item != null || !ShadeEvePlants.CanVisitContact(player, (ushort)info.Entry.Id)))
                throw new QuestException("Accept this Shade's Eve quest from its nearby capital contact.");

            if (BorellianTavern.HandlesQuest(info.Entry.Id) && (item != null || !BorellianTavern.AllowsContact(player, info.Entry.Id)))
                throw new QuestException("Accept this Borellian tavern quest from its nearby patron.");

            // server doesn't send an error message for prerequisites since the client does the exact same checks
            // it's assumed that a player could never get here without cheating in some way
            if (!MeetsPrerequisites(info))
                throw new QuestException($"Player {player.CharacterId} tried to start quest {info.Entry.Id} without meeting the prerequisites!");

            QuestAdd(info);
        }

        private (object Map, QuestState?, QuestState?)? borellianVisibility;

        private void RefreshBorellianTavernActors()
        {
            if (player is not Player viewer || player.IsLoading || player.Map is not { Entry.Id: BorellianTavern.World } map)
            {
                borellianVisibility = null;
                return;
            }
            var current = (map, GetQuestState(6251), GetQuestState(6535));
            if (borellianVisibility == current)
                return;

            borellianVisibility = current;
            viewer.RefreshBorellianTavernActors();
        }

        private void RefreshShadePlantActors()
        {
            if (player is not Player viewer || player.IsLoading || player.Map is not { } map ||
                ShadeEvePlants.FindQuest(map.Entry.Id) is not ushort id)
            {
                shadePlantVisibility = null;
                return;
            }

            var current = (map, GetQuestState(id), CanSeeShadePlantQuest(id));
            if (shadePlantVisibility == current)
                return;

            shadePlantVisibility = current;
            viewer.RefreshShadePlantActors();
        }

        internal bool CanSeeShadePlantQuest(ushort questId)
        {
            IQuest quest = GetQuest(questId);
            if (quest is { PendingDelete: false, State: QuestState.Accepted or QuestState.Achieved })
                return true;

            DateTime now = timeProvider.GetUtcNow().UtcDateTime;
            return !(quest?.Reset > now) && ShadeEveQuestSeason.CanAccept(questId, now);
        }

        private bool MeetsPrerequisites(IQuestInfo info)
        {
            if (!ShadeEveQuestSeason.CanAccept(info.Entry.Id, timeProvider.GetUtcNow().UtcDateTime))
                return false;

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

            if (!info.PrerequisiteQuests.IsEmpty)
            {
                bool preReqQuestsCompleted;
                if ((info.Entry.PrerequisiteFlags & 1) != 0u)
                    preReqQuestsCompleted = info.PrerequisiteQuests.Any(q => GetQuestState((ushort)q.Id) == QuestState.Completed);
                else
                    preReqQuestsCompleted = info.PrerequisiteQuests.All(q => GetQuestState((ushort)q.Id) == QuestState.Completed);

                if (!preReqQuestsCompleted)
                    return false;
            }

            if (info.Entry.PrerequisiteId != 0u && !PrerequisiteManager.Instance.Meets(player, info.Entry.PrerequisiteId))
                return false;

            if (!info.IsContract())
            {
                GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(655);
                // client also hard codes 40 if entry doesn't exist
                if (activeQuests.Count > (entry?.Dataint0 ?? 40u))
                    return false;
            }
            else
            {
                // TODO: contracts use reward property for max slots, RewardProperty.ActiveContractSlots
            }

            return true;
        }

        public void OfferShadeEveStarter(IWorldEntity contact)
        {
            if (contact is not INonPlayerEntity { EntityId: > 0u, IsAlive: true } ||
                !player.InWorld || player.IsLoading || !player.IsAlive || player.Map != contact.Map ||
                Vector3.DistanceSquared(player.Position, contact.Position) > 36f)
                return;

            (ushort questId, uint itemId, uint markerId) = (contact.CreatureId, player.Faction1) switch
            {
                (65407u, Faction.Exile) => ((ushort)9760, 49483u, 47315u),
                (65406u, Faction.Dominion) => ((ushort)9761, 49484u, 47289u),
                _ => ((ushort)0, 0u, 0u)
            };
            if (questId == 0 || GetQuestState(questId) is not (null or QuestState.Unknown or QuestState.Mentioned or QuestState.Abandoned) ||
                DisableManager.Instance.IsDisabled(DisableType.Quest, questId) ||
                GameTableManager.Instance.WorldLocation2.GetEntry(markerId) is not { } marker ||
                contact.Map?.Entry?.Id != marker.WorldId || contact.Zone?.Id != marker.WorldZoneId ||
                Vector3.DistanceSquared(contact.Position, new Vector3(marker.Position0, marker.Position1, marker.Position2)) > 1f ||
                GlobalQuestManager.Instance.GetQuestInfo(questId) is not { } info || !MeetsPrerequisites(info))
                return;

            IInventory inventory = player.Inventory;
            if (inventory.HasItemCount(itemId, 1u) || inventory.Any(bag =>
                bag.Location == InventoryLocation.PlayerBank && bag.Any(item => item.Id == itemId)) ||
                ItemManager.Instance.GetItemInfo(itemId) is not { } starter || starter.Entry.Quest2IdActivation != questId)
                return;

            if (inventory.IsInventoryFull(InventoryLocation.Inventory))
                player.SendGenericError(GenericError.ItemInventoryFull);
            else
                inventory.ItemCreate(InventoryLocation.Inventory, starter, 1u, ItemUpdateReason.Loot);
        }

        /// <summary>
        /// Add a quest from supplied <see cref="IQuestInfo"/>, skipping any prerequisites checks.
        /// </summary>
        public void QuestAdd(IQuestInfo info)
        {
            if (!ShadeEveQuestSeason.CanAccept(info.Entry.Id, timeProvider.GetUtcNow().UtcDateTime))
                return;

            // make sure player has room for all pushed items
            if (player.Inventory.GetInventorySlotsRemaining(InventoryLocation.Inventory)
                < info.Entry.PushedItemIds.Count(i => i != 0u))
            {
                player.SendGenericError(GenericError.ItemInventoryFull);
                return;
            }

            for (int i = 0; i < info.Entry.PushedItemIds.Length; i++)
            {
                uint itemId = info.Entry.PushedItemIds[i];
                if (itemId != 0u)
                    player.Inventory.ItemCreate(InventoryLocation.Inventory, itemId, info.Entry.PushedItemCounts[i]);
            }

            // TODO: virtual items

            IQuest quest = GetQuest((ushort)info.Entry.Id);
            if (quest == null)
                quest = new Quest.Quest(player, info);
            else
                QuestRemove(quest);

            quest.Flags |= QuestStateFlags.Tracked;
            quest.State = QuestState.Accepted;
            activeQuests.Add((ushort)info.Entry.Id, quest);

            quest.InitialiseTimer();
            if (ShadeEveQuestSeason.IsIntroductoryQuest(info.Entry.Id) && info.Objectives.IsEmpty)
                quest.State = QuestState.Achieved;

            log.Trace($"Accepted new quest {info.Entry.Id}.");
        }

        private void QuestRemove(IQuest quest)
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
                case QuestState.Mentioned:
                    inactiveQuests.Remove(quest.Id);
                    break;
            }

            if (quest.PendingDelete)
                quest.EnqueueDelete(false);

            // reset previous objective progress
            foreach (IQuestObjective objective in quest)
            {
                objective.Progress = 0u;
                if (objective is QuestObjective tracked)
                    tracked.ClearChecklist();
            }
        }

        /// <summary>
        /// Retry an inactive quest id that was previously failed.
        /// </summary>
        public void QuestRetry(ushort questId)
        {
            IQuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            IQuest quest = GetQuest(questId, GetQuestFlags.Inactive);
            if (quest == null)
                throw new QuestException($"Player {player.CharacterId} tried to restart quest {questId} which they don't have!");

            if (quest.State != QuestState.Botched)
                throw new QuestException($"Player {player.CharacterId} tried to restart quest {questId} which hasn't been failed!");

            QuestAdd(info, quest, null);
        }

        /// <summary>
        /// Abandon an active quest.
        /// </summary>
        public void QuestAbandon(ushort questId)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            IQuest quest = GetQuest(questId, GetQuestFlags.Active | GetQuestFlags.Inactive);
            if (quest == null || quest.PendingDelete)
                throw new QuestException($"Player {player.CharacterId} tried to abandon quest {questId} which they don't have!");

            if (!quest.CanAbandon())
                throw new QuestException($"Player {player.CharacterId} tried to abandon quest {questId} which can't be abandoned!");

            // don't delete quests that have been mentioned, they may not be able to be re-collected.
            if (!quest.PendingCreate && quest.CanDelete())
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

            foreach (IQuestObjective objective in quest)
            {
                objective.Progress = 0u;
                if (objective is QuestObjective tracked)
                    tracked.ClearChecklist();
            }

            if (quest.Info.IsQuestMentioned)
            {
                quest.State = QuestState.Mentioned;
                inactiveQuests.Add(quest.Id, quest);
            }
            else
                quest.State = QuestState.Abandoned;

            log.Trace($"Abandoned quest {questId}.");
        }

        /// <summary>
        /// Complete all <see cref="IQuestObjective"/>'s for supplied active quest id.
        /// </summary>
        public void QuestAchieve(ushort questId)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            IQuest quest = GetQuest(questId);
            if (quest == null || quest.PendingDelete)
                throw new QuestException($"Player {player.CharacterId} tried to achieve quest {questId} which they don't have!");

            if (quest.State != QuestState.Accepted)
                throw new QuestException($"Player {player.CharacterId} tried to achieve quest {questId} with invalid state!");

            foreach (IQuestObjectiveInfo info in quest.Info.Objectives)
                quest.ObjectiveUpdate(info.Type, info.Entry.Data, info.Entry.Count);
        }

        /// <summary>
        /// Complete single <see cref="IQuestObjective"/> for supplied active quest id.
        /// </summary>
        public void QuestAchieveObjective(ushort questId, byte index)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            IQuest quest = GetQuest(questId);
            if (quest == null || quest.PendingDelete)
                throw new QuestException();

            if (quest.State != QuestState.Accepted)
                throw new QuestException();

            IQuestObjective objective = quest.SingleOrDefault(o => o.Index == index);
            if (objective == null)
                throw new QuestException();

            quest.ObjectiveUpdate(objective.ObjectiveInfo.Type, objective.ObjectiveInfo.Entry.Data, objective.ObjectiveInfo.Entry.Count);
        }

        /// <summary>
        /// Complete an achieved quest supplying an optional reward and whether the quest was completed from the communicator.
        /// </summary>
        public void QuestComplete(ushort questId, ushort reward, bool communicator)
        {
            IQuestInfo questInfo = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (questInfo == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            if (DisableManager.Instance.IsDisabled(DisableType.Quest, questId))
            {
                player.SendSystemMessage($"Unable to complete quest {questId} because it is disabled.");
                return;
            }

            IQuest quest = GetQuest(questId, GetQuestFlags.Active);
            if (quest == null)
            {
                if (!questInfo.IsAutoComplete())
                    throw new QuestException($"Player {player.CharacterId} tried to complete quest {questId} which they don't have!");

                QuestAdd(questId, null);
                quest = GetQuest(questId);
                quest.State = QuestState.Achieved;
            }

            if (quest.State != QuestState.Achieved)
                throw new QuestException($"Player {player.CharacterId} tried to complete quest {questId} which wasn't complete!");

            if (ShadeEvePlants.HandlesQuest(questId) && (communicator || !ShadeEvePlants.CanVisitContact(player, questId)))
                throw new QuestException("Report this Shade's Eve quest to its nearby capital contact.");

            if (BorellianTavern.HandlesQuest(questId) && (communicator || !BorellianTavern.AllowsContact(player, questId)))
                throw new QuestException("Complete this Borellian tavern quest at its nearby patron.");

            if (communicator)
            {
                // TODO: check if this is complete, client seems to also refer to contact info
                // for more see QuestTracker:HelperShowQuestCallbackBtn in LUA which contains the logic to show the complete button in the quest tracker
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

        private void RewardQuest(IQuestInfo info, ushort reward)
        {
            // Handle all Rewards that are not chosen
            foreach (Quest2RewardEntry rewardEntry in info.Rewards.Values.Where(x => x.Flags == 0))
                RewardQuest(rewardEntry);

            // Handle any chosen rewards
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
                player.XpManager.GrantXp(experience, ExpReason.Quest);

            uint money = info.GetRewardMoney();
            if (money != 0u)
                player.CurrencyManager.CurrencyAddAmount(CurrencyType.Credits, money);
        }

        private void RewardQuest(Quest2RewardEntry entry)
        {
            switch ((QuestRewardType)entry.Quest2RewardTypeId)
            {
                case QuestRewardType.Item:
                    player.Inventory.ItemCreate(InventoryLocation.Inventory, entry.ObjectId, entry.ObjectAmount);
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
        /// Ignore or acknowledge an inactive quest.
        /// </summary>
        public void QuestIgnore(ushort questId, bool ignored)
        {
            IQuestInfo questInfo = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (questInfo == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            IQuest quest = GetQuest((ushort)questInfo.Entry.Id);
            if (quest == null)
                quest = new Quest.Quest(player, questInfo); // Add quest so we can set it to ignored.
            else
                QuestRemove(quest); // Removes from quest log. Might not be the cleanest way to do this?

            quest.State = ignored ? QuestState.Ignored : QuestState.Mentioned;

            inactiveQuests.Add(questId, quest);
        }

        /// <summary>
        /// Track or hide an active quest.
        /// </summary>
        public void QuestTrack(ushort questId, bool tracked)
        {
            if (GlobalQuestManager.Instance.GetQuestInfo(questId) == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            IQuest quest = GetQuest(questId, GetQuestFlags.Active);
            if (quest == null)
                throw new QuestException($"Player {player.CharacterId} tried to track quest {questId} which they don't have!");

            if (quest.State != QuestState.Accepted && quest.State != QuestState.Achieved)
                throw new QuestException($"Player {player.CharacterId} tried to track quest {questId} with invalid state!");

            if (tracked)
                quest.Flags |= QuestStateFlags.Tracked;
            else
                quest.Flags &= ~QuestStateFlags.Tracked;

            log.Trace($"Updated tracked state of quest {questId} to {tracked}.");
        }

        /// <summary>
        /// Share supplied quest with another <see cref="IPlayer"/>.
        /// </summary>
        public void QuestShare(ushort questId)
        {
            IQuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
                throw new ArgumentException($"Invalid quest {questId}!");

            IQuest quest = GetQuest(questId);
            if (quest == null)
                throw new QuestException($"Player {player.CharacterId} tried to share quest {questId} which they don't have!");

            if (!quest.CanShare())
                throw new QuestException($"Player {player.CharacterId} tried to share quest {questId} which can't be shared!");

            if (player.TargetGuid == null)
                throw new QuestException($"Player {player.CharacterId} tried to share quest {questId} without a target!");

            IPlayer recipient = player.GetVisible<IPlayer>(player.TargetGuid.Value);
            if (recipient == null)
                throw new QuestException($"Player {player.CharacterId} tried to share quest {questId} to an invalid player!");

            // TODO

            log.Trace($"Shared quest {questId} with player {recipient.Name}.");
        }

        /// <summary>
        /// Accept or deny a shared quest from another <see cref="IPlayer"/>.
        /// </summary>
        public void QuestShareResult(ushort questId, bool result)
        {
            // TODO
        }

        /// <summary>
        /// Update any active quest <see cref="IQuestObjective"/>'s with supplied <see cref="QuestObjectiveType"/> and data with progress.
        /// </summary>
        public void ObjectiveUpdate(QuestObjectiveType type, uint data, uint progress)
        {
            foreach (IQuest quest in activeQuests.Values)
                quest.ObjectiveUpdate(type, data, progress);
        }

        /// <summary>
        /// Update any active quest <see cref="IQuestObjective"/>'s with supplied ID with progress.
        /// </summary>
        public void ObjectiveUpdate(uint id, uint progress)
        {
            foreach (IQuest quest in activeQuests.Values)
                quest.ObjectiveUpdate(id, progress);
        }

        /// <summary>
        /// Returns a collection of all active quests.
        /// </summary>
        public IEnumerable<IQuest> GetActiveQuests()
        {
            return activeQuests.Values;
        }
    }
}
