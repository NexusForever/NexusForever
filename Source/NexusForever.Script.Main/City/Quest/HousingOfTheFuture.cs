using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable;
using NexusForever.Script.Template;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Main.City.Quest
{
    public class HousingOfTheFuture
    {
        private enum StoryPanelIds
        {
            ZenPond        = 2291,
            PowerGenerator = 2292,
            Windmill       = 2294,
            StorageUnit    = 2293
        }

        // 54400: Zen Pond (Thayd)
        // 65296: Zen Pond (Illium)
        [ScriptFilterCreatureId(54400, 65296)]
        public class ZenPond : ISimpleScript, IOwnedScript<ISimple>
        {
            private ISimple owner;

            #region Dependency Injection

            private readonly IStoryBuilder storyBuilder;
            private readonly IGameTableManager gameTableManager;

            public ZenPond(
                IStoryBuilder storyBuilder,
                IGameTableManager gameTableManager)
            {
                this.storyBuilder = storyBuilder;
                this.gameTableManager = gameTableManager;
            }

            #endregion

            public void OnLoad(ISimple owner)
            {
                this.owner = owner;
            }

            public void OnActivateSuccess(IPlayer activator)
            {
                storyBuilder.SendStoryPanel(gameTableManager.StoryPanel.GetEntry(StoryPanelIds.ZenPond), activator);
            }
        }

        // 54401: Windmill (Thayd)
        // 65297: Windmill (Illium)
        [ScriptFilterCreatureId(54401, 65297)]
        public class Windmill : ISimpleScript, IOwnedScript<ISimple>
        {
            private ISimple owner;

            #region Dependency Injection

            private readonly IStoryBuilder storyBuilder;
            private readonly IGameTableManager gameTableManager;

            public Windmill(
                IStoryBuilder storyBuilder,
                IGameTableManager gameTableManager)
            {
                this.storyBuilder = storyBuilder;
                this.gameTableManager = gameTableManager;
            }

            #endregion

            public void OnLoad(ISimple owner)
            {
                this.owner = owner;
            }

            public void OnActivateSuccess(IPlayer activator)
            {
                storyBuilder.SendStoryPanel(gameTableManager.StoryPanel.GetEntry(StoryPanelIds.Windmill), activator);
            }
        }

        // 54403: Power Generator (Thayd)
        // 65298: Power Generator (Illium)
        [ScriptFilterCreatureId(54403, 65298)]
        public class PowerGenerator : ISimpleScript, IOwnedScript<ISimple>
        {
            private ISimple owner;

            #region Dependency Injection

            private readonly IStoryBuilder storyBuilder;
            private readonly IGameTableManager gameTableManager;

            public PowerGenerator(
                IStoryBuilder storyBuilder,
                IGameTableManager gameTableManager)
            {
                this.storyBuilder = storyBuilder;
                this.gameTableManager = gameTableManager;
            }

            #endregion

            public void OnLoad(ISimple owner)
            {
                this.owner = owner;
            }

            public void OnActivateSuccess(IPlayer activator)
            {
                storyBuilder.SendStoryPanel(gameTableManager.StoryPanel.GetEntry(StoryPanelIds.PowerGenerator), activator);
            }
        }

        // 54404: Storage Unit (Thayd)
        // 65299: Storage Unit (Illium)
        [ScriptFilterCreatureId(54404, 65299)]
        public class StorageUnit : ISimpleScript, IOwnedScript<ISimple>
        {
            private ISimple owner;

            #region Dependency Injection

            private readonly IStoryBuilder storyBuilder;
            private readonly IGameTableManager gameTableManager;

            public StorageUnit(
                IStoryBuilder storyBuilder,
                IGameTableManager gameTableManager)
            {
                this.storyBuilder = storyBuilder;
                this.gameTableManager = gameTableManager;
            }

            #endregion

            public void OnLoad(ISimple owner)
            {
                this.owner = owner;
            }

            public void OnActivateSuccess(IPlayer activator)
            {
                storyBuilder.SendStoryPanel(gameTableManager.StoryPanel.GetEntry(StoryPanelIds.StorageUnit), activator);
            }
        }
    }
}
