using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Character;
using NexusForever.Shared;

namespace NexusForever.Game.Character
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameCharacter(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<ICharacterManager, CharacterManager>();
        }
    }
}
