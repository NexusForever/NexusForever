using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Static;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Story")]
    public class StoryCommandHandler : CommandCategory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public StoryCommandHandler()
            : base(true, "story")
        {
        }

        [SubCommandHandler("panelId", "storyPanelId - Send a Story Panel to this Player.")]
        [SubCommandHandler("p", "storyPanelId - Send a Story Panel to this Player.")]
        public Task PanelSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length < 1)
            {
                SendHelpAsync(context);
                return Task.CompletedTask;
            }

            StoryBuilder.Instance.SendStoryPanel(GameTableManager.Instance.StoryPanel.GetEntry(uint.Parse(parameters[0])), context.Session.Player);

            return Task.CompletedTask;
        }

        [SubCommandHandler("communicator", "textId creatureId [duration] [storyPanelType] [windowTypeId] [soundEventId] - Send a test story window to this Player.")]
        [SubCommandHandler("c", "textId creatureId [duration] [storyPanelType] [windowTypeId] [soundEventId] - Send a test story window to this Player.")]
        public Task TestSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length < 2)
            {
                SendHelpAsync(context);
                return Task.CompletedTask;
            }

            uint textId = 0;
            if (!uint.TryParse(parameters[0], out textId))
            {
                context.SendErrorAsync("Error trying to parse textId.");
                return Task.CompletedTask;
            }

            uint creatureId = 0;
            if (!uint.TryParse(parameters[1], out creatureId))
            {
                context.SendErrorAsync("Error trying to parse creatureId.");
                return Task.CompletedTask;
            }

            uint duration = 10000;
            if (parameters.Length >= 3 && !uint.TryParse(parameters[2], out duration))
                duration = 10000;

            StoryPanelType storyPanelType = 0;
            if (parameters.Length >= 4 && !System.Enum.TryParse(parameters[3], out storyPanelType))
                storyPanelType = 0;

            WindowType windowTypeId = 0;
            if (parameters.Length >= 5 && !System.Enum.TryParse(parameters[4], out windowTypeId))
                windowTypeId = 0;

            uint soundEventId = 0;
            if (parameters.Length >= 6 && !uint.TryParse(parameters[5], out soundEventId))
                soundEventId = 0;

            byte unknown = 0;
            if (parameters.Length >= 7 && !byte.TryParse(parameters[5], out unknown))
                unknown = 0;

            StoryBuilder.Instance.SendStoryCommunicator(textId, creatureId, context.Session.Player, duration, storyPanelType, windowTypeId, soundEventId, unknown);

            return Task.CompletedTask;
        }
    }
}
