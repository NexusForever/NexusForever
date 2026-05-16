using System.Linq.Expressions;
using NexusForever.Database.Query.Model;

namespace NexusForever.Database.Query.Repository.Query.Parameter
{
    public class QueryParameterPath : IQueryParameter
    {
        public Game.Static.PlayerPath.Path Path { get; set; }

        public Expression<Func<CharacterModel, bool>> Express()
        {
            return c => c.Path == Path;
        }
    }
}
