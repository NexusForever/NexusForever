using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NexusForever.Database;
using NexusForever.Database.World;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Chat;
using NexusForever.Game.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Chat;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Quest, "A collection of commands to manage quests for a character.", "quest")]
    [CommandTarget(typeof(IPlayer))]
    public class QuestCommandCategory : CommandCategory
    {
        // Objective types that currently have ObjectiveUpdate call sites in gameplay code.
        private static readonly HashSet<QuestObjectiveType> WiredObjectiveTypes =
        [
            QuestObjectiveType.KillCreature,
            QuestObjectiveType.KillCreature2,
            QuestObjectiveType.KillTargetGroup,
            QuestObjectiveType.KillTargetGroups,
            QuestObjectiveType.ActivateEntity,
            QuestObjectiveType.ActivateTargetGroup,
            QuestObjectiveType.ActivateTargetGroupChecklist,
            QuestObjectiveType.TalkTo,
            QuestObjectiveType.TalkToTargetGroup,
            QuestObjectiveType.EnterZone,
            QuestObjectiveType.EnterArea,
            QuestObjectiveType.SucceedCSI
        ];

        [Command(Permission.QuestList, "List all active quests.", "list")]
        public void HandleQuestList(ICommandContext context)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Active quests for {context.GetTargetOrInvoker<IPlayer>().Name}");
            builder.AppendLine("=============================");
            context.SendMessage(builder.ToString());
            foreach (Quest quest in context.GetTargetOrInvoker<IPlayer>().QuestManager.GetActiveQuests())
            {
                var chatBuilder = new ChatMessageBuilder
                {
                    Type = ChatChannelType.System,
                    Text = $"({quest.Id}) "
                };
                chatBuilder.AppendQuest(quest.Id);
                context.GetTargetOrInvoker<IPlayer>().Session.EnqueueMessageEncrypted(chatBuilder.Build());
            }
        }

        [Command(Permission.QuestList, "Dev diagnostic: list quests tied to creatures spawned in a world (default 3460 NPE).", "world")]
        public void HandleQuestWorld(ICommandContext context,
            [Parameter("World id to inspect. Defaults to 3460 (New Player Experience).")]
            ushort? worldId)
        {
            ushort world = worldId ?? 3460;
            var entities = DatabaseManager.Instance.GetDatabase<WorldDatabase>().GetEntities(world);
            var spawnCreatures = entities.Select(e => e.Creature).Distinct().ToHashSet();

            context.SendMessage($"World {world}: {entities.Count} entities, {spawnCreatures.Count} unique creatures.");
            if (spawnCreatures.Count == 0)
            {
                context.SendMessage("No entities found for this world in the world DB.");
                return;
            }

            var spawnTargetGroups = new HashSet<uint>();
            foreach (uint creatureId in spawnCreatures)
            {
                foreach (uint targetGroupId in AssetManager.Instance.GetTargetGroupsForCreatureId(creatureId) ?? Enumerable.Empty<uint>())
                    spawnTargetGroups.Add(targetGroupId);
            }

            var questIds = new SortedSet<ushort>();
            var givers = new Dictionary<ushort, List<uint>>();
            var receivers = new Dictionary<ushort, List<uint>>();

            foreach (uint creatureId in spawnCreatures)
            {
                Creature2Entry creature = GameTableManager.Instance.Creature2.GetEntry(creatureId);
                if (creature == null)
                    continue;

                foreach (uint questId in creature.QuestIdGiven.Where(q => q != 0u))
                {
                    var id = (ushort)questId;
                    questIds.Add(id);
                    if (!givers.TryGetValue(id, out List<uint> list))
                        givers[id] = list = [];
                    list.Add(creatureId);
                }

                foreach (uint questId in creature.QuestIdReceive.Where(q => q != 0u))
                {
                    var id = (ushort)questId;
                    questIds.Add(id);
                    if (!receivers.TryGetValue(id, out List<uint> list))
                        receivers[id] = list = [];
                    list.Add(creatureId);
                }
            }

            foreach (Quest2Entry questEntry in GameTableManager.Instance.Quest2.Entries)
            {
                foreach (uint objectiveId in questEntry.Objectives.Where(o => o != 0u))
                {
                    QuestObjectiveEntry objective = GameTableManager.Instance.QuestObjective.GetEntry(objectiveId);
                    if (objective == null)
                        continue;

                    if (ObjectiveTouchesSpawn(objective, spawnCreatures, spawnTargetGroups))
                        questIds.Add((ushort)questEntry.Id);
                }
            }

            // Also include quests whose WorldZoneId is mapped to this world.
            var worldZoneIds = GameTableManager.Instance.MapZoneWorldJoin.Entries
                .Where(e => e.WorldId == world)
                .Select(e => GameTableManager.Instance.MapZone.GetEntry(e.MapZoneId)?.WorldZoneId ?? 0u)
                .Where(z => z != 0u)
                .ToHashSet();

            foreach (Quest2Entry questEntry in GameTableManager.Instance.Quest2.Entries)
            {
                if (questEntry.WorldZoneId != 0u && worldZoneIds.Contains(questEntry.WorldZoneId))
                    questIds.Add((ushort)questEntry.Id);
            }

            context.SendMessage($"Found {questIds.Count} quest(s). Types marked WIRED have ObjectiveUpdate call sites.");

            var typeUsage = new Dictionary<QuestObjectiveType, int>();
            foreach (ushort questId in questIds)
            {
                IQuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
                if (info == null)
                {
                    context.SendMessage($"Q{questId}: missing Quest2 entry");
                    continue;
                }

                string title = GameTableManager.Instance.TextEnglish.GetEntry(info.Entry.LocalizedTextIdTitle) ?? $"#{info.Entry.LocalizedTextIdTitle}";
                string giverText = givers.TryGetValue(questId, out List<uint> giverList)
                    ? string.Join(',', giverList.Distinct())
                    : "-";
                string receiverText = receivers.TryGetValue(questId, out List<uint> receiverList)
                    ? string.Join(',', receiverList.Distinct())
                    : "-";

                context.SendMessage($"Q{questId} \"{title}\" givers=[{giverText}] receivers=[{receiverText}]");

                byte index = 0;
                foreach (IQuestObjectiveInfo objective in info.Objectives)
                {
                    var type = objective.Type;
                    typeUsage[type] = typeUsage.GetValueOrDefault(type) + 1;
                    string wired = WiredObjectiveTypes.Contains(type) ? "WIRED" : "NOT WIRED";
                    string text = GameTableManager.Instance.TextEnglish.GetEntry(objective.Entry.LocalizedTextIdShort)
                        ?? GameTableManager.Instance.TextEnglish.GetEntry(objective.Entry.LocalizedTextIdFull)
                        ?? "";
                    context.SendMessage($"  [{index}] {type}({(uint)type}) data={objective.Entry.Data} count={objective.Entry.Count} [{wired}] {text}");
                    index++;
                }
            }

            context.SendMessage("Objective type summary:");
            foreach ((QuestObjectiveType type, int count) in typeUsage.OrderBy(k => (uint)k.Key))
            {
                string wired = WiredObjectiveTypes.Contains(type) ? "WIRED" : "NOT WIRED";
                context.SendMessage($"  {type}({(uint)type}): {count} [{wired}]");
            }
        }

        private static bool ObjectiveTouchesSpawn(QuestObjectiveEntry objective, HashSet<uint> spawnCreatures, HashSet<uint> spawnTargetGroups)
        {
            var type = (QuestObjectiveType)objective.Type;
            return type switch
            {
                QuestObjectiveType.KillCreature
                    or QuestObjectiveType.KillCreature2
                    or QuestObjectiveType.TalkTo
                    or QuestObjectiveType.ActivateEntity
                    or QuestObjectiveType.ActivateEntity2
                    or QuestObjectiveType.SucceedCSI
                    or QuestObjectiveType.GatheResource
                    => spawnCreatures.Contains(objective.Data),
                QuestObjectiveType.KillTargetGroup
                    or QuestObjectiveType.KillTargetGroups
                    or QuestObjectiveType.ActivateTargetGroup
                    or QuestObjectiveType.ActivateTargetGroupChecklist
                    or QuestObjectiveType.TalkToTargetGroup
                    => spawnTargetGroups.Contains(objective.Data),
                _ => false
            };
        }

        [Command(Permission.QuestAdd, "Add a new quest to character.", "add")]
        public void HandleQuestAdd(ICommandContext context,
            [Parameter("Quest entry id to add to character.")]
            ushort questId)
        {
            IQuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
            {
                context.SendMessage($"Quest id {questId} is invalid!");
                return;
            }

            context.GetTargetOrInvoker<IPlayer>().QuestManager.QuestAdd(info);
        }

        [Command(Permission.QuestAchieve, "Achieve an existing quest by completing all objectives for character.", "achieve")]
        public void HandleQuestAchieve(ICommandContext context,
            [Parameter("Quest entry id to achieve for character.")]
            ushort questId)
        {
            IQuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
            {
                context.SendMessage($"Quest id {questId} is invalid!");
                return;
            }

            context.GetTargetOrInvoker<IPlayer>().QuestManager.QuestAchieve(questId);
        }

        [Command(Permission.QuestAchieveObjective, "Achieve a single objective for an existing quest for character.", "achieveobjective")]
        public void HandleQuestAchieveObjective(ICommandContext context,
            [Parameter("Quest entry id to achieve for character.")]
            ushort questId,
            [Parameter("Quest objective index to achieve for character.")]
            byte index)
        {
            IQuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
            {
                context.SendMessage($"Quest id {questId} is invalid!");
                return;
            }

            context.GetTargetOrInvoker<IPlayer>().QuestManager.QuestAchieveObjective(questId, index);
        }

        [Command(Permission.QuestObjective, "Update all quest objectives with type for character.", "objective")]
        public void HandleQuestObjective(ICommandContext context,
            [Parameter("Quest objective type for objectives to update.", ParameterFlags.None, typeof(EnumParameterConverter<QuestObjectiveType>))]
            QuestObjectiveType type,
            [Parameter("Data value to match quest objectives against.")]
            uint data,
            [Parameter("Progress to increment matching quest objectives.")]
            uint? progress)
        {
            progress ??= 1u;
            context.GetTargetOrInvoker<IPlayer>().QuestManager.ObjectiveUpdate(type, data, progress.Value);
        }

        [Command(Permission.QuestKill, "Update all quest objectives that require a kill with the given creature id.", "kill")]
        public void HandleQuestKill(ICommandContext context,
            [Parameter("Creature id to match quest objectives against.")]
            uint creatureId,
            [Parameter("Quantity to update quest objectives by.")]
            uint? quantity)
        {
            quantity ??= 1u;

            var target = context.GetTargetOrInvoker<IPlayer>();
            target.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillCreature, creatureId, quantity.Value);
            target.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillCreature2, creatureId, quantity.Value);

            foreach (uint targetGroupId in AssetManager.Instance.GetTargetGroupsForCreatureId(creatureId) ?? Enumerable.Empty<uint>())
            {
                target.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillTargetGroup, targetGroupId, quantity.Value);
                target.QuestManager.ObjectiveUpdate(QuestObjectiveType.KillTargetGroups, targetGroupId, quantity.Value);
            }

            context.SendMessage($"Success! You've killed {quantity} of Creature ID: {creatureId}");
        }
    }
}
