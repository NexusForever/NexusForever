using Microsoft.Extensions.DependencyInjection;
using NexusForever.Database.Query.Model;
using NexusForever.Database.Query.Repository;
using NexusForever.Database.Query.Repository.Query;

namespace NexusForever.Server.Character.Game.Who
{
    public class QueryExecutor
    {
        #region Dependency Injection

        private readonly IServiceProvider _serviceProvider;
        private readonly QueryRepository _repository;

        public QueryExecutor(
            IServiceProvider serviceProvider,
            QueryRepository repository)
        {
            _serviceProvider = serviceProvider;
            _repository      = repository;
        }

        #endregion

        public async Task<List<Character.Character>> QueryAsync(Query query)
        {
            var characters = new List<Character.Character>();
            foreach (CharacterModel model in await _repository.QueryAsync(query))
            {
                var character = _serviceProvider.GetRequiredService<Character.Character>();
                character.Initialise(model);
                characters.Add(character);
            }

            return characters;
        }
    }
}
