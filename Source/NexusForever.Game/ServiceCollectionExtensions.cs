using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract;
using NexusForever.Game.Achievement;
using NexusForever.Game.Character;
using NexusForever.Game.Cinematic;
using NexusForever.Game.Combat;
using NexusForever.Game.Customisation;
using NexusForever.Game.Entity;
using NexusForever.Game.Guild;
using NexusForever.Game.Housing;
using NexusForever.Game.Map;
using NexusForever.Game.Prerequisite;
using NexusForever.Game.Quest;
using NexusForever.Game.RBAC;
using NexusForever.Game.Reputation;
using NexusForever.Game.Server;
using NexusForever.Game.Social;
using NexusForever.Game.Spell;
using NexusForever.Game.Storefront;
using NexusForever.Game.Text;
using NexusForever.Shared;

namespace NexusForever.Game
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGame(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IAssetManager, AssetManager>();
            sc.AddSingletonLegacy<ICleanupManager, CleanupManager>();
            sc.AddSingletonLegacy<IDisableManager, DisableManager>();
            sc.AddSingletonLegacy<IItemManager, ItemManager>();
            sc.AddSingletonLegacy<IRealmContext, RealmContext>();
            sc.AddSingletonLegacy<IShutdownManager, ShutdownManager>();
            sc.AddSingletonLegacy<IStoryBuilder, StoryBuilder>();
            sc.AddSingletonLegacy<IDamageCalculator, DamageCalculator>();

            sc.AddGameAchievement();
            sc.AddGameCharacter();
            sc.AddGameCinematic();
            sc.AddGameCustomisation();
            sc.AddGameEntity();
            sc.AddGameGuild();
            sc.AddGameHousing();
            sc.AddGameMap();
            sc.AddGamePrerequisite();
            sc.AddGameQuest();
            sc.AddGameRbac();
            sc.AddGameReputation();
            sc.AddGameServer();
            sc.AddGameSocial();
            sc.AddGameSpell();
            sc.AddGameStore();
            sc.AddGameText();
        }
    }
}
