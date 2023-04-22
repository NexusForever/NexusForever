﻿using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static;
using NexusForever.Game.Static.RBAC;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Story, "A collection of commands to send story content to characters.", "story")]
    [CommandTarget(typeof(IPlayer))]
    public class StoryCommandCategory : CommandCategory
    {
        [Command(Permission.StoryPanel, "Send a story panel to a character.", "panel", "p")]
        public void HandleStoryPanel(ICommandContext context,
            [Parameter("Story panel entry to send to character.")]
            uint storyPanelId)
        {
            StoryPanelEntry entry = GameTableManager.Instance.StoryPanel.GetEntry(storyPanelId);
            if (entry == null)
            {
                context.SendError($"Invalid story panel entry {storyPanelId}!");
                return;
            }

            StoryBuilder.Instance.SendStoryPanel(entry, context.GetTargetOrInvoker<IPlayer>());
        }

        [Command(Permission.StoryCommunicator, "Send a story communicator window to a character.", "communicator", "c")]
        public void TestSubCommand(ICommandContext context,
            [Parameter("")]
            uint textId,
            [Parameter("")]
            uint creatureId,
            [Parameter("")]
            uint? duration,
            [Parameter("", ParameterFlags.None, typeof(EnumParameterConverter<StoryPanelType>))]
            StoryPanelType? storyPanelType,
            [Parameter("", ParameterFlags.None, typeof(EnumParameterConverter<WindowType>))]
            WindowType? windowType,
            [Parameter("")]
            uint? soundEvent,
            [Parameter("")]
            byte? priority)
        {
            duration       ??= 10000u;
            storyPanelType ??= StoryPanelType.Default;
            windowType     ??= WindowType.LeftAligned;
            soundEvent     ??= 0u;
            priority       ??= 0;

            StoryBuilder.Instance.SendStoryCommunicator(textId, creatureId, context.GetTargetOrInvoker<IPlayer>(),
                duration.Value, storyPanelType.Value, windowType.Value, soundEvent.Value, priority.Value);
        }
    }
}
