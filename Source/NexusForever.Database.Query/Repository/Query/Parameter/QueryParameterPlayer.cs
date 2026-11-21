using System.Linq.Expressions;
using NexusForever.Database.Query.Model;

namespace NexusForever.Database.Query.Repository.Query.Parameter
{
    public class QueryParameterPlayer : IQueryParameter
    {
        public string Name { get; set; }

        public Expression<Func<CharacterModel, bool>> Express()
        {
            return c => c.Name.Contains(Name);
        }
    }
}
