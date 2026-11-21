using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Query.Model;
using NexusForever.Database.Query.Repository.Query;

namespace NexusForever.Database.Query.Repository
{
    public class QueryRepository
    {
        #region Dependency Injection

        private readonly QueryContext _context;
        private readonly QueryExpressionBuilder _expressionBuilder;

        public QueryRepository(
            QueryContext context,
            QueryExpressionBuilder expressionBuilder)
        {
            _context           = context;
            _expressionBuilder = expressionBuilder;
        }

        #endregion

        public async Task<List<CharacterModel>> QueryAsync(Query.Query query)
        {
            Expression<Func<CharacterModel, bool>> expression = _expressionBuilder.Build(query);

            IQueryable<CharacterModel> databaseQuery = _context.Character.AsQueryable()
                .Where(c => c.LastOnline == null)
                .Where(expression)
                .OrderBy(c => c.CharacterId)
                .Take((int)query.MaxResults);

            return await databaseQuery.ToListAsync();
        }
    }
}
