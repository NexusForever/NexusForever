using System.Linq;
using System.Text;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Quest;
using NexusForever.Game.Social;
using NexusForever.Game.Static.Quest;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Social;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Quest, "A collection of commands to manage quests for a character.", "quest")]
    [CommandTarget(typeof(IPlayer))]
    public class QuestCommandCategory : CommandCategory
    {
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
