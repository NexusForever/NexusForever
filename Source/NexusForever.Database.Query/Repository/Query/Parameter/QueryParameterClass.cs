using System.Linq.Expressions;
using NexusForever.Database.Query.Model;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Database.Query.Repository.Query.Parameter
{
    public class QueryParameterClass : IQueryParameter
    {
        public Class Class { get; set; }

        public Expression<Func<CharacterModel, bool>> Express()
        {
            return c => c.Class == Class;
        }
    }
}
