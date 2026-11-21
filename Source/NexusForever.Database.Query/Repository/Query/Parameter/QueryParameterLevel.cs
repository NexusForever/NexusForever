using System.Linq.Expressions;
using NexusForever.Database.Query.Model;

namespace NexusForever.Database.Query.Repository.Query.Parameter
{
    public class QueryParameterLevel : IQueryParameter
    {
        public uint BottomLevel { get; set; }
        public uint TopLevel { get; set; }

        public Expression<Func<CharacterModel, bool>> Express()
        {
            return c => c.Level >= BottomLevel && c.Level < TopLevel;
        }
    }
}
