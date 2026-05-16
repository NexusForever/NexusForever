using System;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Story;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Story;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Shared;
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

            IStoryBuilder storyBuilder = LegacyServiceProvider.Provider.GetService<IStoryBuilder>();
            storyBuilder.SendServerStoryPanelShow(context.GetTargetOrInvoker<IPlayer>(), entry.Id);
        }

        [Command(Permission.StoryCommunicator, "Send a story communicator window to a character.", "communicator", "c")]
        public void TestSubCommand(ICommandContext context,
            [Parameter("")]
            uint localisedTextId,
            [Parameter("")]
            uint creatureId,
            [Parameter("")]
            uint? duration,
            [Parameter("", ParameterFlags.None, typeof(EnumParameterConverter<CommunicatorOverlay>))]
            CommunicatorOverlay? overlay,
            [Parameter("", ParameterFlags.None, typeof(EnumParameterConverter<CommunicatorPortraitPlacement>))]
            CommunicatorPortraitPlacement? placement,
            [Parameter("", ParameterFlags.None, typeof(EnumParameterConverter<CommunicatorBackground>))]
            CommunicatorBackground? background)
        {
            duration       ??= 10000u;
            overlay        ??= CommunicatorOverlay.Default;
            placement      ??= CommunicatorPortraitPlacement.Left;
            background     ??= CommunicatorBackground.Default;

            IStoryBuilder storyBuilder = LegacyServiceProvider.Provider.GetService<IStoryBuilder>();
            storyBuilder.SendServerStoryTextCommunicator(context.GetTargetOrInvoker<IPlayer>(), localisedTextId, creatureId,
                TimeSpan.FromSeconds(duration.Value), overlay.Value, placement.Value, background.Value);
        }
    }
}
