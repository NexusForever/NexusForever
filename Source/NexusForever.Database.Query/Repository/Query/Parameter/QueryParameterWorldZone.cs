using System.Linq.Expressions;
using NexusForever.Database.Query.Model;

namespace NexusForever.Database.Query.Repository.Query.Parameter
{
    public class QueryParameterWorldZone : IQueryParameter
    {
        public ushort WorldZoneId { get; set; }

        public Expression<Func<CharacterModel, bool>> Express()
        {
            return c => c.WorldZoneId == WorldZoneId;
        }
    }
}
