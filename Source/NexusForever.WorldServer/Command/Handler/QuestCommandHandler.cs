using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Quest;
using NexusForever.WorldServer.Game.Quest.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Quest")]
    public class QuestCommandHandler : CommandCategory
    {
        public QuestCommandHandler()
            : base(true, "quest")
        {
        }

        [SubCommandHandler("add", "questId - Add a new quest")]
        public async Task QuestAddCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1 || !ushort.TryParse(parameters[0], out ushort questId))
            {
                await SendHelpAsync(context);
                return;
            }

            QuestInfo info = GlobalQuestManager.Instance.GetQuestInfo(questId);
            if (info == null)
            {
                await context.SendMessageAsync($"Quest id {questId} is invalid!");
                return;
            }

            context.Session.Player.QuestManager.QuestAdd(info);
        }

        [SubCommandHandler("achieve", "questId - Complete all objectives for quest")]
        public async Task QuestAchieveCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1 || !ushort.TryParse(parameters[0], out ushort questId))
            {
                await SendHelpAsync(context);
                return;
            }

            context.Session.Player.QuestManager.QuestAchieve(questId);
        }

        [SubCommandHandler("achieveobjective", "questId, objectiveIndex - Complete a single objective for quest")]
        public async Task QuestObjectiveAchieveCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 2
                || !ushort.TryParse(parameters[0], out ushort questId)
                || !byte.TryParse(parameters[1], out byte index))
            {
                await SendHelpAsync(context);
                return;
            }

            context.Session.Player.QuestManager.QuestAchieveObjective(questId, index);
        }

        [SubCommandHandler("objective", "objectiveType, data - Update all quest objectives with type and data by 1")]
        public async Task QuestObjectiveCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 2
                || !byte.TryParse(parameters[0], out byte type)
                || !uint.TryParse(parameters[1], out uint data))
            {
                await SendHelpAsync(context);
                return;
            }

            context.Session.Player.QuestManager.ObjectiveUpdate((QuestObjectiveType)type, data, 1u);
        }
    }
}
