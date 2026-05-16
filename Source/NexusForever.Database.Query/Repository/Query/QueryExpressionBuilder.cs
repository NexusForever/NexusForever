using System.Linq.Expressions;
using NexusForever.Database.Query.Model;
using NexusForever.Database.Query.Repository.Query.Parameter;

namespace NexusForever.Database.Query.Repository.Query
{
    public class QueryExpressionBuilder
    {
        private ParameterExpression _parameter;

        public Expression<Func<CharacterModel, bool>> Build(Query query)
        {
            _parameter = Expression.Parameter(typeof(CharacterModel), "c");

            Expression body = null;
            foreach (QueryGroup group in query.Groups)
                body = BuildGroup(body, group);

            body ??= Expression.Constant(true);

            return Expression.Lambda<Func<CharacterModel, bool>>(body, _parameter);
        }

        private Expression BuildGroup(Expression body, QueryGroup group)
        {
            Expression groupBody = null;
            foreach (IQueryParameter parameter in group.Parameters)
                groupBody = BuildParameter(groupBody, parameter);

            if (body == null)
                return groupBody;
            else
                return Expression.OrElse(body, groupBody);
        }   

        private Expression BuildParameter(Expression body, IQueryParameter parameter)
        {
            Expression<Func<CharacterModel, bool>> parameterBody = parameter.Express();

            var visitor = new ParameterVisitor(parameterBody.Parameters[0], _parameter).Visit(parameterBody.Body);
            if (body == null)
                return visitor;
            else
                return Expression.AndAlso(body, visitor);
        }
    }
}
