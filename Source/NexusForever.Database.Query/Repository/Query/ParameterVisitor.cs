using System.Linq.Expressions;

namespace NexusForever.Database.Query.Repository.Query
{
    public class ParameterVisitor : ExpressionVisitor
    {
        #region Dependency Injection

        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ParameterVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to   = to;
        }

        #endregion

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _from ? _to : base.VisitParameter(node);
        }
    }
}
