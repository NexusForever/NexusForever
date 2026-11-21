using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Story;

namespace NexusForever.Game.Story
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameStory(this IServiceCollection sc)
        {
            sc.AddSingleton<IStoryBuilder, StoryBuilder>();
            sc.AddTransient<IStoryMessageBuilder, StoryMessageBuilder>();
        }
    }
}
