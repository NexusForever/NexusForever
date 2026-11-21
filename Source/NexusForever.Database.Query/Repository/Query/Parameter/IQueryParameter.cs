using System.Linq.Expressions;
using NexusForever.Database.Query.Model;

namespace NexusForever.Database.Query.Repository.Query.Parameter
{
    public interface IQueryParameter
    {
        Expression<Func<CharacterModel, bool>> Express();
    }
}
